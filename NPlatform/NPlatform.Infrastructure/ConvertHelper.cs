using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Web;
using System.Text.RegularExpressions;

namespace NPlatform.Infrastructure
{
    /// <summary>
    /// ���ʽ���� ������ZF02���ɱ���
    /// </summary>
    public enum EnumFormula
    {
        Add,//�Ӻ�
        Dec,//����
        Mul,//�˺�
        Div,//����
        Sin,//����
        Cos,//����
        Tan,//����
        ATan,//����
        Sqrt,//ƽ����
        Pow,//����
        None,//��
        Round,//����ĳ�����ְ�ָ��λ�����������֡� 
        Int,//����ʵ������������ֵ��
        Exp,//���� e �� n ���ݡ� 2.71828182���� e ����845904������Ȼ�����ĵ����� 
        Abs,//���ز����ľ���ֵ����������ֵ�ǲ���ȥ�������ź����ֵ��
        Log10,//������ 10 Ϊ�׵Ķ����� 
        Ln,//����һ��������Ȼ��������Ȼ�����Գ����� e��2.71828182845904��Ϊ�ס� 
    }

    /// <summary>
    /// FormulaDeal
    /// </summary>
    public class FormulaDeal
    {
        /// <summary>
        /// ������ʽ
        /// </summary>
        /// <param name="strExpression"></param>
        /// <returns></returns>
        private static decimal CalculateExpress(string strExpression)
        {
            string strTemp = "";
            string strTempB = "";
            string strOne = "";
            string strTwo = "";
            decimal ReplaceValue = 0;
            strExpression = strExpression.Replace(" ", string.Empty).Trim().Replace("+-", "-").Replace("--", "+").Replace("-+", "+").Replace("++", "+");
            if (strExpression.StartsWith("-"))
            {
                strExpression = "0" + strExpression;
            }

            while (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
            || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                //�жϸ��ų��ֵĴ��������ֻ��һ���Ҹ��ų����ڵ�һ��������-1��ֱ�ӷ���-1
                var temp = strExpression.Split('-');
                if (temp.Length == 2 && strExpression.IndexOf("-") == 0)
                {
                    if (temp[1].IndexOf("-") == -1 && temp[1].IndexOf("-") == -1 && temp[1].IndexOf("-") == -1 && temp[1].IndexOf("-") == -1)
                    {
                        return Convert.ToDecimal(strExpression);
                    }
                }
                if (strExpression.IndexOf("*") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("*") + 1, strExpression.Length - strExpression.IndexOf("*") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("*"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);

                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = Convert.ToDecimal(GetExpType(strOne)) * Convert.ToDecimal(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "*" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("/") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("/") + 1, strExpression.Length - strExpression.IndexOf("/") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("/"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);


                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = Convert.ToDecimal(GetExpType(strOne)) / Convert.ToDecimal(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "/" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("+") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("+") + 1, strExpression.Length - strExpression.IndexOf("+") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("+"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);

                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = Convert.ToDecimal(GetExpType(strOne)) + Convert.ToDecimal(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "+" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("-") != -1)
                {
                    if (strExpression.IndexOf("-") == 0)  // ��ʼλ���Ǹ���
                    {
                        // ��һλ��û�и��ţ�ֱ�ӷ��أ���Ϊ���㵽���ֻ����ָ��ţ���������������
                        if (strExpression.IndexOf("-", 1) != -1)
                        {
                            strTemp = strExpression.Substring(strExpression.IndexOf("-", 1) + 1, strExpression.Length - strExpression.IndexOf("-", 1) - 1);
                            strTempB = strExpression.Substring(0, strExpression.IndexOf("-", 1));
                            strOne = strTempB;// strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);

                            strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                            ReplaceValue = Convert.ToDecimal(GetExpType(strOne)) - Convert.ToDecimal(GetExpType(strTwo));
                            strExpression = strExpression.Replace(strOne + "-" + strTwo, ReplaceValue.ToString());
                        }
                        else
                        {
                            return Convert.ToDecimal(strExpression);
                        }
                    }
                    else
                    {
                        strTemp = strExpression.Substring(strExpression.IndexOf("-") + 1, strExpression.Length - strExpression.IndexOf("-") - 1);
                        strTempB = strExpression.Substring(0, strExpression.IndexOf("-"));
                        strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);


                        strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                        ReplaceValue = Convert.ToDecimal(GetExpType(strOne)) - Convert.ToDecimal(GetExpType(strTwo));
                        strExpression = strExpression.Replace(strOne + "-" + strTwo, ReplaceValue.ToString());
                    }
                }
            }
            return Convert.ToDecimal(strExpression);
        }
        /// <summary>
        /// ���㺬��ѧ�����ı��ʽ
        /// </summary>
        /// <param name="strExpression"></param>
        /// <param name="expressType"></param>
        /// <returns></returns>
        private static double CalculateExExpress(string strExpression, EnumFormula expressType)
        {
            double retValue = 0;
            switch (expressType)
            {
                case EnumFormula.Sin:
                    retValue = Math.Sin(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Cos:
                    retValue = Math.Cos(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Tan:
                    retValue = Math.Tan(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.ATan:
                    retValue = Math.Atan(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Sqrt:
                    retValue = Math.Sqrt(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Pow:
                    retValue = Math.Pow(Convert.ToDouble(strExpression), 2);
                    break;
                case EnumFormula.Round:
                    retValue = Math.Round(Convert.ToDouble(strExpression), 2);
                    break;
                case EnumFormula.Int:
                    retValue = Convert.ToInt32(Math.Round(Convert.ToDecimal(strExpression), 2));
                    break;
                case EnumFormula.Exp:
                    retValue = Math.Exp(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Abs:
                    retValue = Math.Abs(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Log10:
                    retValue = Math.Log10(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Ln:
                    retValue = Math.Log(Convert.ToDouble(strExpression), Math.E);
                    break;
            }
            if (retValue == 0) return Convert.ToDouble(strExpression);
            return retValue;
        }

        /// <summary>
        /// ��ȡ������λ��
        /// </summary>
        /// <param name="strExpression"></param>
        /// <returns></returns>
        private static int GetNextPos(string strExpression)
        {
            int[] ExpPos = new int[4];
            ExpPos[0] = strExpression.IndexOf("+");
            ExpPos[1] = strExpression.IndexOf("-");
            ExpPos[2] = strExpression.IndexOf("*");
            ExpPos[3] = strExpression.IndexOf("/");
            int tmpMin = strExpression.Length;
            for (int count = 1; count <= ExpPos.Length; count++)
            {
                if (tmpMin > ExpPos[count - 1] && ExpPos[count - 1] != -1)
                {
                    tmpMin = ExpPos[count - 1];
                }
            }
            return tmpMin;
        }


        private static int GetPrivorPos(string strExpression)
        {
            int[] ExpPos = new int[4];
            ExpPos[0] = strExpression.LastIndexOf("+");
            ExpPos[1] = strExpression.LastIndexOf("-");
            ExpPos[2] = strExpression.LastIndexOf("*");
            ExpPos[3] = strExpression.LastIndexOf("/");
            int tmpMax = -1;
            for (int count = 1; count <= ExpPos.Length; count++)
            {
                if (tmpMax < ExpPos[count - 1] && ExpPos[count - 1] != -1)
                {
                    tmpMax = ExpPos[count - 1];
                }
            }
            return tmpMax;

        }
        /// <summary>
        /// �ָ���ʽ������
        /// </summary>
        /// <param name="strExpression"></param>
        /// <returns></returns>
        public static string SpiltExpression(string strExpression)
        {
            string pi = "3.141593";
            strExpression = strExpression.ToLower().Replace("pi()", pi);

            string strTemp = "";
            string strExp = "";
            while (strExpression.IndexOf("(") != -1)
            {
                strTemp = strExpression.Substring(strExpression.LastIndexOf("(") + 1, strExpression.Length - strExpression.LastIndexOf("(") - 1);
                strExp = strTemp.Substring(0, strTemp.IndexOf(")"));
                strExpression = strExpression.Replace("(" + strExp + ")", CalculateExpress(strExp).ToString());
            }
            if (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
            || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                strExpression = CalculateExpress(strExpression).ToString();
            }
            return strExpression;
        }

        private static string GetExpType(string strExpression)
        {
            if (string.IsNullOrEmpty(strExpression))
            {
                strExpression = "0";
            }
            strExpression = strExpression.ToUpper();
            if (strExpression.IndexOf("SIN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Sin).ToString();
            }
            if (strExpression.IndexOf("COS") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("S") + 1, strExpression.Length - 1 - strExpression.IndexOf("S")), EnumFormula.Cos).ToString();
            }
            if (strExpression.IndexOf("TAN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Tan).ToString();
            }
            if (strExpression.IndexOf("ATAN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.ATan).ToString();
            }
            if (strExpression.IndexOf("SQRT") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("T") + 1, strExpression.Length - 1 - strExpression.IndexOf("T")), EnumFormula.Sqrt).ToString();
            }
            if (strExpression.IndexOf("POW") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("W") + 1, strExpression.Length - 1 - strExpression.IndexOf("W")), EnumFormula.Pow).ToString();
            }
            if (strExpression.IndexOf("ROUND") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("D") + 1, strExpression.Length - 1 - strExpression.IndexOf("D")), EnumFormula.Round).ToString();
            }
            if (strExpression.IndexOf("INT") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("T") + 1, strExpression.Length - 1 - strExpression.IndexOf("T")), EnumFormula.Int).ToString();
            }
            if (strExpression.IndexOf("EXP") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("P") + 1, strExpression.Length - 1 - strExpression.IndexOf("P")), EnumFormula.Exp).ToString();
            }
            if (strExpression.IndexOf("ABS") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("S") + 1, strExpression.Length - 1 - strExpression.IndexOf("S")), EnumFormula.Abs).ToString();
            }
            if (strExpression.IndexOf("LOG10") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("0") + 1, strExpression.Length - 1 - strExpression.IndexOf("0")), EnumFormula.Log10).ToString();
            }
            if (strExpression.IndexOf("LN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Ln).ToString();
            }
            return strExpression;
        }
    }

    /// <summary>
    /// ���ڰѶ���������ת���ض��������͵ķ���
    /// </summary>
    public class ConvertHelper
    {
        #region ���������ת���ַ��������ķ���
        /// <summary>
        /// ���������ת���ַ��������ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>�ַ�������</returns>
        public static string GetString(object obj)
        {
            return (obj == DBNull.Value || obj == null) ? "" : obj.ToString();
        }
        #endregion

        #region ���������ת��32λ�����ͱ����ķ���
        /// <summary>
        /// ���������ת��32λ�����ͱ����ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>32λ�����ͱ���</returns>
        public static int GetInteger(object obj)
        {
            return ConvertStringToInteger(GetString(obj));
        }
        #endregion

        #region ���������ת��64λ�����ͱ����ķ���
        /// <summary>
        /// ���������ת��64λ�����ͱ����ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>64λ�����ͱ���</returns>
        public static long GetLong(object obj)
        {
            return ConvertStringToLong(GetString(obj));
        }
        #endregion

        #region ���������ת��˫���ȸ����ͱ����ķ���
        /// <summary>
        /// ���������ת��˫���ȸ����ͱ����ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>˫���ȸ����ͱ���</returns>
        public static double GetDouble(object obj)
        {
            return ConvertStringToDouble(GetString(obj));
        }
        #endregion

        #region ���������ת��ʮ�������ֱ����ķ���
        /// <summary>
        /// ���������ת��ʮ�������ֱ����ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>ʮ�������ֱ���</returns>
        public static decimal GetDecimal(object obj)
        {
            return ConvertStringToDecimal(GetString(obj));
        }
        #endregion

        #region ���������ת�ɲ����ͱ����ķ���
        /// <summary>
        /// ���������ת�ɲ����ͱ����ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>�����ͱ���</returns>
        public static bool GetBoolean(object obj)
        {
            return (obj == DBNull.Value || obj == null) ? false :
                GetString(obj).Length == 0 ? false : Convert.ToBoolean(obj);
        }
        #endregion

        #region ���������ת������ʱ�����ַ��������ķ���
        /// <summary>
        /// ���������ת������ʱ�����ַ��������ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <param name="sFormat">��ʽ�ַ�</param>
        /// <returns>ʱ�����ַ�������</returns>
        public static string GetDateTimeString(object obj, string sFormat)
        {
            return (obj == DBNull.Value || obj == null) ? "" : Convert.ToDateTime(obj).ToString(sFormat);
        }
        #endregion

        #region ���������ת�������ַ��������ķ���
        /// <summary>
        /// ���������ת�������ַ��������ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>�����ַ�������</returns>
        public static string GetShortDateString(object obj)
        {
            return GetDateTimeString(obj, "yyyy-MM-dd");
        }
        #endregion

        #region ���������ת�������ͱ����ķ���
        /// <summary>
        /// ���������ת�������ͱ����ķ���
        /// </summary>
        /// <param name="obj">�������</param>
        /// <returns>�����ͱ���</returns>
        public static DateTime GetDateTime(object obj)
        {
            return obj == null || obj == DBNull.Value ? new DateTime() : Convert.ToDateTime(obj);
        }
        #endregion

        #region ���ַ���ת��32λ�����ͱ����ķ���
        /// <summary>
        /// ���ַ���ת��32λ�����ͱ����ķ���
        /// </summary>
        /// <param name="s">�ַ���</param>
        /// <returns>32λ�����ͱ���</returns>
        private static int ConvertStringToInteger(string s)
        {
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }
        #endregion

        #region ���ַ���ת��64λ�����ͱ����ķ���
        /// <summary>
        /// ���ַ���ת��64λ�����ͱ����ķ���
        /// </summary>
        /// <param name="s">�ַ���</param>
        /// <returns>64λ�����ͱ���</returns>
        private static long ConvertStringToLong(string s)
        {
            long result = 0;
            long.TryParse(s, out result);
            return result;
        }
        #endregion

        #region ���ַ���ת��˫���ȸ����ͱ����ķ���
        /// <summary>
        /// ���ַ���ת��˫���ȸ����ͱ����ķ���
        /// </summary>
        /// <param name="s">�ַ���</param>
        /// <returns>˫���ȸ����ͱ���</returns>
        private static double ConvertStringToDouble(string s)
        {
            double result = 0;
            double.TryParse(s, out result);
            return result;
        }
        #endregion

        #region ���ַ���ת��ʮ�������ֱ����ķ���
        /// <summary>
        /// ���ַ���ת��ʮ�������ֱ����ķ���
        /// </summary>
        /// <param name="s">�ַ���</param>
        /// <returns>ʮ�������ֱ���</returns>
        private static decimal ConvertStringToDecimal(string s)
        {
            decimal result = 0;
            decimal.TryParse(s, out result);
            return result;
        }
        #endregion


        #region ���ַ���ת���ɼ�����ʽ�����ؼ�����,֧�ִ󲿷���ѧ����
        /// <summary>
        /// ���ַ���ת���ɱ��ʽ�����ؽ��
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static dynamic ExpressEval(string expression)
        {
                string strVal = FormulaDeal.SpiltExpression(expression);
                return strVal;
        }
        #endregion

        /// <summary>
        /// ���ַ���ת���ɱ��ʽ�����ؽ��
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        //public static dynamic Eval(string expression)
        //{
        //    try
        //    {
        //        MSScriptControl.ScriptControl mspt = new MSScriptControl.ScriptControl();
        //        mspt.Language = "vbscript";
        //        return mspt.Eval(expression).ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

    }
}