[![Mutation testing badge](https://img.shields.io/endpoint?style=for-the-badge&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Flyndychivs%2FRetry%2Fmaster)](https://dashboard.stryker-mutator.io/reports/github.com/lyndychivs/Retry/master)
![NuGet Downloads](https://img.shields.io/nuget/dt/lyndychivs.Retry?style=for-the-badge&logo=nuget&link=https%3A%2F%2Fgithub.com%2Flyndychivs%2FRetry%2Fpkgs%2Fnuget%2Flyndychivs.Retry)

## lyndychivs.Retry
A simple library for retrying and waiting for a condition to be True.

### Example
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

### Tests
All Unit Tests can be found under the [Retry.Tests](https://github.com/lyndychivs/Retry/tree/master/Retry.Tests) namesapce.

### Package
Available on:
- [GitHub Packages - lyndychivs.Retry](https://github.com/lyndychivs/Retry/pkgs/nuget/lyndychivs.Retry)
- [Nuget Packages - lyndychivs.Retry](https://www.nuget.org/packages/lyndychivs.Retry/)
