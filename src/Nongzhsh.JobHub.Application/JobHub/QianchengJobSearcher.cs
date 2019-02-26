using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using RestSharp;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Nongzhsh.JobHub.JobHub
{
    public class QianchengJobSearcher : ISingletonDependency
    {
        const string BaseUrl = "https://search.51job.com/jobsearch/search_result.php";

        private readonly IRestClient _client;
        private readonly ILogger<QianchengJobSearcher> _logger;

        public QianchengJobSearcher(ILogger<QianchengJobSearcher> logger)
        {
            _client = new RestClient(BaseUrl)
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            _logger = logger;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            var cityCode = CityCodeDictionary.Get(JobSource.前程无忧, input.City);
            var request = new RestRequest();
            //request.AddParameter("salary", "*"); //TODO: 实现自定义
            request.AddParameter("keyword", input.Keyword);
            request.AddParameter("jobarea", cityCode);
            request.AddParameter("curr_page", input.PageIndex);

            request.AddParameter("pageSize", input.PageSize);
            input.PageSize = 50; // 前程无忧固定的 PageSize

            try
            {
                var response = await _client.ExecuteGetTaskAsync<dynamic>(request);
                var htmlString = Encoding.GetEncoding("GBK").GetString(response.RawBytes);
                var htmlParser = new HtmlParser();
                var document = htmlParser.ParseDocument(htmlString);
                var jobInfoElements = document.QuerySelectorAll(".dw_table div.el")
                    .Where(t => t.QuerySelector(".t1 span a") != null);

                var jobs = jobInfoElements.Select(t => new JobDto
                {
                    Position = t.QuerySelector(".t1 span a").TextContent.Trim(),
                    Company = t.QuerySelector(".t2 a").TextContent,
                    Salary = t.QuerySelector(".t4").TextContent,
                    Address = t.QuerySelector(".t3").TextContent,
                    PublishedTime = t.QuerySelector(".t5").TextContent,
                    //Education = t.QuerySelector(".edu").TextContent,
                    Url = t.QuerySelector(".t1 span a").Attributes["href"]?.Value
                }).ToList();

                long.TryParse(document.QuerySelector("input[name=jobid_count]").Attributes["value"]?.Value, out var totalCount);

                return new JobListResultDto(jobs, totalCount);
            }
            catch(Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Qiancheng_001", e.Message, e.StackTrace, e.InnerException);
            }
        }
    }
}
