using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace MobiwikFunctionApp
{
    public static class Mobiwik
    {
        [FunctionName("MobiwikDataSet")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

                dynamic data = await req.Content.ReadAsAsync<object>();

            await EncDecLogic.EncryptAES(data, "key");
            

            string responseMessage = $"Data Encrypted";

            return req.CreateResponse(HttpStatusCode.OK, responseMessage);
        }
    }

}
