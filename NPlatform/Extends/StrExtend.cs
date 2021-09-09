/***********************************************************
**��Ŀ����:	                                     
**��    ��:	                                 				   
**��������:	                                       					   
**��    ��: 	�׶���                                         			   
**�� �� ��:	1.0                                             			   
**�������ڣ�2015/3/21 
**�޸���ʷ��
************************************************************/

namespace NPlatform
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// �ַ���������
    /// </summary>
    public static class StrExtend
    {
        /// <summary>
        /// ���ݰ��������ַ����·ݵ�����(�ɸ���Ϊĳ������)
        /// </summary>	
        public static string[] Monthes
        {
            get
            {
                return new string[]
                           {
                               "January", "February", "March", "April", "May", "June", "July", "August", "September",
                               "October", "November", "December"
                           };
            }
        }

        /// <summary>
        /// �����ַ���
        /// </summary>
        public static string CleanInput(this string strIn)
        {
            return Regex.Replace(strIn.Trim(), @"[^\w\.@-]", "");
        }

        /// <summary>
        /// ��������ַ����еĻس������з�
        /// </summary>
        /// <param name="str">Ҫ������ַ���</param>
        /// <returns>����󷵻ص��ַ���</returns>
        public static string ClearBR(this string str)
        {
            Regex r = null;
            Match m = null;
            r = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
                str = str.Replace(m.Groups[0].ToString(), "");
            return str;
        }

        /// <summary>
        /// ���������ַ��ĳ���
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns>���������ַ��ĳ���</returns>
        public static int CnLength(this string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        /// ��ȡ�����е��ַ���
        /// </summary>
        /// <param name="strInput">����</param>
        /// <param name="intlen">ȡ������</param>
        /// <param name="flg">��β�������ַ���</param>
        /// <returns>�����е��ַ���</returns>
        public static string CutStr(this string strInput, int intlen, string flg) //��ȡ�ַ���
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int intLength = 0;
            string strString = "";
            byte[] s = ascii.GetBytes(strInput);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    intLength += 2;
                }
                else
                {
                    intLength += 1;
                }

                try
                {
                    strString += strInput.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (intLength > intlen)
                {
                    break;
                }
            }

            byte[] mybyte = Encoding.Default.GetBytes(strInput);
            if (mybyte.Length > intlen)
            {
                strString += flg;
            } //����ع�����ϰ���ַ���

            return strString;
        }

        /// <summary>
        /// ���ַ�����ָ��λ�ý�ȡָ�����ȵ����ַ���
        /// </summary>
        /// <param name="str">ԭ�ַ���</param>
        /// <param name="startIndex">���ַ�������ʼλ��</param>
        /// <param name="length">���ַ����ĳ���</param>
        /// <returns>���ַ���</returns>
        public static string CutString(this string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else startIndex = startIndex - length;
                }

                if (startIndex > str.Length) return "";
            }
            else
            {
                if (length < 0) return "";
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else return "";
                }
            }

            if (str.Length - startIndex < length) length = str.Length - startIndex;
            try
            {
                return str.Substring(startIndex, length);
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// ���ַ�����ָ��λ�ÿ�ʼ��ȡ���ַ�����β���˷���
        /// </summary>
        /// <param name="str">ԭ�ַ���</param>
        /// <param name="startIndex">���ַ�������ʼλ��</param>
        /// <returns>���ַ���</returns>
        public static string CutString(this string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        /// <summary>
        /// ���Assembly��Ʒ��Ȩ
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyCopyright()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(myAssembly.Location);
            return myFileVersion.LegalCopyright;
        }

        /// <summary>
        /// ���Assembly��Ʒ����
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyProductName()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(myAssembly.Location);
            return myFileVersion.ProductName;
        }

        /// <summary>
        /// ���Assembly�汾��
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(myAssembly.Location);
            return string.Format(
                "{0}.{1}.{2}",
                myFileVersion.FileMajorPart,
                myFileVersion.FileMinorPart,
                myFileVersion.FileBuildPart);
        }

        /// <summary>
        /// ȡ�ļ���չ��
        /// </summary>
        /// <param name="filename">�ļ�URL</param>
        /// <returns>�ļ���չ��</returns>
        public static string GetFileExtends(this string filename)
        {
            string ext = null;
            if (filename.IndexOf('.') > 0)
            {
                string[] fs = filename.Split('.');
                ext = fs[fs.Length - 1];
            }

            return ext;
        }

        /// <summary>
        /// ��ȡҳ����������� GetHref(HtmlCode);
        /// </summary>
        /// <param name="HtmlCode"></param>
        /// <returns></returns>
        public static string GetHref(this string HtmlCode)
        {
            string MatchVale = "";
            string Reg = @"(h|H)(r|R)(e|E)(f|F) *= *('|"")?((\w|\\|\/|\.|:|-|_)+)('|""| *|>)?";
            foreach (Match m in Regex.Matches(HtmlCode, Reg))
            {
                MatchVale += (m.Value).ToLower().Replace("href=", "").Trim() + "||";
            }

            return MatchVale;
        }

        /// <summary>
        /// ƥ��&lt;img src="" />�е�ͼƬ·��ʵ������
        /// </summary>
        /// <param name="ImgString">Html�ַ���</param>
        /// <param name="imgHttp">ǰ��URL</param>
        /// <returns></returns>
        public static string GetImg(this string ImgString, string imgHttp)
        {
            string MatchVale = "";
            string Reg = @"src=.+\.(bmp|jpg|gif|png|)";
            foreach (Match m in Regex.Matches(ImgString.ToLower(), Reg))
            {
                MatchVale += (m.Value).ToLower().Trim().Replace("src=", "");
            }

            return (imgHttp + MatchVale);
        }

        /// <summary>
        /// ƥ��ҳ���ͼƬ��ַ GetImgSrc(HtmlCode,"http://www.baidu.com/");������:&lt;img src="bb/x.gif">��Ҫ����http://www.baidu.com/,������http��Ϣʱ,�����Ϊ��
        /// </summary>
        /// <param name="HtmlCode"></param>
        /// <param name="imgHttp">Ҫ�����http://·����Ϣ</param>
        /// <returns></returns>
        public static string GetImgSrc(this string HtmlCode, string imgHttp)
        {
            string MatchVale = "";
            string Reg = @"<img.+?>";
            foreach (Match m in Regex.Matches(HtmlCode, Reg))
            {
                MatchVale += GetImg((m.Value).ToLower().Trim(), imgHttp) + "|";
            }

            return MatchVale.Replace("\"", "");
        }

        /// <summary>
        /// �ж�ָ���ַ�����ָ���ַ��������е�λ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�ַ�����ָ���ַ��������е�λ��, �粻�����򷵻�-1</returns>
        public static int GetInArrayID(this string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower()) return i;
                }
                else
                {
                    if (strSearch == stringArray[i]) return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// �ж�ָ���ַ�����ָ���ַ��������е�λ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <returns>�ַ�����ָ���ַ��������е�λ��, �粻�����򷵻�-1</returns>		
        public static int GetInArrayID(this string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        /// <summary>
        /// ִ��������ȡ��ֵ GetRegValue("<title>.+?</title>",HtmlCode)
        /// </summary>
        /// <param name="RegexString">������ʽ</param>
        /// <param name="HtmlCode">HTML����</param>
        /// <returns></returns>
        public static string GetRegexValue(this string HtmlCode, string RegexString)
        {
            string MatchVale = "";
            Regex r = new Regex(RegexString);
            Match m = r.Match(HtmlCode);
            if (m.Success)
            {
                MatchVale = m.Value;
            }

            return MatchVale;
        }

        /// <summary>
        /// ��ȡָ�����ȵ��ַ���
        /// </summary>
        /// <param name="str">Ҫ��ȡ���ַ���</param>
        /// <param name="length">�ַ�������</param>
        /// <param name="flg">��β�������ַ���</param>
        /// <returns>ָ�����ȵ��ַ���</returns>
        public static string GetStr(this string str, int length, string flg)
        {
            int i = 0, j = 0;
            foreach (char chr in str)
            {
                if ((int)chr > 127)
                {
                    i += 2;
                }
                else
                {
                    i++;
                }

                if (i > length)
                {
                    str = str.Substring(0, j);
                    str += flg;
                    break;
                }

                j++;
            }

            return str;
        }

        /// <summary>
        /// �ַ���ȡ
        /// </summary>
        public static string GetSubString(object obj, int pLength, string pTailString)
        {
            if (obj == null) return "";
            return obj.ToString().GetSubString(pLength, pTailString);
        }

        /// <summary>
        /// �ַ�������ٹ�ָ�������򽫳����Ĳ�����ָ���ַ�������
        /// </summary>
        /// <param name="p_SrcString">Ҫ�����ַ���</param>
        /// <param name="pLength">ָ������</param>
        /// <param name="pTailString">�����滻���ַ���</param>
        /// <returns>��ȡ����ַ���</returns>
        public static string GetSubString(this string p_SrcString, int pLength, string pTailString)
        {
            string myResult = p_SrcString;
            if (pLength >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);
                if (bsSrcString.Length > pLength)
                {
                    int nRealLength = pLength;
                    int[] anResultFlag = new int[pLength];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = 0; i < pLength; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3) nFlag = 1;
                        }
                        else nFlag = 0;

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[pLength - 1] > 127) && (anResultFlag[pLength - 1] == 1))
                        nRealLength = pLength + 1;
                    bsResult = new byte[nRealLength];
                    Array.Copy(bsSrcString, bsResult, nRealLength);
                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + pTailString;
                }
            }

            return myResult;
        }

        /// <summary>
        /// ����URL�н�β���ļ���
        /// </summary>		
        public static string GetUrlFileName(this string url)
        {
            if (url == null) return "";
            string[] strs1 = url.Split(new char[] { '/' });
            return strs1[strs1.Length - 1].Split(new char[] { '?' })[0];
        }

        /// <summary>
        /// �����ڷ�
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsDateTime(this string input)
        {
            //string pet = @"^(?:(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(\/|-|\.)(?:0?2\1(?:29))$)|(?:(?:1[6-9]|[2-9]\d)?\d{2})(\/|-|\.)(?:(?:(?:0?[13578]|1[02])\2(?:31))|(?:(?:0?[1,3-9]|1[0-2])\2(29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\2(?:0?[1-9]|1\d|2[0-8]))$";
            string pet =
                @"^(?=\d)(?:(?!(?:1582(?:\.|-|\/)10(?:\.|-|\/)(?:0?[5-9]|1[0-4]))|(?:1752(?:\.|-|\/)0?9(?:\.|-|\/)(?:0?[3-9]|1[0-3])))(?=(?:(?!000[04]|(?:(?:1[^0-6]|[2468][^048]|[3579][^26])00))(?:(?:\d\d)(?:[02468][048]|[13579][26]))\D0?2\D29)|(?:\d{4}\D(?!(?:0?[2469]|11)\D31)(?!0?2(?:\.|-|\/)(?:29|30))))(\d{4})([-\/.])(0?\d|1[012])\2((?!00)[012]?\d|3[01])(?:$|(?=\x20\d)\x20))?((?:(?:0?[1-9]|1[012])(?::[0-5]\d){0,2}(?:\x20[aApP][mM]))|(?:[01]?\d|2[0-3])(?::[0-5]\d){1,2})?$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// ��Email��
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsEmail(this string input)
        {
            string pet = @"^\w+((-\w+)|(\.\w+))*\@\w+((\.|-)\w+)*\.\w+$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="strSearch">�ַ���</param>
        /// <param name="stringArray">�ַ�������</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�жϽ��</returns>
        public static bool IsInArray(this string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ַ�������</param>
        /// <returns>�жϽ��</returns>
        public static bool IsInArray(this string str, string[] stringarray)
        {
            return IsInArray(str, stringarray, false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool IsInArray(this string str, string stringarray)
        {
            return IsInArray(str, stringarray.Split(','), false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <param name="strsplit">�ָ��ַ���</param>
        /// <returns>�жϽ��</returns>
        public static bool IsInArray(this string str, string stringarray, char strsplit)
        {
            return IsInArray(str, stringarray.Split(strsplit), false);
        }

        /// <summary>
        /// �ж�ָ���ַ����Ƿ�����ָ���ַ��������е�һ��Ԫ��
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="stringarray">�ڲ��Զ��ŷָ�ʵ��ַ���</param>
        /// <param name="strsplit">�ָ��ַ���</param>
        /// <param name="caseInsensetive">�Ƿ����ִ�Сд, trueΪ������, falseΪ����</param>
        /// <returns>�жϽ��</returns>
        public static bool IsInArray(this string str, string stringarray, char strsplit, bool caseInsensetive)
        {
            return IsInArray(str, stringarray.Split(strsplit), caseInsensetive);
        }

        /// <summary>
        /// ����ָ��IP�Ƿ���ָ����IP�������޶��ķ�Χ��, IP�����ڵ�IP��ַ����ʹ��*��ʾ��IP������, ����192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool IsInIPArray(this string ip, string[] iparray)
        {
            string[] userip = ip.Split('.');
            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = iparray[ipIndex].Split('.');
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*") return true;
                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i]) r++;
                        else break;
                    }
                    else break;
                }

                if (r == 4) return true;
            }

            return false;
        }

        /// <summary>
        /// �������� ��0������
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsInt(this string input)
        {
            string pet = @"^[0-9]*[1-9][0-9]*$"; //@"^\d{1,}$"//����У�鳣��//@"^-?(0|\d+)(\.\d+)?$"//��ֵУ�鳣�� 
            return input.IsMatch(pet);
        }

        /// <summary>
        /// ��IP���ͷ�
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsIp(this string input)
        {
            string pet = @"^(([01]?[\d]{1,2})|(2[0-4][\d])|(25[0-5]))(\.(([01]?[\d]{1,2})|(2[0-4][\d])|(25[0-5]))){3}$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// У���¼����ֻ������4-20������ĸ��ͷ���ɴ����֡���_������.�����ִ�
        /// ʹ�÷�ʽ�����û����ַ�����.IsLoginName();
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsLoginName(this string input)
        {
            string pet = @"^[\@A-Za-z0-9\!\#\$\%\^\&\*\.\~]{4,22}$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// �Ƿ�ƥ��������ʽ
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern">������ʽ</param>
        /// <returns>��ȫƥ�䷵����</returns>
        public static bool IsMatch(this string str, string pattern)
        {
            if (string.IsNullOrEmpty(str)) return false;
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
            return re.IsMatch(str);
        }

        /// <summary>
        /// �ֻ��� + - 
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsMobile(this string input)
        {
            string pet = @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// �ж��ַ��Ƿ�Ϊnull����string.Empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// �жϸ������ַ���(strNumber)�Ƿ�����ֵ��
        /// </summary>
        /// <param name="strNumber">Ҫȷ�ϵ��ַ���</param>
        /// <returns>���򷵼�true �����򷵻� false</returns>
        public static bool IsNumber(this string strNumber)
        {
            string pet = @"^(\-|\+)?\d+(\.\d+)?$";
            return strNumber.IsMatch(pet);
        }

        /// <summary>
        /// У�����룺ֻ������8-20����ĸ+���֣���ĸ+�����ַ�������+�����ַ�,���߶���
        /// ʹ�÷�ʽ�����û����ַ�����.IsPassword();
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsPassword(this string input)
        {
            if (input.Length < 8 || input.Length > 20) return false;
            string pattern1 = @"^(?![a-zA-z]+$)(?!\d+$)(?![!@#$%^&*]+$)[a-zA-Z\d!@#$%^&*().]+$";
            return input.IsMatch(pattern1);
        }

        /// <summary>
        /// �����֤��
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsSSN(this string input)
        {
            string pet = @"\d{18}|\d{15}";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// �绰���� + -
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsTelepone(this string input)
        {
            string pet = @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$"; //��"^(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}$
            return input.IsMatch(pet);
        }

        /// <summary>
        /// ��Url��
        /// </summary>
        /// <param name="input">�ַ���</param>
        /// <returns>��/��</returns>
        public static bool IsUrl(this string input)
        {
            string pet = @"^http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            return input.IsMatch(pet);
        }

        /// <summary>
        /// �Ƿ�Ϊ������Guid����Guid.Empty
        /// </summary>
        /// <returns></returns>
        public static bool IsUsableGuid(this string str)
        {
            Guid guid;
            if (Guid.TryParse(str, out guid))
            {
                return guid != Guid.Empty;
            }

            return false;
        }
        /// <summary>
        /// �Ƿ�Ϊ���
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidYear(this string input)
        {
            return Regex.IsMatch(input, @"^(19\d\d)|(200\d)$");
        }

        /// <summary>
        /// �滻HTMLԴ����
        /// </summary>
        /// <param name="HtmlCode">htmlԴ����</param>
        /// <returns></returns>
        public static string RemoveHTML(this string HtmlCode)
        {
            string MatchVale = HtmlCode;
            MatchVale = new Regex("<br>", RegexOptions.IgnoreCase).Replace(MatchVale, "\n");
            foreach (Match s in Regex.Matches(HtmlCode, "<[^{><}]*>"))
            {
                MatchVale = MatchVale.Replace(s.Value, "");
            } //"(<[^{><}]*>)"//@"<[\s\S-! ]*?>"//"<.+?>"//<(.*)>.*<\/\1>|<(.*) \/>//<[^>]+>//<(.|\n)+?>

            MatchVale = new Regex("\n", RegexOptions.IgnoreCase).Replace(MatchVale, "<br>");
            return MatchVale;
        }

        /// <summary>
        /// �滻��ҳ�еĻ��к�����
        /// </summary>
        /// <param name="HtmlCode">HTMLԴ����</param>
        /// <returns></returns>
        public static string ReplaceEnter(this string HtmlCode)
        {
            string s = "";
            if (HtmlCode == null || HtmlCode == "") s = "";
            else s = HtmlCode.Replace("\"", "");
            s = s.Replace("\r\n", "");
            return s;
        }

        /// <summary>
        /// �Զ�����滻�ַ�������
        /// </summary>
        public static string ReplaceString(
            this string SourceString,
            string SearchString,
            string ReplaceString,
            bool IsCaseInsensetive)
        {
            return Regex.Replace(
                SourceString,
                Regex.Escape(SearchString),
                ReplaceString,
                IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>
        /// �ߵ��ַ�������
        /// </summary>
        /// <param name="_str">�ַ���</param>
        /// <returns>�ߵ��ַ�������</returns>
        public static string Reverse(this string _str)
        {
            if (_str.Length <= 1) return _str;
            char[] c = _str.ToCharArray();
            StringBuilder sb = new StringBuilder(c.Length);
            for (int i = c.Length - 1; i > -1; i--) sb.Append(c[i]);
            return sb.ToString();
        }

        /// <summary>
        /// ɾ���ַ���β���Ļس�/����/�ո�
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(this string str)
        {
            return str.TrimEnd(new char[] { ' ', '\r', '\n' });
        }

        /// <summary>
        /// string��ת��Ϊfloat��
        /// </summary>
        /// <param name="strValue">Ҫת�����ַ���</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>ת�����int���ͽ��</returns>
        public static float ToFloat(this object strValue, float defValue = 0)
        {
            if ((strValue == null) || (strValue.ToString().Length > 10))
                return defValue;

            string val = strValue.ToString();
            float rst = defValue;
            if (float.TryParse(val, out rst))
                return rst;
            else
                return defValue;
        }

        /// <summary>
        /// string��ת��Ϊint��
        /// </summary>
        /// <param name="strValue">Ҫת�����ַ���</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>ת�����int���ͽ��</returns>
        public static int ToInt(this object strValue, int defValue = 0)
        {
            if ((strValue == null) || (strValue.ToString() == string.Empty))
                return defValue;
            string val = strValue.ToString();
            int rst = defValue;
            if (int.TryParse(val, out rst))
                return rst;
            else
                return defValue;
        }
        /// <summary>
        /// string��ת��Ϊbool��
        /// </summary>
        /// <param name="strValue">Ҫת�����ַ���</param>
        /// <param name="defValue">ȱʡֵ</param>
        /// <returns>ת�����bool���ͽ��</returns>
        public static bool ToBool(this object strValue, bool defValue = false)
        {
            if ((strValue == null) || (strValue.ToString() == string.Empty))
                return defValue;
            string val = strValue.ToString();
            bool rst = defValue;
            if (bool.TryParse(val, out rst))
                return rst;
            else
                return defValue;
        }
        /// <summary>
        /// Int ת char  ��Сд�� a~z 
        /// </summary>
        /// <param name="acciiInt"></param>
        /// <returns></returns>
        public static string ToLowerChar(this int acciiInt)
        {
            var tmp = acciiInt % 26;
            string c = char.ConvertFromUtf32(97 + tmp);
            return c;
        }

        /// <summary>
        ///  �������Ϊ�գ��򷵻�string.Empty
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String ToStrNoNull(this object val)
        {
            return val == null ? string.Empty : val.ToString().Trim();
        }

        /// <summary>
        ///  �Ƴ��ո�����ַ�Ϊnull�򷵻�string.empty
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static String TrimNull(this string val)
        {
            return val == null ? string.Empty : val.Trim();
        }

        //0000 0000-0000 007F - 0xxxxxxx  (ascii converts to 1 octet!)
        //0000 0080-0000 07FF - 110xxxxx 10xxxxxx    ( 2 octet format)
        //0000 0800-0000 FFFF - 1110xxxx 10xxxxxx 10xxxxxx (3 octet format)
        /// <summary>
        /// �ж��ļ����Ƿ�ΪUTF8�ַ���
        /// </summary>
        /// <param name="sbInputStream">�ļ���</param>
        /// <returns>�жϽ��</returns>
        private static bool IsUTF8(this FileStream sbInputStream)
        {
            int i;
            byte cOctets; // octets to go in this UTF-8 encoded character 
            byte chr;
            bool bAllAscii = true;
            long iLen = sbInputStream.Length;
            cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                chr = (byte)sbInputStream.ReadByte();
                if ((chr & 0x80) != 0) bAllAscii = false;
                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        }
                        while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0) return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80) return false;
                    cOctets--;
                }
            }

            if (cOctets > 0) return false;
            if (bAllAscii) return false;
            return true;
        }

        //      #region ToSChinese/ToTChinese
        //      /// <summary>
        ///// ת��Ϊ��������
        ///// </summary>
        //public static string ToSChinese(this string str)
        //{
        //	return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0) ;
        //}

        ///// <summary>
        ///// ת��Ϊ��������
        ///// </summary>
        //public static string ToTChinese(this string str)
        //{
        //	return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);
        //      }
        //      #endregion
    }
}