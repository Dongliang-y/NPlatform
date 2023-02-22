/**************************************************************
 *  Filename:    HandlerBase.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: HandlerBase ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2022/3/17 14:33:57  @Reviser  Initial Version
 **************************************************************/
using MediatR;
using NPlatform.Result;

namespace NPlatform.Domains.Service
{
    /// <summary>
    /// Command Handler base
    /// </summary>
    /// <typeparam name="TAdd"></typeparam>
    /// <typeparam name="TDelete"></typeparam>
    /// <typeparam name="TEdit"></typeparam>
    public abstract class HandlerBase<TAdd, TDelete, TEdit> : BaseService, IRequestHandler<ICommand, INPResult>
        where TAdd : class, ICommand where TDelete : class, ICommand where TEdit : class, ICommand
    {
        public async Task<INPResult> Handle(ICommand request, CancellationToken cancellationToken)
        {
            switch (request.CommandType)
            {
                case Enums.CType.ADD:
                    return await this.Add(request as TAdd);
                case Enums.CType.DELETE:
                    return await this.Delete(request as TDelete);
                case Enums.CType.EDIT:
                    return await this.Edit(request as TEdit);
                default:
                    throw new NotSupportedException($"Not Supperted {request.CommandType} Command.");
            }
        }

        /// <summary>
        /// 处理新增命令
        /// </summary>
        /// <param name="addCommand">处理新增命令</param>
        /// <returns></returns>
        public abstract Task<INPResult> Add(TAdd addCommand);
        /// <summary>
        /// 处理Delete
        /// </summary>
        /// <param name="addCommand">Delete</param>
        /// <returns></returns>
        public abstract Task<INPResult> Delete(TDelete addCommand);

        /// <summary>
        /// 处理编辑命令
        /// </summary>
        /// <param name="addCommand">处理编辑命令</param>
        /// <returns></returns>
        public abstract Task<INPResult> Edit(TEdit addCommand);
    }
}
