﻿namespace NPlatform.Repositories
{
    using NPlatform.Filters;
    using NPlatform.Repositories.IRepositories;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// 默认仓储上下文配置
    /// </summary>
    public class RepositoryOptions : IRepositoryOptions
    {

        /// <summary>
        /// 超时时间
        /// </summary>
        public int? TimeOut { get; set; } = 60;

        /// <summary>
        /// 过滤器集合
        /// </summary>
        private Dictionary<string, IQueryFilter> queryFilters = new Dictionary<string, IQueryFilter>();

        /// <summary>
        /// Gets or sets 过滤器清单
        /// </summary>
        public IDictionary<string, IQueryFilter> AllQueryFilters
        {
            get
            {
                return this.queryFilters;
            }

            set => this.queryFilters = value as Dictionary<string, IQueryFilter>;
        }

        /// <summary>
        /// Gets or sets 过滤器清单
        /// </summary>
        public IDictionary<string, IQueryFilter> QueryFilters
        {
            get
            {
                return this.queryFilters.Where(t => t.Value.IsEnabled == true).ToDictionary(x => x.Key, y => y.Value);
            }

            set => this.queryFilters = value as Dictionary<string, IQueryFilter>;
        }

        /// <summary>
        /// 过滤器集合
        /// </summary>
        private Dictionary<string, IResultFilter> resultFilters = new Dictionary<string, IResultFilter>();

        /// <summary>
        /// Gets or sets 过滤器清单
        /// </summary>
        public IDictionary<string, IResultFilter> AllResultFilters
        {
            get
            {
                return this.resultFilters;
            }

            set => this.resultFilters = value as Dictionary<string, IResultFilter>;
        }

        /// <summary>
        /// Gets or sets 过滤器清单
        /// </summary>
        public IDictionary<string, IResultFilter> ResultFilters
        {
            get
            {
                return this.resultFilters.Where(t => t.Value.IsEnabled == true).ToDictionary(x => x.Key, y => y.Value); ;
            }

            set => this.resultFilters = value as Dictionary<string, IResultFilter>;
        }

        /// <summary>
        /// Gets or sets 连接字符串
        /// </summary>
        public string MainConection { get; set; } = string.Empty;

        /// <summary>
        /// 从库连接
        /// </summary>
        public string MinorConnection { get; set; }

        /// <summary>
        /// Gets or sets 数据库驱动
        /// </summary>
        public DBProvider DBProvider { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryOptions"/> class. 
        /// 初始化默认的仓储配置
        /// </summary>
        public RepositoryOptions()
        {
            this.queryFilters.Add(nameof(LogicDeleteFilter), new LogicDeleteFilter());
            this.queryFilters.Add(nameof(ClientFilter), new ClientFilter());
        }

    }
}