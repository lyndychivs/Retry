# Retry
A simple library for retrying and waiting for a condition to be True.

```csharp
var maxWaitTime = TimeSpan.FromSeconds(10);
var pollingInterval = TimeSpan.FromSeconds(1);

var retry = new Retry(maxWaitTime, pollingInterval);

retry.Until(() =>
{
    bool result = DoSomething();
    return result; // if true, the loop will break
});
```