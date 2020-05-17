# Rx.Net CheatSheet
A collection of links and snippets to resources, samples, answers, people, videos, etc. related to Rx.Net.

_Most of the blogs and videos are pretty dated but the core concepts still apply. The overall public API really hasn't changed much over the years, except for new operators and performance improvements._

## Table of Contents

#### [Timeline](#timeline-1)
#### [Personal Favorites](#personal-favorites-1)
#### [Learning Resources](#learning-resources-1)
#### [Videos](#videos-1)
#### [Tips](#tips-1)
#### [Convenient References](#convenient-references-1)
#### [Notable People to Follow](#notable-people-to-follow-1)

## Timeline

Core Rx team at Microsoft: Erik Meijer, Brian Beckman, Wes Dyer, Bart De Smet, Jeffrey Van Gogh, and Matthew Podwysocki.

[First preview release](https://docs.microsoft.com/en-us/archive/blogs/rxteam/announcing-reactive-extensions-rx-for-net-silverlight): 11/17/2009

[First stable release](https://docs.microsoft.com/en-us/archive/blogs/rxteam/reactive-extensions-v1-0-stable-and-v1-1-experimental-available-now): 06/29/2011

[Open Sourced](https://docs.microsoft.com/en-us/archive/blogs/interoperability/ms-open-tech-open-sources-rx-reactive-extensions-a-cure-for-asynchronous-data-streams-in-cloud-programming): 11/06/2012

*Google searching tip:* Because there are so many languages that have adopted Rx, there's a need to filter the results to dotnet. To achieve this, I always strucutre my query as "C# observable [my specific query]".

## Learning Resources

[Github Repo](https://github.com/dotnet/reactive)

[Rx.Net Slack](https://reactivex.slack.com/messages/rxnet/)

[IntroToRx](http://www.introtorx.com) - If I had to name a single resource to help someone get started with Rx.Net, it would be this invaluable online book by Lee Campbell. Even after years of reactive programming, I find myself going back time and time again as a reference/reminder of some of the smaller details.

[Book: Rx.Net in Action](https://livebook.manning.com/book/rx-dot-net-in-action/about-this-book/)

[Book: Programming Reactive Extensions and LINQ](https://www.apress.com/gp/book/9781430237471)

[RxCookbook](https://github.com/LeeCampbell/RxCookbook) - Useful for learning how to utilize the power of Rx in common C# application scenarios, such as INotifyPropertyChanged and logging.

[Bart De Smet Blog](http://blogs.bartdesmet.net/blogs/bart/archive/tags/Rx/default.aspx)

[RxViz](https://rxviz.com/) - interactive JavaScript with visualizations (I just wish a DotNet version of this existed)

[RxMarbles](http://rxmarbles.com/) - interactive visualizations

[ReactiveX.io](http://reactivex.io/) - Rx hub for all languages

[101 Rx Samples](http://rxwiki.wikidot.com/101samples)

## Videos

[Channel9 Videos, filtered by Rx](https://channel9.msdn.com/tags/Rx/)

[Channel9 Videos, filtered by Bart De Smit (one of the creators)](https://channel9.msdn.com/Tags/bart+de+smet)

[Reactive Extensions for .NET Developers with Michael Stonis (November 2018)](https://channel9.msdn.com/Shows/On-NET/Reactive-Extensions-for-NET-Developers?WT.mc_id=ondotnet-channel9-cephilli)

[Why You Should Be Building Better Mobile Apps with Reactive Programming – Michael Stonis](https://www.youtube.com/watch?v=DYEbUF4xs1Q)

[UniRx playlist, but also includes intro to Rx theory and operators](https://www.youtube.com/playlist?list=PLKERDLXpXl_gdZ7NAHkAxKf12g3oNjTAc)

## Tips

### Best practices
* Members that return a sequence should never return null. This applies to IEnumerable<T> and IObservable<T> sequences. Return an empty sequence instead.
* Dispose of subscriptions.
* Subscriptions should match their scope.
* Always provide an OnError handler.
* Avoid breaking the monad with blocking operators such as First, FirstOrDefault, Last, LastOrDefault, Single, SingleOrDefault and ForEach.
* Avoid switching between monads, i.e. going from IObservable<T> to IEnumerable<T> and back to IObservable<T>.
* Favor lazy evaluation over eager evaluation.
* Break large queries up into parts. Key indicators of a large query:
  * nesting
  * over 10 lines of query comprehension syntax
  * using the into statement
* Name your queries well, i.e. avoid using the names like query, q, xs, ys, subject etc.
* Avoid creating side effects. If you must create side effects, be explicit by using the Do operator.
* **Minimize*** the use of the subject types. Rx is effectively a functional programming paradigm. Using subjects means we are now managing state, which is potentially mutating. Dealing with both mutating state and asynchronous programming at the same time is very hard to get right. Furthermore, many of the operators (extension methods) have been carefully written to ensure that correct and consistent lifetime of subscriptions and sequences is maintained; when you introduce subjects, you can break this. Future releases may also see significant performance degradation if you explicitly use subjects.
* Avoid creating your own implementations of the IObservable<T> interface. Favor using the Observable.Create factory method overloads instead.
* Avoid creating your own implementations of the IObserver<T> interface. Favor using the Subscribe extension method overloads instead.
* The subscriber should define the concurrency model. The SubscribeOn and ObserveOn operators should only ever precede a Subscribe method.

[Source](http://introtorx.com/Content/v1.0.10621.0/18_UsageGuidelines.html#UsageGuidelines)

*If you check the source link above, you'll see I modified this point to say *Minimize* rather than *Avoid*. I did this because the opinions of some of the most respected Rx users on the web slightly differ when it comes to the degree of how much Subjects should be avoided. Here are some snippets of those opinions:

> [Lee Campbell](https://stackoverflow.com/a/14460634/5984310): The reason I really don't like Subjects, is that is usually a case of the developer not really having a clear design on the problem. Hack in a subject, poke it here there and everywhere, and then let the poor support dev guess at WTF was going on. When you use the Create/Generate etc methods you are localizing the effects on the sequence. You can see it all in one method and you know no-one else is throwing in a nasty side effect. If I see a subject fields I now have to go looking for all the places in a class it is being used. If some MFer exposes one publicly, then all bets are off, who knows how this sequence is being used! Async/Concurrency/Rx is hard. You don't need to make it harder by allowing side effects and causality programming to spin your head even more.

> [Sergey Aldoukhov](https://stackoverflow.com/q/9299813/5984310): People who frequent the RX forum know that E.Meijer does not like Subjects. While I have a deepest respect to RX creator's opinion, I have been using Subjects quite extensively in multiple projects for a couple of years and haven't had any architectural problem or a bug because of them.<br />
[Ana Betts](https://stackoverflow.com/a/9301923/5984310): Erik Meijer is thinking in a purely functional way - Subjects are the mutable variables of Rx. So, in general usage he's right - using Subjects is sometimes a way to cop out of Thinking Functionally, and if you use them too much, you're trying to row upstream. However! Subject are extremely useful when you're interfacing with the non-Functional world of .NET. Wrapping an event or callback method? Subjects are great for that. Trying to put an Rx "interface" onto some existing code? Use a Subject!

> [Bart De Smet](https://stackoverflow.com/a/12049997/5984310): Whenever you need to create an observable out of thin air, Observable.Create should be the first thing to think of. in a lot of cases, there's already a built-in primitive in Rx that does exactly what you need. For example, there's From* factory methods to bridge with existing concepts (such as events, tasks, asynchronous methods, enumerable sequence), some of which using a subject under the covers. For multicasting logic, there's the Publish, Replay, etc. family of operators.

> [James World](https://stackoverflow.com/a/21824601/5984310): It's not so much that the use of Subject<T> is bad - there has to be some way of "entering the monad" - that's the academic way of saying "get an IObservable<T>". You need to start somewhere. The problem with Subject<T> arises more when it's used from a subscription instead of chaining existing observables together. Subjects should just exist at the edges of your Rx machinery. If none of the provided entry points (e.g. FromEvent, FromEventPattern, FromAsync, Return, ToObservable and so on) work for you then using Subject<T> is perfectly valid. And there's no need to add extra complexity just to facilitate using one of the above - most of them use subjects or subject-like constructs under the covers anyway.

> [Enigmativity](https://stackoverflow.com/a/45979673/5984310): It's usually a good idea to avoid subjects.

> [Shlomo](https://stackoverflow.com/a/45966519/5984310): Subjects aren't universally bad.

## Convenient References

### Scheduler Defaults

[SchedulerDefaults](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Concurrency/SchedulerDefaults.cs)

AsyncConversions: [Start, ToAsync, FromAsyncPattern](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Async.cs)

ConstantTimeOperations: [Return, Throw](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Creation.cs), [Append, Prepend, StartWith](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Single.cs)

SynchronizationContextScheduler/ConstantTimeOperations: [FromEvent, FromEventPattern](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Events.cs)

Iteration: [Generate, Range, Repeat](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Creation.cs), [TakeLast](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Time.cs#L331), [ToObservable](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Conversions.cs#L81), [ReplaySubject<T>](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Subjects/ReplaySubject.cs)

TailRecursion: At the time of writing, I don't see this scheduler being used in the source code.

TimeBasedOperations: [Buffer, Delay, DelaySubscription, Generate, Interval, Sample, Skip, SkipLast, SkipUntil, Take, TakeLast, TakeLastBuffer, TakeUntil, Throttle, TimeInterval, Timeout, Timer, Timestamp, Window](https://github.com/dotnet/reactive/blob/7ad606b3dcd4bb2c6ae9622f8a59db7f8f52aa85/Rx.NET/Source/src/System.Reactive/Linq/QueryLanguage.Time.cs)

[Useful advice for choosing schedulers](https://stackoverflow.com/a/20636059/5984310)

## Notable People to Follow

_These are basically the names you'll see over and over again on Stack Overflow, answering Rx.Net questions. I've learned a ton from these guys.

James World [@jamesw0rld](https://twitter.com/jamesw0rld)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:87427+[system.reactive])
* [Github](https://github.com/james-world)
* [Blog](http://www.zerobugbuild.com/)

Lee Campbell [@LeeRyanCampbell](https://twitter.com/leeryancampbell)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:393615+[system.reactive])

Shlomo
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:415661+[system.reactive])

Enigmativity
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:259769+[system.reactive])

Paul Betts [@paulcbetts](https://twitter.com/paulcbetts)
* [Github](https://github.com/paulcbetts)
* [Book: Programming Reactive Extensions and LINQ](http://disq.us/url?url=http%3A%2F%2Fjliberty.me%2F2lYAZb8%3AI-DqcbqqiSnKie2L8J8U2hBCoS8&cuid=3716375)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:5728+[system.reactive])
* [Stack Overflow - ReactiveUI](https://stackoverflow.com/search?q=user:5728+[reactiveui])

Brandon
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:674326+[system.reactive])

Dave Sexton [@IDaveSexton](https://twitter.com/idavesexton)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:3970148+[system.reactive])

[Stack Overflow Top Users for System.Reactive](https://stackoverflow.com/tags/system.reactive/topusers)
