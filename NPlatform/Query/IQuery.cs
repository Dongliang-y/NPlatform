/**************************************************************
 *  Filename:    IQuery.cs
 *  Copyright:    Co., Ltd.
 *
 *  Description: IQuery ClassFile.
 *
 *  @author:     Dongliang Yi
 *  @version     2022/2/11 9:26:55  @Reviser  Initial Version
 **************************************************************/

using NPlatform.Domains.Entity;

namespace NPlatform.Query
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public interface IQuery
    {
        Expression<Func<TEntity, bool>> GetExp<TEntity>() where TEntity : IEntity;
    }
}
