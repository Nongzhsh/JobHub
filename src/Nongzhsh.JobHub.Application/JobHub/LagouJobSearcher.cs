using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Nongzhsh.JobHub.JobHub
{
    public class LagouJobSearcher : ISingletonDependency
    {
        private readonly ILogger<LiepinJobSearcher> _logger;

        public LagouJobSearcher(ILogger<LiepinJobSearcher> logger)
        {
            _logger = logger;
        }

        private async Task<IDictionary<string, string>> GetRequestCookieAsync(string city)
        {
            var url = $"https://www.lagou.com/jobs/list_?city={city.UrlEncode()}&cl=false&fromSearch=true&labelWords=&suginput=";
            var client = new RestClient(url)
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            var request = new RestRequest();

            #region Heades
            request.AddHeader("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            #endregion

            var nextRequestCookies = new Dictionary<string, string>
            {
                {"X_MIDDLE_TOKEN", "797bc148d133274a162ba797a6875817"},
                {"_ga", "GA1.2.1912257997.1548059451"},
                {"_gat", "1"},
                {"Hm_lvt_4233e74dff0ae5bd0a3d81c6ccf756e6", "1548059451"},
                {"PRE_UTM", ""},
                {"PRE_HOST", ""},
                {"PRE_SITE", ""},
                {"PRE_LAND", "https%3A%2F%2Fwww.lagou.com%2F%3F_from_mid%3D1"},
                {"_gid", "GA1.2.1194828713.1548059451"},
                {"index_location_city", city.UrlEncode()},
                {"TG-TRACK-CODE", "index_hotjob"},
                {"Hm_lpvt_4233e74dff0ae5bd0a3d81c6ccf756e6", "1548059503"},
            };

            var response = await client.ExecuteGetTaskAsync<dynamic>(request);
            var cookies = response.Cookies; //[JSESSIONID,LGRID,SEARCH_ID]
            foreach(var item in cookies)
            {
                nextRequestCookies.Add(item.Name, item.Value);
            }
            nextRequestCookies.Add("user_trace_token", nextRequestCookies["LGRID"]);
            nextRequestCookies.Add("LGSID", nextRequestCookies["LGRID"]);
            nextRequestCookies.Add("LGUID", nextRequestCookies["LGRID"]);

            return nextRequestCookies;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            var cityCode = CityCodeDictionary.Get(JobSource.拉钩招聘, input.City);
            var cookies = await GetRequestCookieAsync(cityCode);

            var url = $"https://www.lagou.com/jobs/positionAjax.json?px=new&city={cityCode.UrlEncode()}&needAddtionalResult=false";
            var client = new RestClient(url)
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            var request = new RestRequest();

            #region Heades & Cookie
            request.AddHeader("Accept", @"application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("Origin", "https://www.lagou.com");
            request.AddHeader("X-Anit-Forge-Code", "0");

            var refererUrl = $"https://www.lagou.com/jobs/list_{(input.Keyword.IsNullOrEmpty() ? "" : input.Keyword.UrlEncode())}?px=new&city={cityCode.UrlEncode()}&needAddtionalResult=false";
            request.AddHeader("Referer", refererUrl);
            request.AddHeader("X-Anit-Forge-Code", "0");
            request.AddHeader("X-Anit-Forge-Token", "None");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("Cookie", $"JSESSIONID={cookies["JSESSIONID"]};SEARCH_ID={cookies["SEARCH_ID"]};LGRID={cookies["LGRID"]}");

            foreach(var item in cookies)
            {
                request.AddCookie(item.Key, item.Value);
            }
            #endregion

            request.AddParameter("first", "true");
            request.AddParameter("kd", input.Keyword);
            request.AddParameter("pn", input.PageIndex);
            request.AddParameter("pageSize", input.PageSize);
            input.PageSize = 15; //固定的 PageSize
            try
            {
                var response = await client.ExecutePostTaskAsync<dynamic>(request);

                dynamic responseData = JObject.Parse(response.Content);
                var jobList = responseData.content.positionResult.result;
                var jobs = new List<JobDto>();
                foreach(var item in jobList)
                {
                    var job = new JobDto
                    {
                        Position = item.positionName,
                        Company = item.companyFullName,
                        Salary = item.salary,
                        Address = item.city + item.district + item.stationname,
                        PublishedTime = item.formatCreateTime,
                        Education = item.education,
                        Url = $"https://www.lagou.com/jobs/{item.positionId}.html",
                        WorkingYears = item.workYear
                    };

                    jobs.Add(job);
                }

                long totalCount = responseData.content.positionResult.totalCount;

                return new JobListResultDto(jobs, totalCount);
            }
            catch(Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Lagou_001", e.Message, e.StackTrace, e.InnerException);
            }
        }
    }
}