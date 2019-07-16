using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using JobHub.JobSearch.Contracts;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Extensions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.VirtualFileSystem;

namespace JobHub.JobSearch.Liepin
{
    [JobSearcherName("猎聘招聘")]
    public class LiepinJobSearcher : IJobSearcher, ISingletonDependency
    {
        protected static IDictionary<string, string[]> CityDictionary = new ConcurrentDictionary<string, string[]>();

        private readonly IRestClient _client;
        private readonly ILogger<LiepinJobSearcher> _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IVirtualFileProvider _virtualFileProvider;

        public LiepinJobSearcher(ILogger<LiepinJobSearcher> logger,
            IJsonSerializer jsonSerializer,
            IVirtualFileProvider virtualFileProvider)
        {
            _client = new RestClient()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            _logger = logger;
            _jsonSerializer = jsonSerializer;
            _virtualFileProvider = virtualFileProvider;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            try
            {
                var cityCode = GetCityCodes(input.City);
                var request = new RestRequest();
                if (input.ExtraProperties.ContainsKey("RequestParams"))
                {
                    var extendedData = _jsonSerializer.Deserialize<RequestParams>(input.ExtraProperties["RequestParams"].ToString());
                    request.AddObject(extendedData);
                }

                //request.AddParameter("industryType", "*"); // 行业类型
                //request.AddParameter("industries", "*");  // 行业
                //request.AddParameter("subIndustry", "*"); // 子行业
                //request.AddParameter("compscale", "*"); // 规模
                //request.AddParameter("compkind", "*"); // 企业性质
                //request.AddParameter("jobKind", "*"); // 职位类型
                //request.AddParameter("salary", "*"); //薪资范围
                //request.AddParameter("pubTime", "*"); // 发布时间
                request.AddParameter("key", input.Keyword);
                request.AddParameter("dqs", cityCode);
                request.AddParameter("curPage", input.PageIndex - 1); //猎聘页索引从0开始
                request.AddParameter("pageSize", input.PageSize);

                _client.BaseUrl = new Uri("http://www.liepin.com/zhaopin/");
                var response = await _client.ExecuteGetTaskAsync(request);
                var htmlString = response.Content;
                var htmlParser = new HtmlParser();
                using (var document = htmlParser.ParseDocument(htmlString))
                {
                    var jobInfoElements = document.QuerySelectorAll("ul.sojob-list li")
                        .Where(t => t.QuerySelector(".job-info h3 a") != null);

                    var jobs = jobInfoElements.Select(t => new JobDto
                    {
                        Position = t.QuerySelector("h3 a").TextContent.Trim(),
                        Company = t.QuerySelector(".company-name a").TextContent,
                        Salary = t.QuerySelector(".text-warning").TextContent,
                        Address = t.QuerySelector(".area").TextContent,
                        PublishedTime = t.QuerySelector("time").TextContent,
                        Education = t.QuerySelector(".edu").TextContent,
                        Url = t.QuerySelector(".job-info h3 a").GetAttribute("href"),
                        WorkingYears = t.QuerySelector(".edu").NextElementSibling.TextContent
                    }).ToList();

                    SetNextRequestParams(input, document, out var totalCount);

                    var resultDto = new JobListResultDto(jobs, totalCount);
                    return resultDto;
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Liepin_001", e.Message, e.StackTrace, e.InnerException);
            }
        }

        private string GetCityCodes(string city)
        {
            var cityCodes = city;

            try
            {
                if (!CityDictionary.Any())
                {
                    var file = _virtualFileProvider.GetFileInfo("/JobHub/JobSearch/Liepin/city-list.json");
                    var fileContent = file.ReadAsString();
                    CityDictionary = _jsonSerializer.Deserialize<IDictionary<string, string[]>>(fileContent);
                }
                if (CityDictionary.Any() && !city.IsNullOrWhiteSpace())
                {
                    var cities = CityDictionary.Where(x => x.Value.Any(y => y.Contains(city)));
                    cityCodes = cities.Select(x => x.Key).JoinAsString(",");
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Liepin_002", e.Message, e.StackTrace, e.InnerException);
            }
            return cityCodes;
        }

        private void SetNextRequestParams(GetJobsInput input, IDocument document, out long totalCount)
        {
            var dataInfoJson = document.Body.GetAttribute("data-info").UrlDecode();
            var dataInfo = _jsonSerializer.Deserialize<DataInfo>(dataInfoJson);
            totalCount = dataInfo.totalcnt;

            /*--- 默认的请求参数 ---*/
            var @params = new RequestParams
            {
                degradeFlag = 0,
                searchType = 1,
                fromSearchBtn = 2,
                sortFlag = 15,

                ckid = dataInfo.ckid,
                init = dataInfo.init ?? -1,
                headckid = dataInfo.headckid,
                d_sfrom = dataInfo.as_from,
                d_ckId = dataInfo.ck_id,
                d_headId = dataInfo.head_id,

                d_curPage = input.PageIndex - 1,
                d_pageSize = input.PageSize,

                siTag = document.QuerySelector("input[name=siTag]").Attributes["value"]?.Value,
                isAnalysis = document.QuerySelector("input[name=isAnalysis]").Attributes["value"]?.Value,
                flushckid = document.QuerySelector("input[name=flushckid]").Attributes["value"]?.Value,
                clean_condition = document.QuerySelector("input[name=clean_condition]").Attributes["value"]?.Value,
            };

            input.ExtraProperties["RequestParams"] = _jsonSerializer.Serialize(@params);
        }

        public class DataInfo
        {
            public string ckid { get; set; }
            public string headckid { get; set; }
            public string as_from { get; set; }
            public string ck_id { get; set; }
            public string head_id { get; set; }
            public int? init { get; set; }
            public int totalcnt { get; set; }
        }

        public class RequestParams
        {
            public int degradeFlag { get; set; }
            public int d_curPage { get; set; }
            public int d_pageSize { get; set; }
            public int fromSearchBtn { get; set; }
            public int sortFlag { get; set; }
            public int searchType { get; set; }
            public int init { get; set; }
            public string ckid { get; set; }
            public string isAnalysis { get; set; }
            public string headckid { get; set; }
            public string flushckid { get; set; }
            public string clean_condition { get; set; }
            public string d_sfrom { get; set; }
            public string d_ckId { get; set; }
            public string d_headId { get; set; }
            public string siTag { get; set; }
        }
    }
}
