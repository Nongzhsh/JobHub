using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using RestSharp;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Nongzhsh.JobHub.JobHub
{
    public class BossJobSearcher : ISingletonDependency
    {
        const string BaseUrl = "https://www.zhipin.com/job_detail/";

        private readonly IRestClient _client;
        private readonly ILogger<QianchengJobSearcher> _logger;

        public BossJobSearcher(ILogger<QianchengJobSearcher> logger)
        {
            _client = new RestClient(BaseUrl)
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            _logger = logger;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            var cityCode = CityCodeDictionary.Get(JobSource.Boss招聘, input.City);
            var request = new RestRequest();
            //request.AddParameter("salary", "*"); //TODO: 实现自定义
            request.AddParameter("query", input.Keyword);
            request.AddParameter("city", cityCode);
            request.AddParameter("page", input.PageIndex);
            request.AddParameter("pageSize", input.PageSize);
            input.PageSize = 30; //固定的 PageSize

            try
            {
                var response = await _client.ExecuteGetTaskAsync<dynamic>(request);
                var htmlString = response.Content;
                var htmlParser = new HtmlParser();
                var document = htmlParser.ParseDocument(htmlString);
                var jobInfoElements = document.QuerySelectorAll(".job-list ul li");

                var jobs = jobInfoElements.Select(t =>
                {
                    var dto = new JobDto
                    {
                        Position = t.QuerySelector(".job-title").TextContent.Trim(),
                        Company = t.QuerySelector(".company-text a").TextContent,
                        Salary = t.QuerySelector(".red").TextContent,
                        Address = t.QuerySelector(".info-primary p").InnerHtml.Split("<em class=\"vline\"></em>")[0],
                        PublishedTime = t.QuerySelector(".info-publis p").TextContent,
                        Education = t.QuerySelector(".info-primary p").InnerHtml.Split("<em class=\"vline\"></em>")[2],
                        Url = "http://www.zhipin.com" + t.QuerySelector(".company-text a").Attributes["href"]?.Value,
                        WorkingYears = t.QuerySelector(".info-primary p").InnerHtml.Split("<em class=\"vline\"></em>")[1]
                    };

                    return dto;
                }).ToList();

                long.TryParse(document.QuerySelector(".job-tab").Attributes["data-rescount"]?.Value, out var totalCount);

                return new JobListResultDto(jobs, totalCount);
            }
            catch(Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Boss_001", e.Message, e.StackTrace, e.InnerException);
            }
        }
    }
}
