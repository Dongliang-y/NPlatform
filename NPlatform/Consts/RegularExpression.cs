namespace NPlatform.Consts
{
    /// <summary>
    /// 校验用的正则表达式
    /// </summary>
    public class RegularExpression
    {
        public const string PasswordRex = @"^(?![a-zA-z]+$)(?!\d+$)(?![!@#$%^&*]+$)[a-zA-Z\d!@#$%^&*().]+$";
    }
}
