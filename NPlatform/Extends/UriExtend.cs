namespace NPlatform.Extends
{
    /// <summary>
    ///  url 扩展
    /// </summary>
    public static class UriExtend
    {
        /// <summary>
        ///检查URL是否为 http或者https
        /// </summary>
        /// <returns></returns>
        public static bool IsValidUri(this string url)
        {
            return (!string.IsNullOrEmpty(url) && (url.StartsWith("https", StringComparison.Ordinal)
               || url.StartsWith("http", StringComparison.Ordinal)));
        }
    }
}
