/**************************************************************
 *  Filename:    DBProvider.cs
 *  Copyright:   .
 *
 *  Description: DBProvider ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/10/18 16:22:26  @Reviser  Initial Version
 **************************************************************/
using System.ComponentModel;

namespace NPlatform.Repositories
{
    /// <summary>
    /// 数据库驱动类型
    /// </summary>
    public enum DBProvider
    {
        /// <summary>
        /// oracle客户端驱动
        /// </summary>
        [Description("System.Data.OracleClient")]
        OracleClient,// = "System.Data.OracleClient",
        /// <summary>
        /// MySqlClient 客户端驱动
        /// </summary>
        [Description("MySql.Data.MySqlClient")]
        MySqlClient,// "MySql.Data.MySqlClient",
        /// <summary>
        /// SqlClient 客户端驱动
        /// </summary>
        [Description("System.Data.SqlClient")]
        SqlClient,//"System.Data.SqlClient",
        /// <summary>
        /// SQLite 客户端驱动
        /// </summary>
        [Description("System.Data.SQLite")]
        SQLite,
        /// <summary>
        /// PostgreSQL 客户端驱动
        /// </summary>
        [Description("System.Data.PostgreSQL")]
        PostgreSQL
    }
}
