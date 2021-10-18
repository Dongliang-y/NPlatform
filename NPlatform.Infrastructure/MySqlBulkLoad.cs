/**************************************************************
 *  Filename:    MySQLUnity.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: MySQLUnity ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/10/18 15:40:40  @Reviser  Initial Version
 **************************************************************/
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPlatform.DapperRepository
{
    public class MySqlBulkLoad
    {

        /// <summary>
        /// 类型是否为空
        /// </summary>
        /// <param name="t">t</param>
        /// <returns>bool</returns>
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="dt">要导入的数据表，注意列头要和数据库匹配</param>
        /// <returns></returns>
        public int BulkLoad(DataTable table,MySqlConnection connection)
        {

            var columns = table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList();

            var cacheFileInfo = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), table.TableName + ".csv");

            var file = new System.IO.FileInfo(cacheFileInfo);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            string csv = DataTableToCsv(table);
            File.WriteAllText(cacheFileInfo, csv);

            MySqlBulkLoader bulk = new MySqlBulkLoader(connection)
            {
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                LineTerminator = "\r\n",
                FileName = file.FullName,
                NumberOfLinesToSkip = 0,
                TableName = table.TableName,
                Local = true
            };

            bulk.Columns.AddRange(columns);
            return bulk.Load();
        }

        ///将DataTable转换为标准的CSV  
        /// </summary>  
        /// <param name="table">数据表</param>  
        /// <returns>返回标准的CSV</returns>  
        private static string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    var value = row[colum]==null?"": row[colum].ToString();

                    if (colum.DataType == typeof(string))
                    {
                        if (value.Contains("\"") || value.Contains(","))
                        {
                            value = "\"" + value.Replace("\"", "\"\"") + "\"";
                        }
                        sb.Append(value);
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
