/********************************************************************************

** auth�� Dongliang YI

** desc�� �������������
*********************************************************************************/

using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPlatform.Infrastructure
{
    /// <summary>
    /// ���� BaseController
    /// </summary>
    public class ReportHelper
    {
        /// <summary>
        /// �Ѷ������л�Ϊ������Ҫ�������Դ��ʽ��
        /// </summary>
        /// <param name="t">��Ҫת���Ķ���</param>
        /// <param name="descriptionAsHeard">�Ƿ�ʹ���ֶ�Description������Ϊ��ͷ</param>
        public static string ObjectToTabString<T>(T t, bool descriptionAsHeard = false)
        {
            try
            {
                System.Reflection.PropertyInfo[] properties = typeof(T)
                        .GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                if (properties.Length <= 0)
                {
                    return string.Empty;
                }

                StringBuilder str = new StringBuilder();
                for (int i = 0; i < properties.Length; i++)
                {
                    var item = properties[i];
                    var col = item.Name;
                    if (descriptionAsHeard)
                    {
                        var desc = item.GetDescription();
                        if (!desc.IsNullOrEmpty())
                        {
                            col = desc;
                        }
                    }

                    str.AppendFormat("{0},", col);
                }

                str.AppendLine();

                if (t != null)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var item = properties[i];
                        object value = item.GetValue(t, null);
                        var val = value == null ? " " : value.ToString().Trim();

                        val = ReplaceSepcialStr(val);
                        if (val.IsNullOrEmpty())
                        {
                            val = " ";
                        }

                        str.AppendFormat("{0},", val);
                    }

                    str.AppendLine();
                }

                return str.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// �����ݼ������л�Ϊ������Ҫ�������Դ��ʽ��
        /// </summary>
        /// <param name="datas">��Ҫת���ļ���</param>
        /// <param name="descriptionAsHeard">�Ƿ�ʹ���ֶ�Description������Ϊ��ͷ</param>
        public static string EnumerableToTabString<T>(IEnumerable<T> datas, bool descriptionAsHeard = false)
        {
            System.Reflection.PropertyInfo[] properties = (datas?.FirstOrDefault()?.GetType() ?? typeof(T))
                    .GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return string.Empty;
            }

            StringBuilder str = new StringBuilder();
            for (int i = 0; i < properties.Length; i++)
            {
                var item = properties[i];

                var col = item.Name;
                if (descriptionAsHeard)
                {
                    var desc = item.GetDescription();
                    if (!desc.IsNullOrEmpty())
                    {
                        col = desc;
                    }
                }

                str.AppendFormat("{0},", col);
            }

            str.AppendLine();

            if (datas == null)
            {
                return str.ToString();
            }

            foreach (var item in datas)
            {
                System.Reflection.PropertyInfo[] itemPr = (item?.GetType() ?? typeof(T))
                   .GetProperties(System.Reflection.BindingFlags.Instance |
                                  System.Reflection.BindingFlags.Public);

                if (item == null || itemPr.Length <= 0)
                {
                    continue;
                }

                foreach (var it in itemPr)
                {
                    object value = it.GetValue(item, null);
                    var val = value == null ? " " : value.ToString().Trim();

                    val = ReplaceSepcialStr(val);
                    if (val.IsNullOrEmpty())
                    {
                        val = " ";
                    }

                    str.AppendFormat("{0},", val);
                }

                str.AppendLine();
            }

            return str.ToString();
        }

        /// <summary>
        /// �滻���С��س����Ʊ�������ţ������ֻᵼ�·������쳣���ַ���
        ///  oldStr.Replace("\r\n", "&at;").Replace("\t", " ").Replace("\n", "&at;").Replace("\r", "&at;").Replace(",", " ");
        /// </summary>
        /// <param name="oldStr">�滻ǰ���ַ���</param>
        /// <returns>�滻����ַ���</returns>
        public static string ReplaceSepcialStr(string oldStr)
        {
            if (!string.IsNullOrEmpty(oldStr))
            {
                return oldStr.Replace("\r\n", "&at;").Replace("\t", " ").Replace("\n", "&at;").Replace("\r", "&at;").Replace(",", "��");
            }
            else
            {
                return " ";
            }
        }
    }
}