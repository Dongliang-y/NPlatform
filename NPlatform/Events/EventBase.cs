/**************************************************************
 *  Filename:    EventBase.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: EventBase ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2022/2/11 11:04:45  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Dto;

namespace NPlatform.Events
{
    /// <summary>
    /// 事件基类  
    /// </summary>
    public abstract class EventBase : BaseDto, IEvent
    {
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 聚合ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 事件基类
        /// </summary>
        public EventBase(string aggregateId) : base(aggregateId)
        {
            this.CreateTime = DateTime.Now;
            this.Id = aggregateId;
        }
        /// <summary>
        /// 校验对象数据
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();
    }
}
