using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace RxCheatsheet
{
    public static class IObservableUtil
    {
        /// <summary>
        /// Merges observables of any type into a single IObservable<Unit>.
        /// </summary>
        public static IObservable<Unit> MergeSignals<T1, T2>(
            IObservable<T1> o1,
            IObservable<T2> o2)
        {
            return Observable.Merge(
                o1.Select(_ => Unit.Default),
                o2.Select(_ => Unit.Default));
        }

        /// <summary>
        /// Merges observables of any type into a single IObservable<Unit>.
        /// </summary>
        public static IObservable<Unit> MergeSignals<T1, T2, T3>(
            IObservable<T1> o1,
            IObservable<T2> o2,
            IObservable<T3> o3)
        {
            return Observable.Merge(
                o1.Select(_ => Unit.Default),
                o2.Select(_ => Unit.Default),
                o3.Select(_ => Unit.Default));
        }

        /// <summary>
        /// Merges observables of any type into a single IObservable<Unit>.
        /// </summary>
        public static IObservable<Unit> MergeSignals<T1, T2, T3, T4>(
            IObservable<T1> o1,
            IObservable<T2> o2,
            IObservable<T3> o3,
            IObservable<T4> o4)
        {
            return Observable.Merge(
                o1.Select(_ => Unit.Default),
                o2.Select(_ => Unit.Default),
                o3.Select(_ => Unit.Default),
                o4.Select(_ => Unit.Default));
        }

        /// <summary>
        /// Merges observables of any type into a single IObservable<Unit>.
        /// </summary>
        public static IObservable<Unit> MergeSignals<T1, T2, T3, T4, T5>(
            IObservable<T1> o1,
            IObservable<T2> o2,
            IObservable<T3> o3,
            IObservable<T4> o4,
            IObservable<T5> o5)
        {
            return Observable.Merge(
                o1.Select(_ => Unit.Default),
                o2.Select(_ => Unit.Default),
                o3.Select(_ => Unit.Default),
                o4.Select(_ => Unit.Default),
                o5.Select(_ => Unit.Default));
        }

        /// <summary>
        /// Merges observables of any type into a single IObservable<Unit>.
        /// </summary>
        public static IObservable<Unit> MergeSignals<T1, T2, T3, T4, T5, T6>(
            IObservable<T1> o1,
            IObservable<T2> o2,
            IObservable<T3> o3,
            IObservable<T4> o4,
            IObservable<T5> o5,
            IObservable<T6> o6)
        {
            return Observable.Merge(
                o1.Select(_ => Unit.Default),
                o2.Select(_ => Unit.Default),
                o3.Select(_ => Unit.Default),
                o4.Select(_ => Unit.Default),
                o5.Select(_ => Unit.Default),
                o6.Select(_ => Unit.Default));
        }
    }
}
