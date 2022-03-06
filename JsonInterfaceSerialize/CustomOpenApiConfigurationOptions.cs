using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace JsonInterfaceSerialize
{
    public class CustomOpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiVersionType OpenApiVersion { get => OpenApiVersionType.V3; }
        public override OpenApiInfo Info
        {
            get
            {
                OpenApiInfo loOAI = base.Info;
                loOAI.Version = "0.0.1.0";
                loOAI.Title = "JIS - Json Interface Serialize Testbed";
                loOAI.Description = "Sample application to demonstrate usage of interface in Azure Function Apps with uUnit and  Moq + Moq.AutoMock.";
                loOAI.Contact = new OpenApiContact() { Name = "sak", Email = "sak.fsap@outlook.com" };
                return loOAI;
            }
            set => base.Info = value;
        }
    }
}
