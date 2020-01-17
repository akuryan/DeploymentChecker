# DeploymentChecker
This repository is a result of work I have done for a long time as DevOps. Here I will collect release results checkers, which is must be executed on each deployment.

# Libraries

## RobotsTxt check

[![Build status](https://dev.azure.com/dobryak/NugetsAndExtensions/_apis/build/status/NuGet/RobotsTxt.Checker)](https://dev.azure.com/dobryak/NugetsAndExtensions/_build/latest?definitionId=10)

RobotsTxt check uses library [RobotsTxt](./src/Libraries) to check both robots.txt file and validate sitemap.xml.

It is built as executable file with [parameters](./src/Executables#robotstxtchecker) and will be packed as NuGet package.