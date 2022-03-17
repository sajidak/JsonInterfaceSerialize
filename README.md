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

### Dependencies
> Extracted from JsonInterfaceSerialize\obj\project.assets.json
- Any extension or dependency should not depend on versions greater than
    ```json
    "System.Threading.Tasks.Extensions": "4.5.3"
    ```

####  Moq
> Dependency table

| Moq Version | Depencency | Last updated |
| ----------- | ---------- | ------------ |
| 4.15.1      | >= 4.5.1   | 11/10/2020   |
| 4.15.2      | >= 4.5.4   | 11/26/2020   |

- System.Threading.Tasks.Extensions
    - Version 4.5.3
    - Last updated 7/9/2019
    - Add as dependency to ensure version lock with Azure Function Apps projects
    - Add before adding Moq
    - https://www.nuget.org/packages/System.Threading.Tasks.Extensions/4.5.3


    
***

## Links of Interest
- https://github.com/maxkagamine/Moq.Contrib.HttpClient
- https://github.com/dwhelan/Moq-Sequences
- https://github.com/moq/Moq.AutoMocker


***

## Copyright
Copyright 2022 Sajid Ali Khan

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

***
