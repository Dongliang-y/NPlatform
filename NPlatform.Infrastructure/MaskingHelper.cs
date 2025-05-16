namespace NPlatform.Infrastructure
{
    using System;
    using System.Linq; // For Where().ToArray()
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 掩码助手类
    /// </summary>
    public static class MaskingHelper
    {
        // --- 掩码方法 (基本保持不变，稍微优化或增加注释) ---

        /// <summary>
        /// 掩码邮箱地址
        /// 例如: test@example.com -> t**t@example.com 或 te*****@example.com
        /// 这里采取保留开头和结尾少量字符，中间用星号代替的策略
        /// </summary>
        /// <param name="email">原始邮箱地址</param>
        /// <returns>掩码后的邮箱地址</returns>
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return email;

            var parts = email.Split('@');
            if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                return email; // 不是标准邮箱格式或为空，不处理

            string localPart = parts[0];
            string domainPart = parts[1];

            // 保留本地部分的第一个和最后一个字符，中间用星号代替
            // 如果本地部分太短，进行简化处理
            if (localPart.Length <= 2)
            {
                // 如果只有1个字符，如 "a@b.com"，掩码为 "a**@b.com" (根据 MaskEmailTest 的第一个策略)
                // 如果只有2个字符，如 "ab@c.com"，掩码为 "ab**@c.com"
                // 这个策略有点模糊，我们更倾向于保留前缀+掩码+后缀的模式
                // 修改策略：保留前1位和后1位，中间掩码。如果长度不足3，则保留前1位，后面全部掩码。
                if (localPart.Length == 1) return $"{localPart}**@{domainPart}";
                if (localPart.Length == 2) return $"{localPart[0]}**@{domainPart}"; // 保留第一位，后面掩码
                                                                                    // 长度 >= 3 时，保留前1后1
                return $"{localPart[0]}**{localPart[localPart.Length - 1]}@{domainPart}";
            }

            // 保留本地部分的第一个和最后一个字符，中间用双星号代替 (为了和 IsMaskedEmail 匹配，固定用 **)
            // 这个策略产生 `c**c@domain` 的模式
            if (localPart.Length > 2)
            {
                return $"{localPart[0]}**{localPart[localPart.Length - 1]}@{domainPart}";
            }

            // Fallback or specific short local part handling if the above logic needs adjustment
            // Let's refine based on common patterns: retain prefix and suffix if long enough, otherwise simple mask
            const int prefixLength = 1; // 保留本地部分前缀长度
            const int suffixLength = 1; // 保留本地部分后缀长度

            if (localPart.Length <= prefixLength + suffixLength)
            {
                // 如果本地部分长度不足以保留前后缀，则保留前缀，其余掩码
                return $"{localPart.Substring(0, Math.Min(localPart.Length, prefixLength))}**@{domainPart}";
            }
            else
            {
                // 保留前缀和后缀，中间掩码
                int charsToMask = localPart.Length - prefixLength - suffixLength;
                return $"{localPart.Substring(0, prefixLength)}{new string('*', charsToMask)}{localPart.Substring(localPart.Length - suffixLength)}@{domainPart}";
            }

        }

        /// <summary>
        /// 掩码手机号码
        /// 例如: 13812345678 -> 138****5678
        /// </summary>
        /// <param name="mobile">原始手机号码</param>
        /// <returns>掩码后的手机号码</returns>
        public static string MaskMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return mobile;

            string digitsOnly = new string(mobile.Where(char.IsDigit).ToArray()); // 仅保留数字

            if (digitsOnly.Length < 11) // 至少需要11位进行掩码（国内手机号通常11位）
                return mobile; // 号码过短或格式不符合预期，不处理

            // 保留前3位和后4位，中间用4个星号代替 (国内11位手机号的常见掩码方式)
            // 即使是更长的数字串，也遵循这个规则，掩码中间部分
            return $"{digitsOnly.Substring(0, 3)}****{digitsOnly.Substring(digitsOnly.Length - 4)}";
        }

        /// <summary>
        /// 掩码身份证号
        /// 例如: 34012319900101123X -> 340123********123X
        /// </summary>
        /// <param name="cardNum">原始身份证号</param>
        /// <returns>掩码后的身份证号</returns>
        public static string MaskCardNum(string cardNum)
        {
            if (string.IsNullOrEmpty(cardNum))
                return cardNum;

            // 仅保留字母和数字 (身份证号末尾可能包含X)
            string alphaNumericOnly = new string(cardNum.Where(c => char.IsLetterOrDigit(c)).ToArray());


            const int charsToKeepStart = 6; // 保留前6位
            const int charsToKeepEnd = 4;   // 保留后4位 (包括可能的X)

            if (alphaNumericOnly.Length < charsToKeepStart + charsToKeepEnd)
                return cardNum; // 号码过短，不处理

            // 计算需要掩码的字符数
            int charsToMask = alphaNumericOnly.Length - charsToKeepStart - charsToKeepEnd;

            // 保留前6位和后4位，中间用星号代替
            return $"{alphaNumericOnly.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{alphaNumericOnly.Substring(alphaNumericOnly.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码银行卡号
        /// 例如: 6228481234567890 -> 6228********7890
        /// 这里采取保留前4位和后4位，中间用星号代替的策略
        /// </summary>
        /// <param name="accountNumber">原始银行卡号</param>
        /// <returns>掩码后的银行卡号</returns>
        public static string MaskBankAccountNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return accountNumber;

            string digitsOnly = new string(accountNumber.Where(char.IsDigit).ToArray()); // 仅保留数字

            const int charsToKeepStart = 4; // 保留前4位
            const int charsToKeepEnd = 4;   // 保留后4位

            if (digitsOnly.Length < charsToKeepStart + charsToKeepEnd)
                return accountNumber; // 号码过短，不处理 (至少需要8位)

            // 计算需要掩码的字符数
            int charsToMask = digitsOnly.Length - charsToKeepStart - charsToKeepEnd;

            // 保留前4位和后4位，中间用星号代替
            return $"{digitsOnly.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{digitsOnly.Substring(digitsOnly.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码社保或公积金账号
        /// 策略类似银行卡号或身份证，保留部分开头和结尾，掩码中间
        /// </summary>
        /// <param name="accountNumber">原始账号</param>
        /// <returns>掩码后的账号</returns>
        public static string MaskSocialProvidentFundAccount(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return accountNumber;

            // 根据实际账号格式保留数字或字母数字
            string cleaned = new string(accountNumber.Where(char.IsLetterOrDigit).ToArray());

            const int charsToKeepStart = 4; // 保留前4位
            const int charsToKeepEnd = 4;   // 保留后4位

            if (cleaned.Length < charsToKeepStart + charsToKeepEnd) // 需要足够长进行掩码
                return accountNumber; // 号码过短，不处理

            // 保留前4位和后4位，中间用星号代替 (可根据实际账号格式调整)
            int charsToMask = cleaned.Length - charsToKeepStart - charsToKeepEnd;

            return $"{cleaned.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{cleaned.Substring(cleaned.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码护照号码
        /// 策略类似身份证，保留部分开头和结尾，掩码中间
        /// </summary>
        /// <param name="passportNumber">原始护照号码</param>
        /// <returns>掩码后的护照号码</returns>
        public static string MaskPassportNumber(string passportNumber)
        {
            if (string.IsNullOrEmpty(passportNumber))
                return passportNumber;

            string alphaNumericOnly = new string(passportNumber.Where(char.IsLetterOrDigit).ToArray()); // 仅保留字母数字

            const int charsToKeepStart = 2; // 保留前2位 (通常护照有字母前缀)
            const int charsToKeepEnd = 4;   // 保留后4位

            if (alphaNumericOnly.Length < charsToKeepStart + charsToKeepEnd) // 需要足够长进行掩码，例如前2后4
                return passportNumber; // 号码过短，不处理

            int charsToMask = alphaNumericOnly.Length - (charsToKeepStart + charsToKeepEnd);

            return $"{alphaNumericOnly.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{alphaNumericOnly.Substring(alphaNumericOnly.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码驾驶证号码 (假设格式类似身份证，掩码中间)
        /// </summary>
        /// <param name="licenseNumber">原始驾驶证号码</param>
        /// <returns>掩码后的驾驶证号码</returns>
        public static string MaskDriversLicenseNumber(string licenseNumber)
        {
            if (string.IsNullOrEmpty(licenseNumber))
                return licenseNumber;

            string alphaNumericOnly = new string(licenseNumber.Where(char.IsLetterOrDigit).ToArray());

            const int charsToKeepStart = 6; // 假设保留前6位
            const int charsToKeepEnd = 4;   // 假设保留后4位

            if (alphaNumericOnly.Length < charsToKeepStart + charsToKeepEnd) // 假设需要类似身份证的长度进行掩码
                return licenseNumber; // 号码过短，不处理

            int charsToMask = alphaNumericOnly.Length - (charsToKeepStart + charsToKeepEnd);

            return $"{alphaNumericOnly.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{alphaNumericOnly.Substring(alphaNumericOnly.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码微信号或QQ号
        /// 策略：保留开头和结尾少量字符，中间用星号代替
        /// </summary>
        /// <param name="value">原始微信号或QQ号</param>
        /// <returns>掩码后的字符串</returns>
        public static string MaskWeixinQQ(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            const int charsToKeepStart = 1; // 保留前1位
            const int charsToKeepEnd = 1;   // 保留后1位

            if (value.Length <= charsToKeepStart + charsToKeepEnd) // 过短，不进行掩码 (至少3位才能保留前1后1中间掩码)
                return value;

            int charsToMask = value.Length - (charsToKeepStart + charsToKeepEnd);

            return $"{value.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{value.Substring(value.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码办公电话号码 (处理有区号和无区号的情况，掩码号码中间部分)
        /// 注意: 电话号码格式复杂，此为简化处理。仅对纯数字进行掩码。
        /// </summary>
        /// <param name="officeTel">原始办公电话号码</param>
        /// <returns>掩码后的电话号码</returns>
        public static string MaskOfficeTelNum(string officeTel)
        {
            if (string.IsNullOrEmpty(officeTel))
                return officeTel;

            // 仅保留数字进行掩码处理，原始字符串的非数字部分（如括号、破折号、空格）会丢失
            string digitsOnly = new string(officeTel.Where(char.IsDigit).ToArray());

            const int charsToKeepStart = 4; // 保留前4位
            const int charsToKeepEnd = 4;   // 保留后4位

            if (digitsOnly.Length < charsToKeepStart + charsToKeepEnd) // 至少需要8位数字进行掩码
                return officeTel; // 号码过短，不处理

            int charsToMask = digitsOnly.Length - charsToKeepStart - charsToKeepEnd;

            // 返回掩码后的纯数字字符串
            return $"{digitsOnly.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{digitsOnly.Substring(digitsOnly.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码姓名 (通常保留姓氏，名字部分掩码)
        /// 例如: 张三 -> 张*
        /// 注意: 单名、复姓等情况可能需要特殊处理。此为基础实现。
        /// </summary>
        /// <param name="name">原始姓名</param>
        /// <returns>掩码后的姓名</returns>
        public static string MaskName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            if (name.Length <= 1) // 单字姓名，不掩码
                return name;

            // 保留第一个字符（姓氏），后面的用星号代替
            return $"{name[0]}{new string('*', name.Length - 1)}";

            // 如果需要处理复姓（如 欧阳震华 -> 欧阳**），需要更复杂的逻辑，可能需要姓氏字典或更高级的匹配。
        }

        /// <summary>
        /// 掩码地址 (简单策略：保留前面一部分，后面用星号代替)
        /// 例如: 广东省深圳市南山区科技园科兴路1号 -> 广东省深圳市南山区科******
        /// </summary>
        /// <param name="address">原始地址</param>
        /// <param name="charsToKeepFromStart">从开头保留的字符数</param>
        /// <returns>掩码后的地址</returns>
        public static string MaskAddress(string address, int charsToKeepFromStart = 10)
        {
            if (string.IsNullOrEmpty(address))
                return address;

            // 确保保留字符数不超过地址长度
            int actualCharsToKeep = Math.Min(address.Length, charsToKeepFromStart);

            if (actualCharsToKeep == address.Length) // 地址不够长，不掩码
                return address;

            // 保留前面指定数量的字符，后面用星号代替
            int charsToMask = address.Length - actualCharsToKeep;
            return $"{address.Substring(0, actualCharsToKeep)}{new string('*', charsToMask)}";
        }

        /// <summary>
        /// 掩码数值型敏感数据 (如工资明细的具体数字)
        /// 通常不是字符掩码，而是用占位符表示数据是敏感的。
        /// </summary>
        /// <returns>掩码后的字符串占位符</returns>
        public static string MaskNumericValue()
        {
            return "[保密]"; // 或 "******" 等占位符
        }

        // --- 判断是否为掩码的扩展方法 ---

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的手机号码 (XXX****XXXX)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码手机号模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedMobile(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 3位数字 + 4个星号 + 4位数字 的模式
            // Regex ^\d{3}\*{4}\d{4}$
            return Regex.IsMatch(value, @"^\d{3}\*{4}\d{4}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的邮箱地址 (例如 t**t@domain.com 或 prefix**@domain.com)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码邮箱模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedEmail(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否包含 "@"，并且在 "@" 之前包含 "**"
            // 更精确的匹配掩码逻辑产生的模式: 本地部分包含 ** 且 ** 不在开头/结尾，或本地部分很短以 ** 结尾
            // Regex ^[^@]*\*{2}[^@]*@[^@]+$  // 本地部分包含 **
            // Regex ^[^@]{1,}\*{2}@[^@]+$ // 本地部分以 ** 结尾，前面有1个或多个字符
            // 结合一下：本地部分包含 **，并且至少有1个非*字符在 ** 之前，或者整个本地部分就是几个字符+**
            // 简单判断策略：包含 @ 符号，且在 @ 符号前包含至少连续两个 * 且不是所有字符都是 *
            if (!value.Contains("@")) return false;
            var parts = value.Split('@');
            if (parts.Length != 2 || string.IsNullOrEmpty(parts[0])) return false;
            string localPart = parts[0];
            // 检查本地部分是否包含 **，并且本地部分不等于 "**" 或 "*" 等纯星号串
            return localPart.Contains("**") && !Regex.IsMatch(localPart, @"^\*+$"); // 包含 ** 且不全是 * 号组成
        }


        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的身份证号 (前6位数字/字母 + 多个星号 + 后4位数字/字母)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码身份证号模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedCardNum(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 6位字母数字 + 1个或多个星号 + 4位字母数字 的模式
            // Regex ^[a-zA-Z0-9]{6}\*{1,}[a-zA-Z0-9]{4}$
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]{6}\*{1,}[a-zA-Z0-9]{4}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的银行卡号 (前4位数字 + 多个星号 + 后4位数字)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码银行卡号模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedBankAccountNumber(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 4位数字 + 1个或多个星号 + 4位数字 的模式
            // Regex ^\d{4}\*{1,}\d{4}$
            return Regex.IsMatch(value, @"^\d{4}\*{1,}\d{4}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的社保或公积金账号 (前4位字母数字 + 多个星号 + 后4位字母数字)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码账号模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedSocialProvidentFundAccount(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 4位字母数字 + 1个或多个星号 + 4位字母数字 的模式
            // Regex ^[a-zA-Z0-9]{4}\*{1,}[a-zA-Z0-9]{4}$
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]{4}\*{1,}[a-zA-Z0-9]{4}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的护照号码 (前2位字母数字 + 多个星号 + 后4位字母数字)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码护照号码模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedPassportNumber(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 2位字母数字 + 1个或多个星号 + 4位字母数字 的模式
            // Regex ^[a-zA-Z0-9]{2}\*{1,}[a-zA-Z0-9]{4}$
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]{2}\*{1,}[a-zA-Z0-9]{4}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的驾驶证号码 (前6位字母数字 + 多个星号 + 后4位字母数字)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码驾驶证号码模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedDriversLicenseNumber(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 6位字母数字 + 1个或多个星号 + 4位字母数字 的模式
            // Regex ^[a-zA-Z0-9]{6}\*{1,}[a-zA-Z0-9]{4}$
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]{6}\*{1,}[a-zA-Z0-9]{4}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的微信号或QQ号 (前1位 + 多个星号 + 后1位)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码微信号/QQ号模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedWeixinQQ(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 1位任意字符 + 1个或多个星号 + 1位任意字符 的模式
            // Regex ^.{1}\*{1,}.{1}$  (这里的 . 匹配除换行符外的任意字符)
            return Regex.IsMatch(value, @"^.{1}\*{1,}.{1}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的办公电话号码 (前4位数字 + 多个星号 + 后4位数字)
        /// 注意: 这个判断是基于 MaskOfficeTelNum 清理后返回纯数字+星号的情况。
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码电话号码模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedOfficeTelNum(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 4位数字 + 1个或多个星号 + 4位数字 的模式
            // Regex ^\d{4}\*{1,}\d{4}$
            return Regex.IsMatch(value, @"^\d{4}\*{1,}\d{4}$");
        }


        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的姓名 (第一个字符 + 多个星号)
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码姓名模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedName(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否符合 1个非星号字符 + 1个或多个星号 的模式
            // Regex ^[^\*]\*{1,}$
            return Regex.IsMatch(value, @"^[^\*]\*{1,}$");
        }

        /// <summary>
        /// 判断字符串是否看起来像一个掩码后的地址 (任意字符 + 多个星号)
        /// 注意: 这个判断比较宽松，只检查是否包含星号且星号在非末尾位置，并且以星号结尾。
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果符合掩码地址模式则返回 true，否则返回 false</returns>
        public static bool IsMaskedAddress(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否包含星号，且不以非星号结尾 (即以星号结尾)，并且星号不在开头
            // Regex ^.*[^\*]\*{1,}$  (以任意字符开头，后跟至少一个非星号，然后是一个或多个星号直到结尾)
            return Regex.IsMatch(value, @"^.*[^\*]\*{1,}$");
        }

        /// <summary>
        /// 判断字符串是否是掩码后的数值型敏感数据占位符 ([保密])
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>如果是占位符 "[保密]" 则返回 true，否则返回 false</returns>
        public static bool IsMaskedNumericValue(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            // 检查是否精确匹配占位符字符串
            return value == "[保密]";
        }

        // 可以根据需要添加其他类型的 IsMasked 扩展方法
    }
}
