using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azyobuzi.TaskingTwLib.DataModels;
using Azyobuzi.TaskingTwLib.Util;

namespace Azyobuzi.TaskingTwLib.Methods.Tweets
{
    /// <summary>
    /// statuses/update、statuses/update_with_media
    /// </summary>
    public class UpdateApi : ITwitterApi<Status>
    {
        private UpdateApi() { }

        private string requestUri;
        string ITwitterApi<Status>.RequestUri
        {
            get { return this.requestUri; }
        }

        HttpMethodType ITwitterApi<Status>.MethodType
        {
            get { return HttpMethodType.POST; }
        }

        private List<FormData> parameters = new List<FormData>();
        IEnumerable<FormData> ITwitterApi<Status>.Parameters
        {
            get { return this.parameters; }
        }

        private string contentType;
        string ITwitterApi<Status>.ContentType
        {
            get { return this.contentType; }
        }

        string ITwitterApi<Status>.Verifier
        {
            get { return null; }
        }

        string ITwitterApi<Status>.Callback
        {
            get { return null; }
        }

        Status ITwitterApi<Status>.Parse(string response, Token token)
        {
            return ResponseParser.ParseStatus(response);
        }

        /// <summary>
        /// statuses/updateのリクエストを作成します。
        /// </summary>
        /// <param name="status">ツイート内容</param>
        /// <param name="inReplyToStatusId">返信先の<see cref="P:Status.Id"/></param>
        /// <param name="lat">緯度</param>
        /// <param name="long">経度</param>
        /// <param name="placeId">場所・地域のID</param>
        /// <param name="displayCoordinates">座標にピンを置くかどうか</param>
        /// <returns>リクエスト</returns>
        public static UpdateApi Create(string status, ulong? inReplyToStatusId = null, double? lat = null, double? @long = null, string placeId = null, bool displayCoordinates = false)
        {
            var re = new UpdateApi();

            re.requestUri = "http://api.twitter.com/1/statuses/update.json";
            re.contentType = HttpContentType.ApplicationXWwwFormUrlencoded;

            re.parameters.Add(new FormData("status", status));
            re.parameters.Add(new FormData("include_entities", "true"));

            if (inReplyToStatusId.HasValue)
                re.parameters.Add(new FormData("in_reply_to_status_id", inReplyToStatusId.Value.ToString()));

            if (lat.HasValue)
                re.parameters.Add(new FormData("lat", lat.Value.ToString()));

            if (@long.HasValue)
                re.parameters.Add(new FormData("long", @long.Value.ToString()));

            if (!string.IsNullOrEmpty(placeId))
                re.parameters.Add(new FormData("place_id", placeId));

            if (displayCoordinates)
                re.parameters.Add(new FormData("display_coordinates", "true"));

            return re;
        }

        /// <summary>
        /// statuses/update_with_mediaのリクエストを作成します。
        /// </summary>
        /// <param name="status">ツイート内容</param>
        /// <param name="mediaFiles">アップロードするメディアファイル</param>
        /// <param name="possiblySensitive">不適切なコンテンツを含む場合は<c>true</c>を指定してください。</param>
        /// <param name="inReplyToStatusId">返信先の<see cref="P:Status.Id"/></param>
        /// <param name="lat">緯度</param>
        /// <param name="long">経度</param>
        /// <param name="placeId">場所・地域のID</param>
        /// <param name="displayCoordinates">座標にピンを置くかどうか</param>
        /// <returns>リクエスト</returns>
        public static UpdateApi Create(string status, IEnumerable<string> mediaFiles, bool possiblySensitive = false, ulong? inReplyToStatusId = null, double? lat = null, double? @long = null, string placeId = null, bool displayCoordinates = false)
        {
            var re = new UpdateApi();

            re.requestUri = "https://upload.twitter.com/1/statuses/update_with_media.json";
            re.contentType = HttpContentType.MultipartFormData;

            re.parameters.Add(new FormData("status", status));
            re.parameters.AddRange(mediaFiles.Select(file => new FormData("media[]", file, true, MimeTypeHelper.GetMimeTypeFromExtension(Path.GetExtension(file)))));

            if (possiblySensitive)
                re.parameters.Add(new FormData("possibly_sensitive", "true"));

            if (inReplyToStatusId.HasValue)
                re.parameters.Add(new FormData("in_reply_to_status_id", inReplyToStatusId.Value.ToString()));

            if (lat.HasValue)
                re.parameters.Add(new FormData("lat", lat.Value.ToString()));

            if (@long.HasValue)
                re.parameters.Add(new FormData("long", @long.Value.ToString()));

            if (!string.IsNullOrEmpty(placeId))
                re.parameters.Add(new FormData("place_id", placeId));

            if (displayCoordinates)
                re.parameters.Add(new FormData("display_coordinates", "true"));

            return re;
        }
    }
}
