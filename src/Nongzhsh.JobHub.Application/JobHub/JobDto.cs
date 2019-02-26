using System;

namespace Nongzhsh.JobHub.JobHub
{
    [Serializable]
    public class JobDto
    {
        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 薪水
        /// </summary>
        public string Salary { get; set; }

        /// <summary>
        /// 工作地点
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishedTime { get; set; }

        /// <summary>
        /// 详情url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// 工作年限
        /// </summary>
        public string WorkingYears { get; set; }
    }
}
