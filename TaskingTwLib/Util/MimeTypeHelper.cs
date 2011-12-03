namespace Azyobuzi.TaskingTwLib.Util
{
    /// <summary>
    /// MIMEタイプ関連
    /// </summary>
    public static class MimeTypeHelper
    {
        /// <summary>
        /// 拡張子からMIMEタイプを取得します。
        /// </summary>
        /// <param name="extension">拡張子</param>
        /// <returns>MIMEタイプ</returns>
        public static string GetMimeTypeFromExtension(string extension)
        {
            switch (extension.TrimStart('.').ToLower())
            {
                case "jpg":
                case "jpeg":
                case "jpe":
                    return "image/jpeg";
                case "gif":
                    return "image/gif";
                case "png":
                    return "image/png";
                case "tiff":
                case "tif":
                    return "image/tiff";
                case "bmp":
                    return "image/x-bmp";
                case "avi":
                    return "video/avi";
                case "wmv":
                    return "video/x-ms-wmv";
                case "flv":
                    return "video/x-flv";
                case "m4v":
                    return "video/x-m4v";
                case "mov":
                    return "video/quicktime";
                case "mp4":
                    return "video/3gpp";
                case "rm":
                    return "application/vndrn-realmedia";
                case "mpeg":
                case "mpg":
                    return "video/mpeg";
                case "3gp":
                    return "movie/3gp";
                case "3g2":
                    return "video/3gpp2";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
