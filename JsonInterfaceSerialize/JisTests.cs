using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using JsonInterfaceSerialize.Utilities;
using JsonInterfaceSerialize.DataModels;
using dm1 = JsonInterfaceSerialize.DataModels.ModelsV1;
using dm2 = JsonInterfaceSerialize.DataModels.ModelsV2;
using dm3 = JsonInterfaceSerialize.DataModels.ModelsV3;
using dmc3 = JsonInterfaceSerialize.DataModels.ModelsV3.Containers;
using dm4 = JsonInterfaceSerialize.DataModels.ModelsV4;
using dmc4 = JsonInterfaceSerialize.DataModels.ModelsV4.Containers;
using JsonInterfaceSerialize.Services;


namespace JsonInterfaceSerialize
{
    public static class JisTests
    {
        [FunctionName("Test_One")]
        [OpenApiOperation(operationId: "Test_One", tags: new[] { "Tests - Manual" },
                                        Summary = "",
                                        Description = ""
                                        )]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The **Name** of user or application that calls this function.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Description = "The response container, with details of the processing outcome.")]
        public static async Task<object> RunOne(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            dm1.JisCountry loObj = TestDataService_v1.SampleCountry_V1();
            string lsJson = string.Empty;

            lsJson = TestDataService_v1.SerializeObjectV1(loObj);
            log.LogInformation(lsJson);

            return new OkObjectResult(loObj);
        }

        [FunctionName("Test_Two")]
        [OpenApiOperation(operationId: "Test_Two", tags: new[] { "Tests - Manual" },
                                        Summary = "",
                                        Description = ""
                                        )]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The **Name** of user or application that calls this function.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Description = "The response container, with details of the processing outcome.")]
        public static async Task<dm2.IJisCountry> RunTwo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            dm2.JisCountry loObj = TestDataService_v2.SampleCountry_V2();
            string lsJson = string.Empty;

            lsJson = TestDataService_v1.SerializeObjectV1(loObj);
            log.LogInformation(lsJson);

            //return new OkObjectResult(loObj);
            return loObj;
        }

        [FunctionName("Test_Three")]
        [OpenApiOperation(operationId: "Test_Three", tags: new[] { "Tests - Manual" },
                                        Summary = "",
                                        Description = ""
                                        )]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The **Name** of user or application that calls this function.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(dmc3.ResultObject<object>), Description = "The response container, with details of the processing outcome.")]
        public static async Task<dmc3.ResultObject<dm3.JisCountry>> RunThree(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            dmc3.ResultObject<dm3.JisCountry> RO = null;
            RO = TestDataService_v3.SampleContainer_v3();

            string lsJson = string.Empty;
            lsJson = TestDataService_v1.SerializeObjectV1(RO);
            log.LogInformation(lsJson);

            //return new OkObjectResult(loObj);
            return RO;
        }

        [FunctionName("Test_Four")]
        [OpenApiOperation(operationId: "Test_Four", tags: new[] { "Tests - Manual" },
                                        Summary = "",
                                        Description = ""
                                        )]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The **Name** of user or application that calls this function.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(dmc4.IResultObject<dm4.IJisCountry>), Description = "The response container, with details of the processing outcome.")]
        public static async Task<dmc4.IResultObject<dm4.IJisCountry>> RunFour(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            dmc4.IResultObject<dm4.IJisCountry> RO = null;
            RO = TestDataService_v4.SampleContainer_v4();

            string lsJson = string.Empty;
            lsJson = TestDataService_v1.SerializeObjectV1(RO);
            log.LogInformation(lsJson);

            return RO;
        }

    }
}

