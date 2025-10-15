using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExamSystem.Repository.Interfaces
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// 根据条件查询实体
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 添加实体
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// 保存更改
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// 获取总数
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// 根据条件获取总数
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
