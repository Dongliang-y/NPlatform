﻿/***********************************************************
**项目名称:	                                                                  				   
**功能描述:	  的摘要说明
**作    者: 	易栋梁                                         			   
**版 本 号:	1.0                                             			   
**创建日期： 2015/12/29 16:15:32
**修改历史：
************************************************************/

namespace NPlatform.Infrastructure
{
    /// <summary>
    /// 货币金额中文大写转换
    /// </summary>
    public class ConvertMoney
    {
        private static readonly string cnNumber = "零壹贰叁肆伍陆柒捌玖";

        private static readonly string cnUnit = "分角元拾佰仟万拾佰仟亿拾佰仟兆拾佰仟";

        private static readonly string[] enLargeNumber =
            {
                "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
            };

        private static readonly string[] enSmallNumber =
            {
                string.Empty, "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN",
                "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
            };

        private static readonly string[] enUnit = { string.Empty, "THOUSAND", "MILLION", "BILLION", "TRILLION" };

        /// <summary>
        /// 以下是货币金额中文大写转换方法
        /// </summary>
        public static string GetCnString(decimal money)
        {
            string MoneyString = money.ToString();
            string[] tmpString = MoneyString.Split('.');
            string intString = MoneyString; // 默认为整数
            string decString = string.Empty; // 保存小数部分字串
            string rmbCapital = string.Empty; // 保存中文大写字串
            int k;
            int j;
            int n;

            if (tmpString.Length > 1)
            {
                intString = tmpString[0]; // 取整数部分
                decString = tmpString[1]; // 取小数部分
            }

            decString += "00";
            decString = decString.Substring(0, 2); // 保留两位小数位
            intString += decString;

            try
            {
                k = intString.Length - 1;
                if (k > 0 && k < 18)
                {
                    for (int i = 0; i <= k; i++)
                    {
                        j = (int)intString[i] - 48;

                        // rmbCapital = rmbCapital + cnNumber[j] + cnUnit[k-i];     // 供调试用的直接转换
                        n = i + 1 >= k ? (int)intString[k] - 48 : (int)intString[i + 1] - 48; // 等效于 if( ){ }else{ }
                        if (j == 0)
                        {
                            if (k - i == 2 || k - i == 6 || k - i == 10 || k - i == 14)
                            {
                                rmbCapital += cnUnit[k - i];
                            }
                            else
                            {
                                if (n != 0)
                                {
                                    rmbCapital += cnNumber[j];
                                }
                            }
                        }
                        else
                        {
                            rmbCapital = rmbCapital + cnNumber[j] + cnUnit[k - i];
                        }
                    }

                    rmbCapital = rmbCapital.Replace("兆亿万", "兆");
                    rmbCapital = rmbCapital.Replace("兆亿", "兆");
                    rmbCapital = rmbCapital.Replace("亿万", "亿");
                    rmbCapital = rmbCapital.TrimStart('元');
                    rmbCapital = rmbCapital.TrimStart('零');

                    return rmbCapital;
                }
                else
                {
                    return string.Empty; // 超出转换范围时，返回零长字串
                }
            }
            catch
            {
                return string.Empty; // 含有非数值字符时，返回零长字串
            }
        }

        /// <summary>
        /// 以下是货币金额英文大写转换方法
        /// </summary>
        public static string GetEnString(decimal money)
        {
            string MoneyString = money.ToString();
            string[] tmpString = MoneyString.Split('.');
            string intString = MoneyString; // 默认为整数
            string decString = string.Empty; // 保存小数部分字串
            string engCapital = string.Empty; // 保存英文大写字串
            string strBuff1;
            string strBuff2;
            string strBuff3;
            int curPoint;
            int i1;
            int i2;
            int i3;
            int k;
            int n;

            if (tmpString.Length > 1)
            {
                intString = tmpString[0]; // 取整数部分
                decString = tmpString[1]; // 取小数部分
            }

            decString += "00";
            decString = decString.Substring(0, 2); // 保留两位小数位

            try
            {
                // 以下处理整数部分
                curPoint = intString.Length - 1;
                if (curPoint >= 0 && curPoint < 15)
                {
                    k = 0;
                    while (curPoint >= 0)
                    {
                        strBuff1 = string.Empty;
                        strBuff2 = string.Empty;
                        strBuff3 = string.Empty;
                        if (curPoint >= 2)
                        {
                            n = int.Parse(intString.Substring(curPoint - 2, 3));
                            if (n != 0)
                            {
                                i1 = n / 100; // 取佰位数值
                                i2 = (n - i1 * 100) / 10; // 取拾位数值
                                i3 = n - i1 * 100 - i2 * 10; // 取个位数值
                                if (i1 != 0)
                                {
                                    strBuff1 = enSmallNumber[i1] + " HUNDRED ";
                                }

                                if (i2 != 0)
                                {
                                    if (i2 == 1)
                                    {
                                        strBuff2 = enSmallNumber[i2 * 10 + i3] + " ";
                                    }
                                    else
                                    {
                                        strBuff2 = enLargeNumber[i2 - 2] + " ";
                                        if (i3 != 0)
                                        {
                                            strBuff3 = enSmallNumber[i3] + " ";
                                        }
                                    }
                                }
                                else
                                {
                                    if (i3 != 0)
                                    {
                                        strBuff3 = enSmallNumber[i3] + " ";
                                    }
                                }

                                engCapital = strBuff1 + strBuff2 + strBuff3 + enUnit[k] + " " + engCapital;
                            }
                        }
                        else
                        {
                            n = int.Parse(intString.Substring(0, curPoint + 1));
                            if (n != 0)
                            {
                                i2 = n / 10; // 取拾位数值
                                i3 = n - i2 * 10; // 取个位数值
                                if (i2 != 0)
                                {
                                    if (i2 == 1)
                                    {
                                        strBuff2 = enSmallNumber[i2 * 10 + i3] + " ";
                                    }
                                    else
                                    {
                                        strBuff2 = enLargeNumber[i2 - 2] + " ";
                                        if (i3 != 0)
                                        {
                                            strBuff3 = enSmallNumber[i3] + " ";
                                        }
                                    }
                                }
                                else
                                {
                                    if (i3 != 0)
                                    {
                                        strBuff3 = enSmallNumber[i3] + " ";
                                    }
                                }

                                engCapital = strBuff2 + strBuff3 + enUnit[k] + " " + engCapital;
                            }
                        }

                        ++k;
                        curPoint -= 3;
                    }

                    engCapital = engCapital.TrimEnd();
                }

                // 以下处理小数部分
                strBuff2 = string.Empty;
                strBuff3 = string.Empty;
                n = int.Parse(decString);
                if (n != 0)
                {
                    i2 = n / 10; // 取拾位数值
                    i3 = n - i2 * 10; // 取个位数值
                    if (i2 != 0)
                    {
                        if (i2 == 1)
                        {
                            strBuff2 = enSmallNumber[i2 * 10 + i3] + " ";
                        }
                        else
                        {
                            strBuff2 = enLargeNumber[i2 - 2] + " ";
                            if (i3 != 0)
                            {
                                strBuff3 = enSmallNumber[i3] + " ";
                            }
                        }
                    }
                    else
                    {
                        if (i3 != 0)
                        {
                            strBuff3 = enSmallNumber[i3] + " ";
                        }
                    }

                    // 将小数字串追加到整数字串后
                    if (engCapital.Length > 0)
                    {
                        engCapital = engCapital + " AND CENTS " + strBuff2 + strBuff3; // 有整数部分时
                    }
                    else
                    {
                        engCapital = "CENTS " + strBuff2 + strBuff3; // 只有小数部分时
                    }
                }

                engCapital = engCapital.TrimEnd();
                return engCapital;
            }
            catch
            {
                return string.Empty; // 含非数字字符时，返回零长字串
            }
        }
    }
}