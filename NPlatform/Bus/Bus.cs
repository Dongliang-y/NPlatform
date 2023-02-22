/**************************************************************
 *  Filename:    Bus.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: Bus ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2022/2/11 14:22:45  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Result;

namespace NPlatform.Bus
{
    public class Bus : IBus
    {
        private MediatR.IMediator _mediator;
        public Bus(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task PublishEvent<T>(T eventObj) where T : IEvent
        {
            await _mediator.Publish<T>(eventObj);
        }

        public async Task<INPResult> SendCommand<T>(T command) where T : ICommand
        {
            return await _mediator.Send<INPResult>(command);
        }
    }
}
