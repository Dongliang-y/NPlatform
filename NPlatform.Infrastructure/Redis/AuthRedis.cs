/**************************************************************
 *  Filename:    AuthRedis.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: AuthRedis ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/14 15:37:51  @Reviser  Initial Version
 **************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace NPlatform.Infrastructure.Redis
{
    /// <summary>
    /// 授权相关的redis操作
    /// </summary>
    public class AuthRedis : RedisTool
    {
        public AuthRedis() : base(0, "Auth")
        {

        }
    }
}
