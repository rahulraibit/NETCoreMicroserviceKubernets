using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using models;
using RestSharp;

namespace AKSExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _serviceEndPointProvider;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IConfiguration configuration,
            IPublishEndpoint publishEndpoint,
            ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
            _serviceEndPointProvider = sendEndpointProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var serviceBaseURI = _configuration.GetValue<string>("service2baseuri");
            var _client = new HttpClient();
            var response = await _client.GetAsync($"{serviceBaseURI}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                if (responseData.ToString() == "true")
                {
                    var _result = new StoreContext().Employees.ToList();
                    return Ok(_result);
                }
                return BadRequest("Response string doesn't match");
            }
            return BadRequest("Response from service2 failed");
        }

        [HttpPost]
        public async Task SendEvent([FromQuery] int id)
        {
            var client = new RestClient("https://akssubscription.westus2-1.eventgrid.azure.net/api/events");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("aeg-sas-key", "9kko9KY+GXxEn38W5pSKv0iH7DGLioqmZeVeqYnS4aA=");
            request.AddHeader("Content-Type", "application/json");
            dynamic body = new System.Dynamic.ExpandoObject();
            body.Id = Guid.NewGuid().ToString();
            body.eventType = "EmployeeInserted";
            body.subject = "New Employee Added";
            body.eventTime = DateTime.Now.ToString();
            body.dataversion = "1.0";
            body.data = new System.Dynamic.ExpandoObject();
            var data = new ArrayList();
            data.Add(body);
            body = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        [HttpGet]
        [Route("servicebusqueuesender")]
        public async Task SendUsingAzureServiceBusQeue()
        {
            try
            {
                var sendEndpoint =
                await _serviceEndPointProvider.GetSendEndpoint(
                    new Uri("sb://test2service.servicebus.windows.net/test"));
                await sendEndpoint.Send<Test>(new Test { message = "Hi This message from sender" });
            }
            catch(Exception ex)
            {

            }
        }
        [HttpGet]
        [Route("servicebusTopicsender")]
        public async Task SendUsingAzureServiceBusTopic()
        {
            try
            {
                await _publishEndpoint.Publish<Test>(new Test { message = "Hi This message from sender" });
            }
            catch (Exception ex)
            {

            }
        }
    }
}
