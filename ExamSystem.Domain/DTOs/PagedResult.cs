using System.Collections.Generic;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 分页结果DTO
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

        public PagedResult()
        {
            Items = new List<T>();
        }
    }
}
