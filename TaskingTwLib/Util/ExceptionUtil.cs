using System;

namespace Azyobuzi.TaskingTwLib.Util
{
    static class ExceptionUtil
    {
        /// <summary>
        /// <see cref="AggregateException"/>のInnerExceptionをいけるところまで掘り起こす
        /// </summary>
        public static Exception DigUpException(this AggregateException ex)
        {
            Exception re = ex;

            do
            {
                re = re.InnerException;
            }
            while (re is AggregateException);

            return re;
        }
    }
}
