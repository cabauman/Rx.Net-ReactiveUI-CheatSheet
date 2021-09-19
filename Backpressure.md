# Backpressure

Strategies for coping with Observables that produce items more rapidly than their observers consume them

In ReactiveX it is not difficult to get into a situation in which an Observable is emitting items more rapidly than an operator or observer can consume them. This presents the problem of what to do with such a growing backlog of unconsumed items.

For example, imagine using the Zip operator to zip together two infinite Observables, one of which emits items twice as frequently as the other. A naive implementation of the operator would have to maintain an ever-expanding buffer of items emitted by the faster Observable to eventually combine with items emitted by the slower one. This could cause ReactiveX to seize an unwieldy amount of system resources.

There are a variety of strategies with which you can exercise flow control and backpressure in ReactiveX in order to alleviate the problems caused when a quickly-producing Observable meets a slow-consuming observer, which include, in some ReactiveX implementations, reactive pull backpressure and some backpressure-specific operators.

A cold Observable emits a particular sequence of items, but can begin emitting this sequence when its observer finds it to be convenient, and at whatever rate the observer desires, without disrupting the integrity of the sequence. For example if you convert a static iterable into an Observable, that Observable will emit the same sequence of items no matter when it is later subscribed to or how frequently those items are observed. Examples of items emitted by a cold Observable might include the results of a database query, file retrieval, or web request.

A hot Observable begins generating items to emit immediately when it is created. Subscribers typically begin observing the sequence of items emitted by a hot Observable from somewhere in the middle of the sequence, beginning with the first item emitted by the Observable subsequent to the establishment of the subscription. Such an Observable emits items at its own pace, and it is up to its observers to keep up. Examples of items emitted by a hot Observable might include mouse & keyboard events, system events, or stock prices.

When a cold Observable is multicast (when it is converted into a connectable Observable and its Connect method is called), it effectively becomes hot and for the purposes of backpressure and flow-control it should be treated as a hot Observable.

Cold Observables are ideal for the reactive pull model of backpressure implemented by some implementations of ReactiveX (which is described elsewhere). Hot Observables typically do not cope well with a reactive pull model, and are better candidates for other flow control strategies, such as the use of the operators described on this page, or operators like Buffer, Sample, Debounce, or Window.