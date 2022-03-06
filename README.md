# JsonInterfaceSerialize
Proof of Concept to demonstrate serialization of Interface based objects to JSON in Azure Function Apps.

## Getting Started

## Environment setup
- Create local.settings.json

### local.settings.json
JsonInterfaceSerialize\local.settings.json

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet"
    }
}
```