/***********************************************************
**��Ŀ����:	                                     
**��    ��:	                                 				   
**��������:	                                       					   
**��    ��: 	�׶���                                         			   
**�� �� ��:	1.0                                             			   
**�������ڣ�2015/3/21 
**�޸���ʷ��
************************************************************/

namespace NPlatform.Infrastructure
{
    using System;
    using System.Text;

    /// <summary>
    /// ��������
    /// </summary>
    public static class EncoderHelper
    {
        /// <summary>
        /// ��Base64�ַ�������Ϊ��ͨ�ַ���
        /// </summary>
        /// <param name="str">Ҫ������ַ���</param>
        public static string Base64Decode(string str)
        {
            byte[] barray;
            barray = Convert.FromBase64String(str);
            return Encoding.Default.GetString(barray);
        }

        /// <summary>
        /// ���ַ�������ΪBase64�ַ���
        /// </summary>
        /// <param name="str">Ҫ������ַ���</param>
        public static string Base64Encode(string str)
        {
            byte[] barray;
            barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }

        /// <summary>
        /// ģ��Javascript��escape�Ĵ�����ʵ��
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Escape(string str)
        {
            if (str == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            int len = str.Length;

            for (int i = 0; i != len; i++)
            {
                char c = str[i];

                // everything other than the optionally escaped chars _must_ be escaped 
                if (char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '/' || c == '\\' || c == '.')
                    sb.Append(c);
                else
                    sb.Append(Uri.HexEscape(c));
            }

            return sb.ToString();
        }

        /// <summary>
        /// ģ��Javascript��unescape�Ľ���ʵ��
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnEscape(string str)
        {
            if (str == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            int len = str.Length;
            int i = 0;
            while (i != len)
            {
                if (Uri.IsHexEncoding(str, i))
                    sb.Append(Uri.HexUnescape(str, ref i));
                else
                    sb.Append(str[i++]);
            }

            return sb.ToString();
        }
    }
}