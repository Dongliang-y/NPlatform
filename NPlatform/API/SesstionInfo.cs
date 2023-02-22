#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 类 名 称 ：AuthInfoDto
* 类 描 述 ：
* 所在的域 ：DESKTOP-QUTJPVF
* 命名空间 ：NPlatform.AuthorizationDto
* 机器名称 ：DESKTOP-QUTJPVF
* CLR 版本 ：4.0.30319.42000
* 作    者 ：王盈攀
* 创建时间 ：2018-12-29 9:59:42
* 更新时间 ：2018-12-29 9:59:42
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ 王盈攀 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/

#endregion << 版 本 注 释 >>
/* 项目“NPlatform (net5.0)”的未合并的更改
在此之前:
using System.Text;
using NPlatform;
在此之后:
using System.Text;
*/

/* 项目“NPlatform (net6.0)”的未合并的更改
在此之前:
using System.Text;
using NPlatform;
在此之后:
using System.Text;
*/



namespace NPlatform.API
{
    /// <summary>
    /// 授权数据
    /// </summary>
    public class SesstionInfo
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 账户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        public string CnName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>

        public string Telephone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 登陆人的角色
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// 默认机构
        /// </summary>
        public string MainOrgCode { get; set; }

        /// <summary>
        /// 默认机构名称
        /// </summary>
        public string MainOrgName { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// 登录设备的唯一标识
        /// </summary>
        public string MacId { get; set; }

        /// <summary>
        /// 登录终端的ID
        /// </summary>
        public string ClientId { get; set; }
    }
}