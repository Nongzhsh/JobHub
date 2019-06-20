using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Json;
using Volo.Blogging.Files;

namespace Nongzhsh.JobHub.Pages.Webhook
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class StatusModel : JobHubPageModelBase
    {
        private readonly IJsonSerializer _jsonSerializer;
        private IOptions<BlogFileOptions> _fileOptions;
        public StatusModel(IJsonSerializer jsonSerializer,IOptions<BlogFileOptions> options)
        {
            _jsonSerializer = jsonSerializer;
            _fileOptions = options;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            /*
             * $tracking_number = $content['tracking_number'];         $reference_number = $content['reference_number'];         $sort_in = $content['sort_in'];         $sort_out = $content['sort_out'];         $close_box = $content['close_box'];         $handover_linehaul = $content['handover_linehaul'];         $reject = $content['reject'];         $return = $content['return'];         $receive = $content['receive'];
             */
            var headers = "";
            foreach (var item in Request.Headers)
            {
                headers += $"{item.Key}:{item.Value}\r\n";
            }

            Logger.LogDebug(message: $"Request-Headers:\r\n{headers}");

            var form = "";
            foreach (var item in Request.Form)
            {
                form += $"{item.Key}:{item.Value}\r\n";
            }

            Logger.LogDebug(message: $"Request-Form:\r\n{form}");

            var filePath = Path.Combine(_fileOptions.Value.FileUploadLocalFolder, "RequestBody.txt");

            System.IO.File.WriteAllBytes(filePath, Request.Body.GetAllBytes());
        }
    }
}