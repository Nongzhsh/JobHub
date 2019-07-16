using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobHub.JobSearch.Contracts;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using RestSharp;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Volo.Abp.Timing;
using Volo.Abp.VirtualFileSystem;

namespace JobHub.JobSearch.Zhilian
{
    [JobSearcherName("智联招聘")]
    public class ZhilianJobSearcher : IJobSearcher, ISingletonDependency
    {
        protected static IDictionary<string, string> CityDictionary = new ConcurrentDictionary<string, string>();

        private readonly IRestClient _client;
        private readonly IClock _clock;
        private readonly ILogger<ZhilianJobSearcher> _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IVirtualFileProvider _virtualFileProvider;

        public ZhilianJobSearcher(ILogger<ZhilianJobSearcher> logger,
            IJsonSerializer jsonSerializer,
            IVirtualFileProvider virtualFileProvider,
            IClock clock)
        {
            _client = new RestClient()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36"
            };

            _logger = logger;
            _jsonSerializer = jsonSerializer;
            _virtualFileProvider = virtualFileProvider;
            _clock = clock;
        }

        public async Task<JobListResultDto> SearchAsync(GetJobsInput input)
        {
            try
            {
                var cityCode = GetCityCodes(input.City);
                var request = new RestRequest();
                var pageSize = input.PageSize;
                request.AddParameter("start", (input.PageIndex - 1) * pageSize);
                request.AddParameter("pageSize", pageSize);
                request.AddParameter("kw", input.Keyword);
                request.AddParameter("cityId", cityCode);

                var timestamp = _clock.Now.TimeOfDay.TotalMilliseconds;
                var x_zp_client_id = Guid.NewGuid().ToString().ToLower();
                var x_zp_page_request_id = $"{x_zp_client_id.ToMd5()}-{(long)timestamp}-{RandomHelper.GetRandom(100000, 999999)}";
                request.AddParameter("x-zp-page-request-id", x_zp_page_request_id);
                request.AddParameter("x-zp-client-id", x_zp_client_id);
                request.AddParameter("kt", 3);
                request.AddParameter("_v", timestamp);

                _client.BaseUrl = new Uri("https://fe-api.zhaopin.com/c/i/sou");
                var response = await _client.ExecuteGetTaskAsync(request);
                var responseData = _jsonSerializer.Deserialize<dynamic>(response.Content);
                var jobList = responseData.data.results;
                var jobs = new List<JobDto>();
                foreach (var item in jobList)
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

                var resultDto = new JobListResultDto(jobs, totalCount);
                return resultDto;
            }
            catch (Exception e)
            {
                _logger.LogException(e);
                throw new BusinessException("Zhilian_001", e.Message, e.StackTrace, e.InnerException);
            }
        }

        private string GetCityCodes(string city)
        {
            var cityCodes = city;

            try
            {
                if (!CityDictionary.Any())
                {
                    var file = _virtualFileProvider.GetFileInfo("/JobHub/JobSearch/Zhilian/city-list.json");
                    var fileContent = file.ReadAsString();
                    var data = _jsonSerializer.Deserialize<dynamic>(fileContent);
                    InitCityDictionary(data.province);
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
                throw new BusinessException("Zhilian_002", e.Message, e.StackTrace, e.InnerException);
            }
            return cityCodes;
        }

        private void InitCityDictionary(dynamic data)
        {
            foreach (var item in data)
            {
                CityDictionary[item.v.ToString()] = item.n.ToString();

                if (item.s != null)
                {
                    InitCityDictionary(item.s);
                }
            }
        }
    }
}
