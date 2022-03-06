using JsonInterfaceSerialize.DataModels.DataInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace JsonInterfaceSerialize.Utilities.Helpers
{
    public class APIResponsePolisher
    {
        public static bool PolishGenericResponse<T>(ref IResultObject<T> ro, ref HttpRequest req, ILogger log)
        {
            bool lbOK = true;
            try
            {
                // Set Response properties
                req.HttpContext.Response.ContentType = "application/json";
                // Set container properties
                ro.Successful = false;
                ro.Message = ro.StatusCode.ToString();
                switch (ro.StatusCode)
                {
                    case HttpStatusCode.OK:
                        ro.Successful = true;
                        break;
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.InternalServerError:
                        break;
                    default:
                        ro.Message = $"Unknown condition. Inform application owner with details.";
                        break;
                }
            }
            catch (Exception se)
            {
                lbOK = false;
                string lsErrData = ExceptionHelpers.SerializeExceptionTxt(se, $"Errored polishing Response Object.");
                log.LogError(lsErrData);
            }
            return lbOK;
        }
    }
}
