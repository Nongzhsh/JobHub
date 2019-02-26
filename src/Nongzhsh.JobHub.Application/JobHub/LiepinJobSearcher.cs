using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Nongzhsh.JobHub.JobHub
{
    public class LiepinJobSearcher : ISingletonDependency
    {
        const string BaseUrl = "http://www.liepin.com/zhaopin/";

        private readonly IRestClient _client;
        private readonly ILogger<LiepinJobSearcher> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        public LiepinJobSearcher(ILogger<LiepinJobSearcher> logger, IJsonSerializer jsonSerializer)
        {
            _client = new RestClient(BaseUrl)
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            _logger = logger;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            var cityCode = CityCodeDictionary.Get(JobSource.猎聘招聘, input.City);

            var request = new RestRequest();
            request.AddParameter("salary", "*"); //TODO: 实现自定义
            request.AddParameter("key", input.Keyword);
            request.AddParameter("dqs", cityCode);
            request.AddParameter("curPage", input.PageIndex - 1); //猎聘页索引从0开始
            request.AddParameter("pageSize", input.PageSize);

            if(!input.ExtendedData.IsNullOrEmpty())
            {
                try
                {
                    var extendedData = _jsonSerializer.Deserialize<ExtendedParameters>(input.ExtendedData);
                    request.AddObject(extendedData);
                }
                catch(Exception e)
                {
                    _logger.LogWarning(e.Message, e.StackTrace, e.InnerException);
                }
            }

            try
            {
                var response = await _client.ExecuteGetTaskAsync<dynamic>(request);
                var htmlString = response.Content;
                var htmlParser = new HtmlParser();
                var document = htmlParser.ParseDocument(htmlString);
                var jobInfoElements = document.QuerySelectorAll("ul.sojob-list li")
                    .Where(t => t.QuerySelector(".job-info h3 a") != null);
                
                var jobs = jobInfoElements.Select(t => new JobDto
                {
                    Position = t.QuerySelector("h3 a").TextContent.Trim(),
                    Company = t.QuerySelector(".company-name a").TextContent,
                    Salary = t.QuerySelector(".text-warning").TextContent,
                    Address = t.QuerySelector(".area").TextContent,
                    PublishedTime = t.QuerySelector("time").Attributes["title"].Value,
                    Education = t.QuerySelector(".edu").TextContent,
                    Url = t.QuerySelector(".job-info h3 a").GetAttribute("href"),
                    WorkingYears = t.QuerySelector(".edu").NextElementSibling.TextContent
                }).ToList();

                var dataInfoJson = document.Body.GetAttribute("data-info").UrlDecode();
                dynamic dataInfo = JObject.Parse(dataInfoJson);

                //下次搜索携带的参数
                var extendedParameters = new ExtendedParameters
                {
                    init = dataInfo.init ?? -1,
                    searchType = 1,
                    headckid = dataInfo.headckid,
                    fromSearchBtn = 2,
                    sortFlag = 15,
                    ckid = dataInfo.ckid,
                    degradeFlag = 0,
                    d_sfrom = dataInfo.as_from,
                    d_ckId = dataInfo.ck_id,
                    d_headId = dataInfo.head_id,
                    d_curPage = input.PageIndex - 1,
                    d_pageSize = input.PageSize
                };

                var siTag = document.QuerySelector("input[name=siTag]").Attributes["value"]?.Value;
                extendedParameters.siTag = siTag;

                long totalCount = dataInfo.totalcnt;
                var extendedDataJson = _jsonSerializer.Serialize(extendedParameters);
                var resultDto = new JobListResultDto(jobs, totalCount)
                {
                    NextRequestExtendedData = extendedDataJson
                };

                return resultDto;
            }
            catch(Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Liepin_001", e.Message, e.StackTrace, e.InnerException);
            }
        }

        private class ExtendedParameters
        {
            public int init { get; set; }
            public int searchType { get; set; }
            public string headckid { get; set; }
            public int fromSearchBtn { get; set; }
            public int sortFlag { get; set; }
            public string ckid { get; set; }
            public int degradeFlag { get; set; }
            public string siTag { get; set; }
            public string d_sfrom { get; set; }
            public string d_ckId { get; set; }
            public string d_headId { get; set; }
            public int d_curPage { get; set; }
            public int d_pageSize { get; set; }
        }
    }
}
