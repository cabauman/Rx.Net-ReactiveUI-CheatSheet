# Rx Glossary

## Observable.FromAsync vs Observable.FromAsync vs Task.ToObservable

- FromAsync starts a new async operation for every subscription.
- StartAsync and ToObservable require an already running task.
- ToObservable doesn't support cancellation.
- FromAsync is basically `Observable.Defer(() => Observable.StartAsync(...))`
- One use for FromAsync is to control reentrancy for multiple calls to an async method.
- Concat ensures that there will be no overlapping in the execution of the tasks.

Source: https://github.com/dotnet/reactive/issues/459

## Subscribe overload that accepts a CancellationToken

```
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
Observable.Interval(TimeSpan.FromSeconds(0.5))
    .Subscribe(_ => Console.WriteLine(DateTime.UtcNow), cts.Token);
Thread.Sleep(10000);
```

Source: https://stackoverflow.com/a/35367449/5984310

## Subject.Synchronize

### Use case: Sharing a subject across multiple threads

- Avoid overlapping OnNext calls.
- By default, subjects do not perform any synchronization across threads.
- Hand out the sycnhronized subject to the producer threads.

```csharp
var synchronizedSubject = Subject.Synchronize(subject);
synchronizedSubject.OnNext(value);
subscription = synchronizedSubject
    .ObserveOn(TaskPoolScheduler.Default)
    .Subscribe(...);
```

## Async subscriptions

Subscribers are not supposed to be long running, and therefore don't support execution of long running async methods in the Subscribe handlers. Instead, consider your async method to be a single value observable sequence that takes a value from another sequence. Now you can compose sequences, which is what Rx was designed to do. Otherwise, 1. you break the error model 2. you are mixing async models (rx here, task there).

Source: https://stackoverflow.com/questions/37129159/subscribing-to-observable-sequence-with-async-function

## SerialDisposable

"We only want one sidebar open at a time"
"Autocomplete should only have one outstanding request in flight"
"Animate this value from here to there and make sure we cancel an already-running animation if we try to start it again"
"We only want to show one dialog on-screen at a time"
"Connect to this websocket but if someone issues another connect() request close the first one"

Source: https://twitter.com/anaisbetts/status/1034168666739200000

## Observable temperature

Refers to the state of the observable at the moment of its subscription. This state describes the time an observable begins and stops its emissions and whether the emissions are shared between observers.

- Convert cold to hot: Publish
- COnvert hot to cold: Defer

## Hot observable

An observable that emits notifications regardless of its observers (even if there are none). The notifications emitted by hot observables are shared among their observers.

A hot observable is in an active state, like a singer performing live.

## Cold observable

An observable that starts emitting notifications only when an observer subscribes, and each observer receives the full sequence of notifications without sharing them with other observers.

A cold observable is in a passive state, like an album waiting to be played.

## Subject

A type that implements the IObservable<T> interface and IObserver<M> interface is called a subject. This type acts as both an observer and an observable

Subject<T>: Broadcasts every observed notification to all observers.
AsyncSubject<T>: Represents an asynchronous operation that emits its value upon completion.
ReplaySubject<T>: Broadcasts notifications for current and future observers.
BehaviorSubject<T>: Broadcasts notifications and saves the latest value for future observers. When created, it’s initialized with a value that emits until changed.

### AsyncSubject

One problem with Subject<T> you may encounter is that if the source observable emits a value before an observer subscribes, this value will be lost. This is specifically problematic if the source always emits only a single notification. Luckily, AsyncSubject provides a remedy for those cases.

Internally, AsyncSubject stores the most recent value so that when the source observable completes, it emits this value to current and future observers.