using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

namespace JobHub.JobSearch.Boss
{
    [JobSearcherName("Boss招聘")]
    public class BossJobSearcher : IJobSearcher, ISingletonDependency
    {
        protected static IDictionary<string, string> CityDictionary = new ConcurrentDictionary<string, string>();

        private readonly IRestClient _client;
        private readonly IVirtualFileProvider _virtualFileProvider;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger<BossJobSearcher> _logger;

        public BossJobSearcher(ILogger<BossJobSearcher> logger,
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
                request.AddParameter("query", input.Keyword);
                request.AddParameter("city", cityCodes);
                request.AddParameter("page", input.PageIndex);

                // pageSize 无意义，Boss固定的 PageSize，
                // 但我们需要更新分页组件，所以需要重新设置 input.PageSize = 30
                input.PageSize = 30;
                request.AddParameter("pageSize", input.PageSize);

                const string baseUrl = "https://www.zhipin.com/job_detail/";
                _client.BaseUrl = new Uri(baseUrl);
                var response = await _client.ExecuteGetTaskAsync(request);
                var htmlParser = new HtmlParser();
                var document = htmlParser.ParseDocument(response.Content);
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
            catch (Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Boss_001", e.Message, e.StackTrace, e.InnerException);
            }
        }

        private string GetCityCodes(string city)
        {
            var cityCodes = city;

            try
            {
                if (!CityDictionary.Any())
                {
                    var file = _virtualFileProvider.GetFileInfo("/JobHub/JobSearch/Boss/city-list.json");
                    var fileContent = file.ReadAsString();
                    var data = _jsonSerializer.Deserialize<dynamic>(fileContent);
                    InitCityDictionary(data.cityList);
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
                throw new BusinessException("Boss_002", e.Message, e.StackTrace, e.InnerException);
            }
            return cityCodes;
        }

        private void InitCityDictionary(dynamic data)
        {
            foreach (var item in data)
            {
                CityDictionary[item.code.ToString()] = item.name.ToString();

                if (item.subLevelModelList != null)
                {
                    InitCityDictionary(item.subLevelModelList);
                }
            }
        }
    }
}
