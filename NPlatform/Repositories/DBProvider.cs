﻿/**************************************************************
 *  Filename:    DBProvider.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: DBProvider ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/10/18 16:22:26  @Reviser  Initial Version
 **************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
        SQLite
    }
}