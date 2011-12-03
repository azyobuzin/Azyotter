using System;
using System.IO;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azyobuzi.TaskingTwLib.OAuth;

namespace Azyobuzi.TaskingTwLib
{
    /// <summary>
    /// TwitterAPIを呼び出します。
    /// </summary>
    public static class TwitterApiExecutor
    {
        /// <summary>
        /// TwitterAPIを呼び出します。
        /// </summary>
        /// <typeparam name="T">APIからの戻り値の型</typeparam>
        /// <param name="apiMethod">呼び出すAPI</param>
        /// <param name="token">OAuthトークン</param>
        /// <param name="cancellationToken">タスクに割り当てられる<see cref="T:CancellationToken"/></param>
        /// <returns>実行中のタスク</returns>
        public static Task<T> CallApi<T>(this ITwitterApi<T> apiMethod, Token token, CancellationToken? cancellationToken)
        {
            Func<T> action = () =>
            {
                var req = RequestGenerator.GenerateTwitterApiRequest(
                    apiMethod.RequestUri,
                    apiMethod.MethodType,
                    token,
                    DefaultSetting.Proxy,
                    DefaultSetting.Timeout,
                    DefaultSetting.UserAgent,
                    apiMethod.ContentType,
                    apiMethod.Parameters,
                    apiMethod.Verifier,
                    apiMethod.Callback);

                using (var res = req.GetResponse())
                {
                    //TODO:RateLimit読み取り

                    using (var sr = new StreamReader(res.GetResponseStream()))
                    {
                        return apiMethod.Parse(sr.ReadToEnd(), token);
                    }
                }
            };

            if (cancellationToken.HasValue)
                return Task<T>.Factory.StartNew(action, cancellationToken.Value);
            else
                return Task<T>.Factory.StartNew(action);
        }

        /// <summary>
        /// TwitterAPIを呼び出します。
        /// </summary>
        /// <typeparam name="T">APIからの戻り値の型</typeparam>
        /// <param name="apiMethod">呼び出すAPI</param>
        /// <param name="token">OAuthトークン</param>
        /// <returns>実行中のタスク</returns>
        public static Task<T> CallApi<T>(this ITwitterApi<T> apiMethod, Token token)
        {
            return CallApi(apiMethod, token, null);
        }

        /// <summary>
        /// Streaming APIを呼び出します。
        /// </summary>
        /// <typeparam name="T">APIからの戻り値の型</typeparam>
        /// <param name="apiMethod">呼び出すAPI</param>
        /// <param name="token">OAuthトークン</param>
        /// <returns>APIからのレスポンスをpush通知する<see cref="IObservable{T}"/></returns>
        public static IObservable<T> CallApi<T>(this IStreamingApi<T> apiMethod, Token token)
        {
            return Observable.Create<T>(observer =>
            {
                HttpWebRequest req = null;

                Scheduler.TaskPool.Schedule(() =>
                {
                    try
                    {
                        req = RequestGenerator.GenerateTwitterApiRequest(
                            apiMethod.RequestUri,
                            HttpMethodType.GET,
                            token,
                            DefaultSetting.Proxy,
                            DefaultSetting.Timeout,
                            DefaultSetting.UserAgent,
                            null,
                            apiMethod.Parameters,
                            null,
                            null
                        );

                        using (var res = req.GetResponse())//あとでレスポンスも扱うかもしれないので一応定義
                        using (var sr = new StreamReader(res.GetResponseStream()))
                        {
                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine();

                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    Scheduler.ThreadPool.Schedule(() =>
                                       observer.OnNext(apiMethod.Parse(line)));
                                }
                            }

                            observer.OnCompleted();
                        }
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status != WebExceptionStatus.RequestCanceled)
                        {
                            observer.OnError(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });

                return () =>
                {
                    if (req != null)
                        req.Abort();
                };
            });
        }
    }
}
