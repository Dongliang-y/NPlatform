/**************************************************************
 *  Filename:    ICommand.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: ICommand ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2022/2/11 10:07:02  @Reviser  Initial Version
 **************************************************************/
using NPlatform.Dto;
using NPlatform.Enums;
using NPlatform.Result;

namespace NPlatform
{

    public interface ICommand : MediatR.IRequest<INPResult>, IDto
    {
        public CType CommandType { get; }

    }
}
