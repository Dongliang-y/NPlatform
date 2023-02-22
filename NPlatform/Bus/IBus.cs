/**************************************************************
 *  Filename:    IBus.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: IBus ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2022/2/11 14:22:38  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Result;

namespace NPlatform.Bus
{
    public interface IBus
    {
        public Task PublishEvent<T>(T eventObj) where T : IEvent;
        public Task<INPResult> SendCommand<T>(T command) where T : ICommand;
    }
}

