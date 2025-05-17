namespace NPlatform.Infrastructure
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    #region DES

    /// <summary>
    /// DES加密，高速可逆
    /// </summary>
    public class DES
    {
        // Key为64位（8个字符）
        private static string ms_IV = "asdfghjk";

        private static string ms_Key = "asdfghjk";

        /// <summary>
        /// 私密
        /// </summary>
        protected static string Ms_IV
        {
            set
            {
                ms_IV = value;
            }
        }

        /// <summary>
        /// 公密
        /// </summary>
        protected static string Ms_Key
        {
            set
            {
                ms_Key = value;
            }
        }

        private static byte[] IV_64
        {
            get
            {
                return new byte[] { 0x37, 0x67, 0xf6, 0x4f, 0x24, 0x63, 0xa7, 3 };
            }
        }

        private static byte[] KEY_64
        {
            get
            {
                return new byte[] { 0x2a, 0x10, 0x5d, 0x9c, 0x4e, 4, 0xda, 0x20 };
            }
        }

        /// <summary>
        /// 内置固定Key解密
        /// </summary>
        public static string DESDecrypt(string strOutput)
        {
            if (strOutput != string.Empty)
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(strOutput));
                CryptoStream stream2 = new CryptoStream(
                    stream,
                    provider.CreateDecryptor(KEY_64, IV_64),
                    CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(stream2);
                return reader.ReadToEnd();
            }

            return string.Empty;
        }

        /// <summary>
        /// 使用8位长度的字符Key解密
        /// </summary>
        public static string DESDecryptByKey(string decryptString, string key)
        {
            if (decryptString != string.Empty)
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = Encoding.ASCII.GetBytes(key);
                provider.IV = Encoding.ASCII.GetBytes(key);
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(decryptString));
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(stream2);
                return reader.ReadToEnd();
            }

            return string.Empty;
        }

        /// <summary>
        /// 内置固定Key加密
        /// </summary>
        public static string DESEncrypt(string strInput)
        {
            if (strInput != string.Empty)
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(
                    stream,
                    provider.CreateEncryptor(KEY_64, IV_64),
                    CryptoStreamMode.Write);
                StreamWriter writer = new StreamWriter(stream2);
                writer.Write(strInput);
                writer.Flush();
                stream2.FlushFinalBlock();
                stream.Flush();
                return Convert.ToBase64String(stream.GetBuffer(), 0, int.Parse(stream.Length.ToString()));
            }

            return string.Empty;
        }

        /// <summary>
        /// 使用8位长度的字符Key加密
        /// </summary>
        public static string DESEncryptByKey(string ecryptString, string key)
        {
            if (ecryptString != string.Empty)
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = Encoding.ASCII.GetBytes(key);
                provider.IV = Encoding.ASCII.GetBytes(key);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
                StreamWriter writer = new StreamWriter(stream2);
                writer.Write(ecryptString);
                writer.Flush();
                stream2.FlushFinalBlock();
                stream.Flush();
                return Convert.ToBase64String(stream.GetBuffer(), 0, int.Parse(stream.Length.ToString()));
            }

            return string.Empty;
        }
    }

    #endregion

    #region MD5

    /// <summary>
    /// 不可逆
    /// </summary>
    public class HashCryptoHelper
    {
        /// <summary>
        /// MD5 32位
        /// </summary>
        public static string EncodeMd5(string s)
        {
            using (MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.Default.GetBytes(s);
                return BitConverter.ToString(provider.ComputeHash(bytes)).Replace("-", string.Empty);
            }
        }

        /// <summary>
        /// MD5 16位
        /// </summary>
        /// <param name="convertString"></param>
        /// <returns></returns>
        public static string GetMd5Str(string convertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(convertString)), 4, 8);
            t2 = t2.Replace("-", string.Empty);
            return t2.ToLower();
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string GetSHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();

            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result); // 返回长度为44字节的字符串
        }
    }

    #endregion

    #region AES


    /// <summary>
    /// AES加密解密 (Modernized using System.Security.Cryptography.Aes)
    /// </summary>
    public class AES
    {
        /// <summary>
        /// 默认密钥向量 (Hardcoded IV - required for compatibility with original encryption)
        /// This IV is 16 bytes, which is the standard block size for AES (128 bits).
        /// </summary>
        private static readonly byte[] keys = // Made readonly as it's a constant IV
            {
            0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F
        };

        // AES uses a block size of 128 bits (16 bytes)
        // This matches the length of the 'keys' IV.
        private const int BlockSize = 16;

        // The original code derives a 32-byte key (256 bits)
        // The GetSubString and PadRight logic targets a 32-character string
        private const int KeyStringLength = 32;
        private const int KeySizeInBytes = 32; // 32 characters UTF8 encoded (mostly ASCII) is 32 bytes


        /// <summary>
        /// Helper method to replicate the original key derivation logic.
        /// WARNING: This uses the original GetSubString method and potentially
        /// non-standard padding with spaces. It is implemented here ONLY for
        /// compatibility with the original encryption process.
        /// </summary>
        private static byte[] DeriveKeyBytes(string keyString)
        {
            // Handle null/empty input for keyString gracefully, matching original behavior
            if (keyString == null)
            {
                keyString = ""; // GetSubString handles "" input
            }

            // Replicate original steps:
            // 1. Apply GetSubString logic (truncates string based on Encoding.Default byte length, converts back)
            string processedKeyString = GetSubString(keyString, KeyStringLength, string.Empty);

            // 2. Pad the resulting string with spaces to ensure it's 32 characters long
            processedKeyString = processedKeyString.PadRight(KeyStringLength, ' ');

            // 3. Convert the 32-character string to bytes using UTF8 encoding
            // Assuming the 32 characters are mostly ASCII or single-byte UTF8 characters,
            // this results in a 32-byte array (AES-256 key size).
            return Encoding.UTF8.GetBytes(processedKeyString);
        }


        /// <summary>
        /// AES解密字符串 (Modernized)
        /// </summary>
        /// <param name="decryptString">待解密的Base64字符串</param>
        /// <param name="decryptKey">解密密钥字符串 (used to derive the actual 32-byte key)</param>
        /// <returns>解密成功返回解密后的字符串, 失败返回string.Empty</returns>
        public static string Decode(string decryptString, string decryptKey = "NPla*t.f!or&m")
        {
            // Handle null or empty input string gracefully
            if (string.IsNullOrEmpty(decryptString))
            {
                return string.Empty;
            }

            try
            {
                // 1. Decode the Base64 string to bytes
                byte[] ciphertextBytes = Convert.FromBase64String(decryptString);

                // Check if the decoded data is empty - nothing to decrypt
                if (ciphertextBytes.Length == 0)
                {
                    return string.Empty;
                }

                // 2. Derive the key bytes using the same logic as the original encryption
                byte[] keyBytes = DeriveKeyBytes(decryptKey);

                // 3. Perform the decryption using the modern Aes class
                // Use 'using' statement to ensure the Aes object is correctly disposed
                using (Aes aes = Aes.Create())
                {
                    // Set standard AES properties. These should match the encryption side.
                    // RijndaelManaged defaults were CBC and PKCS7, which are also defaults for Aes.
                    aes.Mode = CipherMode.CBC; // Cipher Block Chaining mode
                    aes.Padding = PaddingMode.PKCS7; // Standard padding scheme

                    // Set the key and IV.
                    // IMPORTANT: The original code uses a HARDCODED static IV ('keys').
                    // It does NOT expect the IV to be part of the ciphertext.
                    aes.Key = keyBytes;
                    aes.IV = keys; // Use the hardcoded static IV

                    // Ensure KeySize property reflects the actual key length (256 bits for 32 bytes)
                    aes.KeySize = keyBytes.Length * 8; // Should be 256 if keyBytes is 32 bytes


                    // Create a decryptor
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        // Use a MemoryStream to hold the ciphertext bytes and a CryptoStream
                        // to perform the decryption on the stream.
                        using (MemoryStream msDecrypt = new MemoryStream(ciphertextBytes))
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8)) // Read decrypted bytes as UTF8 string
                        {
                            // Read the decrypted bytes from the stream to get the plaintext
                            string plaintext = srDecrypt.ReadToEnd();
                            return plaintext;
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                // Catch specific errors related to crypto operations (e.g., invalid padding, wrong key/IV, invalid ciphertext length)
                // Returning empty string as per original function's error handling style.
                return string.Empty;
            }
            catch (FormatException)
            {
                // Catch errors related to invalid Base64 format of the input string
                return string.Empty;
            }
            catch (Exception)
            {
                // Catch any other unexpected errors during the process
                // Returning empty string as per original function's error handling style.
                return string.Empty;
            }
        }

        /// <summary>
        /// AES加密字符串 (Modernized)
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥字符串 (used to derive the actual 32-byte key)</param>
        /// <returns>加密成功返回加密后的Base64字符串, 失败返回源串</returns>
        public static string Encode(string encryptString, string encryptKey = "NPla*t.f!or&m")
        {
            // Handle null input string gracefully - original code would likely throw on GetBytes
            if (encryptString == null)
            {
                return null; // Or string.Empty depending on desired behavior, but original returns source on error.
            }

            try
            {
                // 1. Get plaintext bytes using UTF8 encoding
                byte[] inputData = Encoding.UTF8.GetBytes(encryptString);

                // 2. Derive the key bytes using the same logic as the original
                byte[] keyBytes = DeriveKeyBytes(encryptKey);

                // 3. Perform the encryption using the modern Aes class
                // Use 'using' statement to ensure the Aes object is correctly disposed
                using (Aes aes = Aes.Create())
                {
                    // Set standard AES properties (CBC, PKCS7). Match decryption side.
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Set the key and IV.
                    // IMPORTANT: The original code uses a HARDCODED static IV ('keys').
                    // It does NOT prepend the IV to the ciphertext in the output.
                    aes.Key = keyBytes;
                    aes.IV = keys; // Use the hardcoded static IV

                    // Ensure KeySize is set correctly (256 bits for a 32-byte key)
                    aes.KeySize = keyBytes.Length * 8; // Should be 256

                    // Create an encryptor
                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        // Use a MemoryStream to capture the encrypted bytes and a CryptoStream
                        // to perform the encryption on the stream.
                        using (MemoryStream msEncrypt = new MemoryStream())
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            // Write the plaintext bytes to the crypto stream
                            csEncrypt.Write(inputData, 0, inputData.Length);

                            // Finalize the encryption (handles padding and writes final block)
                            csEncrypt.FlushFinalBlock();

                            // Get the encrypted bytes from the memory stream
                            byte[] encryptedData = msEncrypt.ToArray();

                            // 4. Convert the encrypted bytes to a Base64 string
                            return Convert.ToBase64String(encryptedData);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // On any error during encryption, return the original string as per the original function's behavior
                return encryptString;
            }
        }


        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// (This method is kept AS-IS because the key derivation depends on it.
        /// WARNING: Uses Encoding.Default which is system-dependent.)
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="pLength">指定长度 (Note: Used as byte length reference with Encoding.Default in original logic)</param>
        /// <param name="pTailString">用于替换的字符串 (Original code passes string.Empty for key derivation)</param>
        /// <returns>截取或原字符串</returns>
        public static string GetSubString(string p_SrcString, int pLength, string pTailString)
        {
            // Handle null or empty input string gracefully
            if (string.IsNullOrEmpty(p_SrcString))
            {
                return string.Empty;
            }

            string myResult = p_SrcString;
            // Note: Original code has pLength >= 0 check, but int is always >= 0 technically.
            // Let's keep the logic flow but check if pLength is positive for the truncation logic.
            if (pLength > 0) // Ensure pLength is positive for substring logic
            {
                // WARNING: Encoding.Default depends on the system's current ANSI code page.
                // This makes key derivation system-dependent!
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                // Check if the byte length exceeds the target length
                if (bsSrcString.Length > pLength)
                {
                    int nRealLength = pLength;
                    // Array to track multi-byte character starts within the pLength boundary
                    int[] anResultFlag = new int[pLength];
                    byte[] bsResult = null;

                    // Logic to adjust length if truncation splits a multi-byte character
                    int nFlag = 0;
                    for (int i = 0; i < pLength; i++)
                    {
                        if (bsSrcString[i] > 127) // Check for non-ASCII byte value
                        {
                            // This logic is specific to encodings where multi-byte
                            // characters are represented by bytes > 127.
                            // For example, GBK/GB2312 uses two bytes > 127.
                            // UTF8 also uses bytes > 127, but the nFlag logic (== 3)
                            // looks specific to DBCS (Double-Byte Character Sets).
                            nFlag++;
                            if (nFlag == 3) nFlag = 1; // This '3' seems specific to an expected pattern
                        }
                        else nFlag = 0;

                        anResultFlag[i] = nFlag;
                    }

                    // This adjustment checks the last byte processed for multi-byte character boundary.
                    // anResultFlag[pLength - 1] == 1 likely means the last byte was the *first* byte
                    // of a two-byte sequence in a DBCS encoding, needing the next byte.
                    if ((bsSrcString[pLength - 1] > 127) && (anResultFlag[pLength - 1] == 1))
                        nRealLength = pLength + 1;

                    // Copy the truncated bytes
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, bsResult, nRealLength);

                    // Convert bytes back to string using Encoding.Default
                    myResult = Encoding.Default.GetString(bsResult);

                    // Append the tail string (which is expected to be empty for key derivation)
                    myResult = myResult + pTailString;
                }
                // If bsSrcString.Length <= pLength, myResult remains p_SrcString, no truncation/padding happens *within this method*.
                // Padding happens later in DeriveKeyBytes.
            }
            // If pLength is not > 0, myResult remains p_SrcString

            return myResult;
        }
    }
    #endregion

    #region RSA

    /// <summary>
    /// RSA 非对称加密算法
    /// </summary>
    public class RSA
    {
        private RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider();

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="s"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] s, string privateKey)
        {
            this._rsa.FromXmlString(privateKey);
            return this._rsa.Decrypt(s, false);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="s"></param>
        /// <param name="privateKey"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public string Decrypt(string s, string privateKey, string encode)
        {
            byte[] bytes = this.Decrypt(Convert.FromBase64String(s), privateKey);
            return Encoding.GetEncoding(encode).GetString(bytes);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="s"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] s, string publicKey)
        {
            this._rsa.FromXmlString(publicKey);
            return this._rsa.Encrypt(s, false);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="s"></param>
        /// <param name="publicKey"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public string Encrypt(string s, string publicKey, string encode)
        {
            return Convert.ToBase64String(this.Encrypt(Encoding.GetEncoding(encode).GetBytes(s), publicKey));
        }

        /// <summary>
        /// 同一个对象产生的私密用来解密
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            return this._rsa.ToXmlString(true);
        }

        /// <summary>
        /// 同一个对象产生的公密用来加密
        /// </summary>
        public string GetPublicKey()
        {
            return this._rsa.ToXmlString(false);
        }
    }

    #endregion
}