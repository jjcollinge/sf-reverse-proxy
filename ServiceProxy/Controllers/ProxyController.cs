using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServiceProxy.Controllers
{
    [ServiceRequestActionFilter]
    public class ProxyController : ApiController
    {
        private static readonly HttpClient client = new HttpClient();

        // POST api/values 
        public async Task PostAsync([FromBody]ProxyItem proxy)
        {
            if (proxy.TrafficPercentage > 0 && proxy.TrafficPercentage <= 100) {

                ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
                ResolvedServicePartition partition =
                    await resolver.ResolveAsync(new Uri("fabric:/ReverseProxy/ReverseProxyService"), new ServicePartitionKey(), new CancellationToken());

                var totalEndpoints = partition.Endpoints.Count;

                var targetEndpointCount = Math.Floor(totalEndpoints * (proxy.TrafficPercentage / 100.0));
                foreach (var endpoint in partition.Endpoints)
                {
                    if (targetEndpointCount > 0)
                    {
                        JObject addresses = JObject.Parse(endpoint.Address);
                        string address = (string)addresses["Endpoints"].First;

                        // Send request to subset of nodes to set A:B test policy
                        HttpContent content = new StringContent(JsonConvert.SerializeObject(proxy));
                        await client.PostAsync(address + "/proxy", content);
                        targetEndpointCount--;
                    }
                }
            }
        }
    }
}
