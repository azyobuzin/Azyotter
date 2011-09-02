using System;
using LinqToTwitter;

namespace Azyobuzi.Azyotter.Util
{
    static class ExceptionUtil
    {
        public static string GetMessage(this Exception ex)
        {
            var tqe = ex as TwitterQueryException;

            if (tqe != null)
            {
                return tqe.Response.Error;
            }
            else
            {
                return ex.Message;
            }
        }
    }
}
