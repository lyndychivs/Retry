# Retry
A simple library for retrying and waiting for a condition to be True.

## Usage Example
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

## Tests
All Unit Tests can be found under the [Retry.Tests](https://github.com/lyndychivs/Retry/tree/master/Retry.Tests) namesapce.

## Package
Available on:
- [GitHub Packages - lyndychivs.Retry](https://github.com/lyndychivs/Retry/pkgs/nuget/lyndychivs.Retry)
- [Nuget Packages - lyndychivs.Retry](https://www.nuget.org/packages/lyndychivs.Retry/)