namespace NPlatform.Consts
{
    /// <summary>
    /// 公用的Redis键值常量
    /// </summary>
    public static class CommonRedisConst
    {
        /// <summary>
        /// 获取第三方登录时缓存手机号码的key
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static string SesstionKey(string token)
        {
            return "AuthInfo:" + token;
        }
    }
}
