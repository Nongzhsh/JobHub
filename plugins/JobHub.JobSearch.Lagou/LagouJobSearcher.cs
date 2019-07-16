using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobHub.JobSearch.Contracts;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Extensions;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace JobHub.JobSearch.Lagou
{
    [JobSearcherName("拉钩招聘")]
    public class LagouJobSearcher : IJobSearcher, ISingletonDependency
    {
        protected static IDictionary<string, string> NextRequestCookies = new ConcurrentDictionary<string, string>();

        private readonly ILogger<LagouJobSearcher> _logger;
        private readonly IJsonSerializer _jsonSerializer;
        public LagouJobSearcher(ILogger<LagouJobSearcher> logger,
            IJsonSerializer jsonSerializer)
        {
            _logger = logger;
            _jsonSerializer = jsonSerializer;
        }
        private async Task<IDictionary<string, string>> GetRequestCookieAsync(string keyword, string city)
        {
            var request = new RestRequest();

            #region Heades
            request.AddHeader("Accept", "text/html, application/xhtml+xml, application/xml; q=0.9, */*; q=0.8");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.5, en; q=0.3");
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Connection", "Keep-Alive");
            request.AddHeader("Host", "www.lagou.com");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362");
            #endregion

            var url = $"https://www.lagou.com/jobs/list_{keyword}?px=default&city={city}";
            var response = await new RestClient(url).ExecuteGetTaskAsync<dynamic>(request);
            var cookies = response.Cookies;
            foreach (var item in cookies)
            {
                NextRequestCookies.Add(item.Name, item.Value);
            }

            return NextRequestCookies;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            try
            {
                var times = 0;
                var city = input.City.IsNullOrEmpty() ? "" : input.City.UrlEncode();
                var keyword = input.Keyword.IsNullOrEmpty() ? "" : input.Keyword.UrlEncode();

                var request = new RestRequest();
                request.AddParameter("kd", keyword);
                request.AddParameter("pn", input.PageIndex);
                request.AddParameter("pageSize", input.PageSize);

                #region Heades & Cookie
                request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", "zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.5, en; q=0.3");
                request.AddHeader("Cache-Control", "max-age=0");
                request.AddHeader("Connection", "Keep-Alive");
                request.AddHeader("Content-Length", "23");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.AddHeader("Host", "www.lagou.com");
                request.AddHeader("Origin", "https://www.lagou.com");
                request.AddHeader("Referer", $"https://www.lagou.com/jobs/list_{keyword}?px=default&city={city}");
                request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362");
                request.AddHeader("X-Anit-Forge-Code", "0");
                request.AddHeader("X-Anit-Forge-Token", "None");
                request.AddHeader("X-Requested-With", "XMLHttpRequest");

            SetRequestCookie:
                var cookies = NextRequestCookies.Any() ? NextRequestCookies : await GetRequestCookieAsync(keyword, city);
                request.AddHeader("Cookie", $"JSESSIONID={cookies["JSESSIONID"]}; SEARCH_ID={cookies["SEARCH_ID"]}; user_trace_token={cookies["user_trace_token"]}; X_HTTP_TOKEN={cookies["X_HTTP_TOKEN"]}");
                foreach (var item in cookies)
                {
                    request.AddCookie(item.Key, item.Value);
                }
                #endregion

                var url = $"https://www.lagou.com/jobs/positionAjax.json?px=default&city={city}&needAddtionalResult=false";
                var response = await new RestClient(url).ExecutePostTaskAsync(request);
                if (response.Content.Contains("频繁"))
                {
                    if (times++ < 5)
                    {
                        NextRequestCookies.Clear();
                        goto SetRequestCookie;
                    }
                }

                var responseData = _jsonSerializer.Deserialize<dynamic>(response.Content);
                var jobList = responseData.content.positionResult.result;
                var jobs = new List<JobDto>();
                foreach (var item in jobList)
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
            catch (Exception e)
            {
                _logger.LogException(e);
                return new JobListResultDto();
            }
        }
    }
}
