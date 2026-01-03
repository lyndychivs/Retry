[![Build & Test](https://github.com/lyndychivs/Retry/actions/workflows/build-test.yml/badge.svg?branch=main)](https://github.com/lyndychivs/Retry/actions/workflows/build-test.yml)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Flyndychivs%2FRetry%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/lyndychivs/Retry/main)
[![NuGet Downloads](https://img.shields.io/nuget/dt/lyndychivs.Retry?style=flat&logo=nuget)](https://www.nuget.org/packages/lyndychivs.Retry/)

# lyndychivs.Retry
A simple library for retrying and waiting for a condition to be True.

## Example
```csharp
var maxWaitTime = TimeSpan.FromSeconds(10);
var pollingInterval = TimeSpan.FromSeconds(1);

var retry = new Retry(maxWaitTime, pollingInterval);

retry.Until(DoSomething);

bool DoSomething()
{
    // do something
    // return true if successful
    // return false if not successful

    return true;
}
```

> Also supports Async context.

## Tests
All Unit Tests can be found under the [Retry.Tests](https://github.com/lyndychivs/Retry/tree/main/Retry.Tests) namesapce.

## Package
Available on:
- [GitHub Packages - lyndychivs.Retry](https://github.com/lyndychivs/Retry/pkgs/nuget/lyndychivs.Retry)
- [Nuget Packages - lyndychivs.Retry](https://www.nuget.org/packages/lyndychivs.Retry/)
