using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Nongzhsh.JobHub.JobHub
{
    public class ZhilianJobSearcher : ISingletonDependency
    {
        const string BaseUrl = "https://fe-api.zhaopin.com/c/i/sou";

        private readonly IRestClient _client;
        private readonly ILogger<LiepinJobSearcher> _logger;

        public ZhilianJobSearcher(ILogger<LiepinJobSearcher> logger)
        {
            _client = new RestClient(BaseUrl)
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };
            _logger = logger;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            var cityCode = CityCodeDictionary.Get(JobSource.智联招聘, input.City);

            var request = new RestRequest();
            request.AddParameter("kt", 3);
            request.AddParameter("kw", input.Keyword);
            request.AddParameter("cityId", cityCode);

            var pageSize = input.PageSize;
            request.AddParameter("start", (input.PageIndex - 1) * pageSize);
            request.AddParameter("pageSize", pageSize);

            try
            {
                var response = await _client.ExecuteGetTaskAsync<dynamic>(request);
                dynamic responseData = JObject.Parse(response.Content);
                var jobList = responseData.data.results;
                var jobs = new List<JobDto>();
                foreach(var item in jobList)
                {
                    var job = new JobDto
                    {
                        Position = item.jobName,
                        Company = item.company.name,
                        Salary = item.salary,
                        Address = item.city.display,
                        PublishedTime = item.updateDate,
                        Education = item.eduLevel.name,
                        Url = item.positionURL,
                        WorkingYears = item.workingExp.name
                    };

                    jobs.Add(job);
                }

                long totalCount = responseData.data.numTotal;

                return new JobListResultDto(jobs, totalCount);
            }
            catch(Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Zhilian_001", e.Message, e.StackTrace, e.InnerException);
            }
        }
    }
}