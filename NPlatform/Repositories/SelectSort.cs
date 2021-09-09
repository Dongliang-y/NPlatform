/**************************************************************
 *  Filename:    SelectSort.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: SelectSort ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2021/9/8 16:52:38  @Reviser  Initial Version
 **************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPlatform.Repositories.Repositories
{
    public interface ISort
    {
        string PropertyName { get; set; }
        bool Ascending { get; set; }
    }

    public class SelectSort : ISort
    {
        public string PropertyName { get; set; }
        public bool Ascending { get; set; }
    }
}
