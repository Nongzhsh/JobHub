using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using JobHub.JobSearch.Contracts;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using RestSharp;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.VirtualFileSystem;

namespace JobHub.JobSearch.Qiancheng
{
    [JobSearcherName("前程无忧")]
    public class QianchengJobSearcher : IJobSearcher, ISingletonDependency
    {
        public static IDictionary<string, string> CityDictionary = new ConcurrentDictionary<string, string>();

        private readonly IRestClient _client;
        private readonly IVirtualFileProvider _virtualFileProvider;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger<QianchengJobSearcher> _logger;

        public QianchengJobSearcher(ILogger<QianchengJobSearcher> logger,
            IVirtualFileProvider virtualFileProvider,
            IJsonSerializer jsonSerializer)
        {
            _client = new RestClient()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            _logger = logger;
            _virtualFileProvider = virtualFileProvider;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            try
            {
                var cityCodes = GetCityCodes(input.City);
                var request = new RestRequest();
                //request.AddParameter("salary", "*"); //TODO: 实现自定义
                request.AddParameter("keyword", input.Keyword);
                request.AddParameter("jobarea", cityCodes);
                request.AddParameter("curr_page", input.PageIndex);

                // pageSize 无意义，前程无忧固定的 PageSize，
                // 但我们需要更新分页组件，所以需要重新设置 input.PageSize = 50
                input.PageSize = 50;
                request.AddParameter("pageSize", input.PageSize);

                const string baseUrl = "https://search.51job.com/jobsearch/search_result.php";
                _client.BaseUrl = new Uri(baseUrl);
                var response = await _client.ExecuteGetTaskAsync(request);
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
                    Url = t.QuerySelector(".t1 span a").Attributes["href"]?.Value
                }).ToList();

                long.TryParse(document.QuerySelector("input[name=jobid_count]").Attributes["value"]?.Value, out var totalCount);

                return new JobListResultDto(jobs, totalCount);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Qiancheng_001", e.Message, e.StackTrace, e.InnerException);
            }
        }

        private string GetCityCodes(string city)
        {
            var cityCodes = city;
            try
            {
                if (!CityDictionary.Any())
                {
                    var file = _virtualFileProvider.GetFileInfo("/JobHub/JobSearch/Qiancheng/city-list.json");
                    var fileContent = file.ReadAsString();
                    CityDictionary = _jsonSerializer.Deserialize<IDictionary<string, string>>(fileContent);
                }

                if (CityDictionary.Any() && !city.IsNullOrWhiteSpace())
                {
                    var cities = CityDictionary.Where(x => x.Value.Contains(city));
                    cityCodes = cities.Select(x => x.Key).JoinAsString(",");
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Qiancheng_002", e.Message, e.StackTrace, e.InnerException);
            }
            return cityCodes;
        }
    }
}
