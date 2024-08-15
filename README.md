# Retry
A simple library for retrying and waiting for a condition to be True.

## Examples
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