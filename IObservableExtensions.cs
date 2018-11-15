using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ReactiveUI;

namespace Extensions
{
    public static class IObservableExtensions
    {
        /// <summary>
        /// Convenience method for Where(x => x != null).
        /// </summary>
        /// <remarks>
        // Credit: Kent Boogaart
        /// </remarks>
        public static IObservable<T> WhereNotNull<T>(this IObservable<T> @this)
        {
            return @this.Where(x => x != null);
        }

        /// <summary>
        /// Convenience method for Select(_ => Unit.Default).
        /// </summary>
        /// <remarks>
        // Credit: Kent Boogaart
        /// </remarks>
        public static IObservable<Unit> ToSignal<T>(this IObservable<T> @this)
        {
            return @this.Select(_ => Unit.Default);
        }

        /// <summary>
        /// Allows user to invoke actions upon subscription and disposal.
        /// </summary>
        /// <remarks>
        // Credit: Kent Boogaart
        // https://github.com/kentcb/YouIandReactiveUI
        /// </remarks>
        public static IObservable<T> DoLifetime<T>(this IObservable<T> @this, Action subscribed, Action unsubscribed)
        {
            return Observable
                .Create<T>(
                    observer =>
                    {
                        subscribed();

                        var disposables = new CompositeDisposable();
                        @this
                            .Subscribe(observer)
                            .DisposeWith(disposables);
                        Disposable
                            .Create(() => unsubscribed())
                            .DisposeWith(disposables);

                        return disposables;
                    });
        }

        /// <summary>
        /// Subscribes to the given observable and provides source code info about the caller when an
        /// exception is thrown, without the user needing to supply an onError handler.
        /// </summary>
        /// <remarks>
        // Credit: Kent Boogaart
        /// </remarks>
        public static IDisposable SubscribeSafe<T>(
            this IObservable<T> @this,
            [CallerMemberName]string callerMemberName = null,
            [CallerFilePath]string callerFilePath = null,
            [CallerLineNumber]int callerLineNumber = 0)
        {
            return @this
                .Subscribe(
                    _ => { },
                    ex =>
                    {
                        // Replace with your logger library.
                        Console.Error.WriteLine(
                            "An exception went unhandled: {0}" +
                            "Caller member name: {1}, " +
                            "caller file path: {2}, " +
                            "caller line number: {3}.",
                            ex,
                            callerMemberName,
                            callerFilePath,
                            callerLineNumber);

                        // Delete this line if you're not using ReactiveUI.
                        RxApp.DefaultExceptionHandler.OnNext(ex);
                    });
        }

        /// <summary>
        /// Subscribes to the given observable and provides source code info about the caller when an
        /// exception is thrown, without the user needing to supply an onError handler.
        /// </summary>
        /// <remarks>
        // Credit: Kent Boogaart
        /// </remarks>
        public static IDisposable SubscribeSafe<T>(
            this IObservable<T> @this,
            Action<T> onNext,
            [CallerMemberName]string callerMemberName = null,
            [CallerFilePath]string callerFilePath = null,
            [CallerLineNumber]int callerLineNumber = 0)
        {
            return @this
                .Subscribe(
                    onNext,
                    ex =>
                    {
                        // Replace with your logger library.
                        Console.Error.WriteLine(
                            "An exception went unhandled: {0}" +
                            "Caller member name: {1}, " +
                            "caller file path: {2}, " +
                            "caller line number: {3}.",
                            ex,
                            callerMemberName,
                            callerFilePath,
                            callerLineNumber);

                        // Delete this line if you're not using ReactiveUI.
                        RxApp.DefaultExceptionHandler.OnNext(ex);
                    });
        }

        /// <summary>
        /// Allows the user to perform an action based on the current and previous emitted items.
        /// </summary>
        /// <remarks>
        // Credit: James World
        // http://www.zerobugbuild.com/?p=213
        /// </remarks>
        public static IObservable<T> WithPrevious<T>(this IObservable<T> @this, Func<T, T, T> projection)
        {
            return @this
                .Scan(
                    Tuple.Create(default(T), default(T)),
                    (acc, current) => Tuple.Create(acc.Item2, current))
                .Select(t => projection(t.Item1, t.Item2));
        }

        /// <summary>
        /// Limits the rate at which events arrive from an Rx stream.
        /// </summary>
        /// <remarks>
        // Credit: James World
        // http://www.zerobugbuild.com/?p=323
        /// </remarks>
        public static IObservable<T> LimitRate<T>(this IObservable<T> @this, TimeSpan interval)
        {
            return @this
                .Select(
                    x =>
                    {
                        return Observable
                            .Empty<T>()
                            .Delay(interval)
                            .StartWith(x);
                    })
                .Concat();
        }

        /// <summary>
        /// Prints a detailed log of what your Rx query is doing.
        /// </summary>
        /// <remarks>
        // Credit: James World
        // https://stackoverflow.com/questions/20220755/how-can-i-see-what-my-reactive-extensions-query-is-doing
        /// </remarks>
        public static IObservable<T> Spy<T>(this IObservable<T> @this, string opName = null)
        {
            opName = opName ?? "IObservable";
            Console.WriteLine("{0}: Observable obtained on Thread: {1}", opName, Thread.CurrentThread.ManagedThreadId);

            return Observable.Create<T>(
                obs =>
                {
                    Console.WriteLine("{0}: Subscribed to on Thread: {1}", opName, Thread.CurrentThread.ManagedThreadId);

                    try
                    {
                        var subscription = @this
                            .Do(
                                x => Console.WriteLine("{0}: OnNext({1}) on Thread: {2}", opName, x, Thread.CurrentThread.ManagedThreadId),
                                ex => Console.WriteLine("{0}: OnError({1}) on Thread: {2}", opName, ex, Thread.CurrentThread.ManagedThreadId),
                                () => Console.WriteLine("{0}: OnCompleted() on Thread: {1}", opName, Thread.CurrentThread.ManagedThreadId))
                            .Subscribe(obs);
                            
                        return new CompositeDisposable(
                            subscription,
                            Disposable.Create(() => Console.WriteLine("{0}: Cleaned up on Thread: {1}", opName, Thread.CurrentThread.ManagedThreadId)));
                    }
                    finally
                    {
                        Console.WriteLine("{0}: Subscription completed.", opName);
                    }
                });
        }

        /// <summary>
        /// Prints the provided name next to stream emissions (useful for debugging).
        /// </summary>
        /// <remarks>
        // Credit: Lee Campbell
        // http://www.introtorx.com/Content/v1.0.10621.0/07_Aggregation.html#Aggregation
        /// </remarks>
        public static void Dump<T>(this IObservable<T> @this, string name)
        {
            @this.Subscribe(
            i => Console.WriteLine("{0}-->{1}", name, i),
            ex => Console.WriteLine("{0} failed-->{1}", name, ex.Message),
            () => Console.WriteLine("{0} completed", name));
        }
    }
}
