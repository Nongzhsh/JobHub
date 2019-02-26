namespace Nongzhsh.JobHub.JobHub
{
    public class GetJobsInput
    {
        public string Keyword { get; set; }

        public string City { get; set; }

        /// <summary>
        /// 月薪范围，如：1001,2000 
        /// </summary>
        public string Salary { get; set; } //TODO：年薪-月薪计算标准？

        public JobSource JobSource { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string ExtendedData { get; set; }
    }
}
