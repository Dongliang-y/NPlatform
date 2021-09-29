/*************************************************************************************
  * CLR版本：       4.0.30319.42000
  * 类 名 称：       Snowflake
  * 机器名称：       DESKTOP123
  * 命名空间：       NPlatform.Infrastructure.IdGenerators
  * 文 件 名：       Snowflake
  * 创建时间：       2020-5-9 17:57:41
  * 作    者：          xxx
  * 说   明：。。。。。
  * 修改时间：
  * 修 改 人：
*************************************************************************************/
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace NPlatform.IdGenerators
{
    /// <summary>
    /// 雪花算法
    /// </summary>
    public  class SnowflakeGenerator 
    {
        private static Snowflake.Core.IdWorker Snow=null;
        private static long WorkerId, DatacenterId;

        public  SnowflakeGenerator(long workerId, long datacenterId)
        {
            if (Snow == null)
            {
                Snow = new Snowflake.Core.IdWorker(workerId, datacenterId);
            }
        }
        public SnowflakeGenerator(long workerId, long datacenterId,long sequence)
        {
            if (Snow == null)
            {
                Snow = new Snowflake.Core.IdWorker(workerId, datacenterId,sequence);
            }
        }

        public long GenerateId()
        {
            return Snow.NextId();
        }

        public  bool IsEmpty(object id)
        {
            if (id == null)
            {
                return true;
            }
            return id.ToString().Trim().IsNullOrEmpty();
        }
    }
}
