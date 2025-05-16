namespace NPlatform.Infrastructure
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions; // Needed for potential regex masking, although string manipulation is often sufficient
    /// <summary>
    /// 掩码助手类
    /// </summary>
    public static class MaskingHelper
    {
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
            if (parts.Length != 2)
                return email; // 不是标准邮箱格式，不处理

            string localPart = parts[0];
            string domainPart = parts[1];

            if (localPart.Length <= 2)
                return $"{localPart}**@{domainPart}"; // 本地部分太短，至少保留本地部分 + **

            // 保留本地部分的第一个和最后一个字符，中间用星号代替
            // 也可以根据需要调整保留的字符数，例如保留前两个和后一个
            return $"{localPart[0]}**{localPart[localPart.Length - 1]}@{domainPart}";

            // 另一种常见策略：保留前面几个字符，后面全部掩码到@
            // const int charsToKeep = 3; // 保留前3个字符
            // if (localPart.Length <= charsToKeep) return $"{localPart}**@{domainPart}";
            // return $"{localPart.Substring(0, charsToKeep)}**@{domainPart}";

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

            // 保留前3位和后4位，中间用4个星号代替
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

            string alphaNumericOnly = new string(cardNum.Where(c => char.IsLetterOrDigit(c) || c == 'x' || c == 'X').ToArray()); // 仅保留字母和数字

            if (alphaNumericOnly.Length < 10) // 至少保留前6后4，所以需要至少10位
                return cardNum; // 号码过短，不处理

            // 保留前6位和后4位，中间用星号代替
            int charsToMask = alphaNumericOnly.Length - 10;
            if (charsToMask <= 0) return alphaNumericOnly; // 不足掩码长度

            return $"{alphaNumericOnly.Substring(0, 6)}{new string('*', charsToMask)}{alphaNumericOnly.Substring(alphaNumericOnly.Length - 4)}";
        }

        /// <summary>
        /// 掩码银行卡号
        /// 例如: 6228481234567890 -> 6228 **** **** 7890 (或者 6228********7890)
        /// 这里采取保留前4位和后4位，中间用星号代替的策略
        /// </summary>
        /// <param name="accountNumber">原始银行卡号</param>
        /// <returns>掩码后的银行卡号</returns>
        public static string MaskBankAccountNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return accountNumber;

            string digitsOnly = new string(accountNumber.Where(char.IsDigit).ToArray()); // 仅保留数字

            if (digitsOnly.Length < 10) // 至少保留前4后4，需要至少8位，但银行卡一般更长，用10位作为最低阈值
                return accountNumber; // 号码过短，不处理

            // 保留前4位和后4位，中间用星号代替
            int charsToMask = digitsOnly.Length - 8;
            if (charsToMask <= 0) return digitsOnly; // 不足掩码长度

            // 可以选择是否在掩码部分按4位分组加空格，这里先用连续星号
            return $"{digitsOnly.Substring(0, 4)}{new string('*', charsToMask)}{digitsOnly.Substring(digitsOnly.Length - 4)}";

            // 如果需要按4位分组掩码，可以更复杂一些：
            // StringBuilder masked = new StringBuilder();
            // masked.Append(digitsOnly.Substring(0, 4)); // 前4位
            // masked.Append(" ");
            //
            // int maskedBlocks = charsToMask / 4;
            // int remainingMaskChars = charsToMask % 4;
            //
            // for(int i = 0; i < maskedBlocks; i++)
            // {
            //     masked.Append("****");
            //     if (i < maskedBlocks -1 || remainingMaskChars > 0) masked.Append(" ");
            // }
            // if(remainingMaskChars > 0)
            // {
            //     masked.Append(new string('*', remainingMaskChars));
            //     masked.Append(" ");
            // }
            // masked.Append(digitsOnly.Substring(digitsOnly.Length - 4)); // 后4位
            //
            // return masked.ToString().TrimEnd(); // TrimEnd to remove potential trailing space

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

            string digitsOnly = new string(accountNumber.Where(char.IsLetterOrDigit).ToArray()); // 根据实际账号格式保留数字或字母数字

            if (digitsOnly.Length < 8) // 需要足够长进行掩码
                return accountNumber; // 号码过短，不处理

            // 保留前4位和后4位，中间用星号代替 (可根据实际账号格式调整)
            int charsToMask = digitsOnly.Length - 8;
            if (charsToMask <= 0) return digitsOnly;

            return $"{digitsOnly.Substring(0, 4)}{new string('*', charsToMask)}{digitsOnly.Substring(digitsOnly.Length - 4)}";
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

            if (alphaNumericOnly.Length < 6) // 需要足够长进行掩码，例如前2后4
                return passportNumber; // 号码过短，不处理

            // 保留前2位和后4位，中间用星号代替 (可根据实际格式调整)
            int charsToKeepStart = 2;
            int charsToKeepEnd = 4;
            if (alphaNumericOnly.Length <= charsToKeepStart + charsToKeepEnd) return alphaNumericOnly;

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

            if (alphaNumericOnly.Length < 10) // 假设需要类似身份证的长度进行掩码
                return licenseNumber; // 号码过短，不处理

            // 保留前6位和后4位，中间用星号代替 (可根据实际格式调整)
            int charsToKeepStart = 6;
            int charsToKeepEnd = 4;
            if (alphaNumericOnly.Length <= charsToKeepStart + charsToKeepEnd) return alphaNumericOnly;

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

            if (value.Length <= 3) // 过短，不进行掩码
                return value;

            // 保留前1位和后1位，中间用星号代替
            int charsToKeepStart = 1;
            int charsToKeepEnd = 1;
            if (value.Length <= charsToKeepStart + charsToKeepEnd) return value;

            int charsToMask = value.Length - (charsToKeepStart + charsToKeepEnd);

            return $"{value.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{value.Substring(value.Length - charsToKeepEnd)}";
        }

        /// <summary>
        /// 掩码办公电话号码 (处理有区号和无区号的情况，掩码号码中间部分)
        /// 注意: 电话号码格式复杂，此为简化处理
        /// </summary>
        /// <param name="officeTel">原始办公电话号码</param>
        /// <returns>掩码后的电话号码</returns>
        public static string MaskOfficeTelNum(string officeTel)
        {
            if (string.IsNullOrEmpty(officeTel))
                return officeTel;

            // 尝试移除常见分隔符和空格，但保留可能的区号括号
            string cleaned = officeTel.Replace("-", "").Replace(" ", "");

            // 简单策略：如果长度大于等于8，保留前4后4，掩码中间
            if (cleaned.Length >= 8)
            {
                int charsToKeepStart = 4;
                int charsToKeepEnd = 4;
                if (cleaned.Length <= charsToKeepStart + charsToKeepEnd) return officeTel;

                int charsToMask = cleaned.Length - (charsToKeepStart + charsToKeepEnd);
                return $"{cleaned.Substring(0, charsToKeepStart)}{new string('*', charsToMask)}{cleaned.Substring(cleaned.Length - charsToKeepEnd)}";
            }

            // 对于其他格式或较短的号码，不做掩码
            return officeTel;
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

            if (address.Length <= charsToKeepFromStart) // 地址不够长，不掩码
                return address;

            // 保留前面指定数量的字符，后面用星号代替
            int charsToMask = address.Length - charsToKeepFromStart;
            return $"{address.Substring(0, charsToKeepFromStart)}{new string('*', charsToMask)}";
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

        // 可以根据需要添加其他类型的掩码方法
    }
}
