namespace NPlatform.Infrastructure
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    /// GZIP �ַ���ѹ�����Դ�������ѹ��Ч���á�
    /// </summary>
    public static class GZipStr
    {
        /// <summary>
        /// ѹ���������ļ�
        /// </summary>
        /// <param name="data">����</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
            byte[] buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;
        }

        /// <summary>
        /// ѹ���ַ���
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns></returns>
        public static string CompressString2String(string str)
        {
            return Convert.ToBase64String(
                Compress(Convert.FromBase64String(Convert.ToBase64String(Encoding.Default.GetBytes(str)))));
        }

        /// <summary>
        /// ��ѹ���������ļ�
        /// </summary>
        /// <param name="data">����</param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
            MemoryStream msreader = new MemoryStream();
            byte[] buffer = new byte[0x1000];
            while (true)
            {
                int reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }

                msreader.Write(buffer, 0, reader);
            }

            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();
            return buffer;
        }

        /// <summary>
        /// ��ѹ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns></returns>
        public static string DecompressString2String(string str)
        {
            return Encoding.Default.GetString(Decompress(Convert.FromBase64String(str)));
        }
    }
}