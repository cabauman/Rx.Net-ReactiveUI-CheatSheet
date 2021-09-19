# Rx Glossary

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

Subject<T>—Broadcasts every observed notification to all observers.
AsyncSubject<T>—Represents an asynchronous operation that emits its value upon completion.
ReplaySubject<T>—Broadcasts notifications for current and future observers.
BehaviorSubject<T>—Broadcasts notifications and saves the latest value for future observers. When created, it’s initialized with a value that emits until changed.

### AsyncSubject

One problem with Subject<T> you may encounter is that if the source observable emits a value before an observer subscribes, this value will be lost. This is specifically problematic if the source always emits only a single notification. Luckily, AsyncSubject provides a remedy for those cases.

Internally, AsyncSubject stores the most recent value so that when the source observable completes, it emits this value to current and future observers.