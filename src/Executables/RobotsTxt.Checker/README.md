# RobotsTxt.Checker

[Nuget publish status](https://vsrm.dev.azure.com/dobryak/_apis/public/Release/badge/9156eb95-ee9c-447d-99c5-d1e42b69f230/2/2)

Executable to check robots.txt and check accessibility of sitemap.xml (if correct absolute link is added).

## Usage

Package is distributed as no-self-contained [nuget](https://www.nuget.org/packages/RobotsTxt.Checker/), which means, that .NET Core v.3.1 or *higher* (at least runtime) should be installed on your host.

On Windows x64 host - execute exe file `RobotsTxt.Checker.exe` with parameters.

On any other host - invoke dll via `dotnet` command with parameters. Example: ```dotnet RobotsTxt.Checker.dll --webappurls  test.com, test2.com```

## Parameters

1.  --webappurls        Required. Please, provide comma-separated list of web app root URLs, where robots.txt shall be checked. Example: https://www.google.com/, https://www.gmail.com/

1. --serverhostName     (optional) Host, where request should be sent. (curl example: curl https://www.test.com/robots.txt --header 'Host: somehost.to')

1.  --crawlingdenied    (Default: false) Define, if robots.txt shall deny all crawling of web app. Default value is false, e.g. crawling shall be allowed.

1.  --help              Display this help screen.

1.  --version           Display version information.