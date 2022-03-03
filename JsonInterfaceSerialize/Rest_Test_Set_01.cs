using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using JsonInterfaceSerialize.DataModels.ModelsV4;
using JsonInterfaceSerialize.DataModels.ModelsV4.Containers;
using JsonInterfaceSerialize.Services;

namespace JsonInterfaceSerialize
{
    public static class Rest_Test_Set_01
    {
        /*
         * Full REST compliance not possible with function Apps
         * Function JsonInterfaceSerialize.RestTest01.RestTest01_GetOne and JsonInterfaceSerialize.RestTest01.RestTest01_GetAll have the same value for FunctionNameAttribute. Each function must have a unique name.
         * 
         * Workaround is a routing method to tie them all together
         * 
         * TODO: Refine return signatures of individual paths
         */
        #region REST compliance

        [FunctionName("Countries")]
        [OpenApiOperation(
            operationId: "Countries",
            tags: new[] { "Tests - REST Compliance" },
            Summary = "One to connect them all.",
            Description = "See documentation of individual Paths for required input"
            )]
        [OpenApiParameter("SyncStatus", Type = typeof(string), In = ParameterLocation.Query, Required = true, Summary = "true or false (default). Sets API StatusCode same as result container code.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Description = "The OK response")]
        public static async Task<object> RestTest01(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", "patch", "delete", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            switch (req.Method.ToLowerInvariant())
            {
                case "get":
                    // extract the route fragment and pass it back.
                    if (req.Query.ContainsKey("CountryName"))
                        return await RestTest01_GetOne(req, log);
                    else return await RestTest01_GetAll(req, log);
                case "post":
                    return await RestTest01_Create(req, log);
                case "patch":
                    return await RestTest01_Update(req, log);
                case "delete":
                    // extract the route fragment and pass it back.
                    return await RestTest01_Delete(req, log);
            }
            return null;
        }

        [FunctionName("CountriesAll")]
        [OpenApiOperation(
            operationId: "CountriesAll",
            tags: new[] { "Tests - REST Compliance" },
            Summary = "Get All",
            Description = ""
            )]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<IResultObject<IJisCountry>>), Description = "The OK response")]
        public static async Task<IResultObject<IList<IJisCountry>>> RestTest01_GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            return null;
        }

        [FunctionName("CountriesOne")]
        [OpenApiOperation(
            operationId: "CountriesOne",
            tags: new[] { "Tests - REST Compliance" },
            Summary = "Get One",
            Description = "Valid values for CountryName are 'India', 'null_value', 'throw_exception', ''"
            )]
        [OpenApiParameter("CountryName", Type = typeof(string), In = ParameterLocation.Query, Required = true, Summary = "Name of the country to get details of.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<IResultObject<IJisCountry>>), Description = "The OK response")]
        public static async Task<IResultObject<IJisCountry>> RestTest01_GetOne(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            // TODO: validate input
            string CountryName = req.Query["CountryName"].ToString();

            // TODO: Write proper logs
            IResultObject<IJisCountry> ARO = new ResultObject<IJisCountry>()
            {
                CorrelationId = System.Guid.NewGuid().ToString(),
            };
            IInternalResultObject<IJisCountry> IRO = null;
            try
            {
                IRO = await IFTestSVC_v1.Country_GetOne_v1(CountryName, log);
                ARO.IngestErrors(IRO?.Errors); // Test for null IRO

                ARO.Data = IRO?.Result;
                ARO.StatusCode = (IRO?.Result is null) ? HttpStatusCode.NoContent : HttpStatusCode.OK;
                ARO.Message = (IRO?.Result is null) ? HttpStatusCode.NoContent.ToString() : HttpStatusCode.OK.ToString();
                if (ARO.StatusCode == HttpStatusCode.NoContent) ARO.Message = $"No country found for given name '{CountryName}'";

            }
            catch (System.Exception se)
            {
                log.LogError(se, "Error getting country by name");
                ARO.StatusCode = HttpStatusCode.InternalServerError;
                ARO.Message = se.Message;
                ARO.Errors.Add(new Error { Type = ErrorTypes.ERROR, Message = "Error getting Country by name." });
            }

            // TODO: Abstract to a helper method - BEGIN

            req.HttpContext.Response.ContentType = "application/json";

            // Set container properties
            switch (ARO.StatusCode)
            {
                case HttpStatusCode.OK:
                    ARO.Successful = true;
                    break;
                case HttpStatusCode.NoContent:
                    ARO.Successful = true;
                    break;
                case HttpStatusCode.Unauthorized:
                    ARO.Successful = false;
                    break;
                case HttpStatusCode.InternalServerError:
                    ARO.Successful = false;
                    break;

                // Other codes, identify those that can be used to return better meaning
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.Processing:
                case HttpStatusCode.EarlyHints:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.MultiStatus:
                case HttpStatusCode.AlreadyReported:
                case HttpStatusCode.IMUsed:
                case HttpStatusCode.Ambiguous:
                case HttpStatusCode.Moved:
                case HttpStatusCode.Found:
                case HttpStatusCode.SeeOther:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.UseProxy:
                case HttpStatusCode.Unused:
                case HttpStatusCode.TemporaryRedirect:
                case HttpStatusCode.PermanentRedirect:
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.Gone:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestUriTooLong:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.MisdirectedRequest:
                case HttpStatusCode.UnprocessableEntity:
                case HttpStatusCode.Locked:
                case HttpStatusCode.FailedDependency:
                case HttpStatusCode.UpgradeRequired:
                case HttpStatusCode.PreconditionRequired:
                case HttpStatusCode.TooManyRequests:
                case HttpStatusCode.RequestHeaderFieldsTooLarge:
                case HttpStatusCode.UnavailableForLegalReasons:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.HttpVersionNotSupported:
                case HttpStatusCode.VariantAlsoNegotiates:
                case HttpStatusCode.InsufficientStorage:
                case HttpStatusCode.LoopDetected:
                case HttpStatusCode.NotExtended:
                case HttpStatusCode.NetworkAuthenticationRequired:
                default:
                    break;
            }

            // Root cause of HTTP 500 erors with no information in the container
            if (req.Query.ContainsKey("SyncStatus") && req.Query["SyncStatus"].ToString().ToLower() == "true")
            {
                req.HttpContext.Response.StatusCode = (int)ARO.StatusCode;
            }
            // TODO: Abstract to a helper method - FINIS

            return ARO;
        }


        [FunctionName("CountriesAdd")]
        [OpenApiOperation(
            operationId: "CountriesAdd",
            tags: new[] { "Tests - REST Compliance" },
            Summary = "Create",
            Description = ""
            )]
        [OpenApiRequestBody("application/json", typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<IResultObject<IJisCountry>>), Description = "The OK response")]
        public static async Task<IList<IResultObject<IJisCountry>>> RestTest01_Create(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            return null;
        }

        [FunctionName("CountriesUpdate")]
        [OpenApiOperation(
            operationId: "CountriesUpdate",
            tags: new[] { "Tests - REST Compliance" },
            Summary = "Update",
            Description = ""
            )]
        [OpenApiRequestBody("application/json", typeof(object))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<IResultObject<IJisCountry>>), Description = "The OK response")]
        public static async Task<IList<IResultObject<IJisCountry>>> RestTest01_Update(
            [HttpTrigger(AuthorizationLevel.Function, "patch", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            return null;
        }

        [FunctionName("CountriesDelete")]
        [OpenApiOperation(
            operationId: "CountriesDelete",
            tags: new[] { "Tests - REST Compliance" },
            Summary = "Delete",
            Description = ""
            )]
        [OpenApiParameter("CountryName", Type = typeof(string), In = ParameterLocation.Query, Required = true, Summary = "Name of the country to get details of.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<IResultObject<IJisCountry>>), Description = "The OK response")]
        public static async Task<IList<IResultObject<IJisCountry>>> RestTest01_Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req,
            ILogger log
            )
        {
            return null;
        }


        #endregion //REST compliance

        // Default Code




        [FunctionName("RestTest01")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Miscellaneous" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> RestTest01_01(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }


}

