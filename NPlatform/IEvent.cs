/**************************************************************
 *  Filename:    IEvent.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: IEvent ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2022/2/11 10:15:56  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Dto;

namespace NPlatform
{
    /// <summary>
    /// 领域事件
    /// </summary>
    public interface IEvent : MediatR.INotification, IDto
    {
    }
}
