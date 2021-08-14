#ReactiveUI CheatSheet
A collection of links and snippets to resources, samples, answers, people, videos, etc. related to ReactiveUI.

## Table of Contents

#### [Sample Projects](#sample-projects-1)
#### [Learning Resources](#learning-resources-1)
#### [Tips & Best Practices](#tips--best-practices-1)
#### [Notable People to Follow](#notable-people-to-follow-1)
#### [ReactiveUI Glossary](#reactiveui-glossary-1)

## Sample Projects

_Most of these are pretty dated but still have a lot of relevance._

A lot of these are taken from [this thread](https://github.com/reactiveui/ReactiveUI/issues/687), but I find it easier to browse when it's organized by platform, like this.

### Xamarin.Forms

[WorkoutWotch - Xamarin Forms Video Series](https://github.com/kentcb/WorkoutWotch) - The accompanying video series, referenced in the README, covers Xamarin iOS Native, but the codebase was later converted to Xamarin Forms to support other platforms.

[Reactive Examples by TheEightBot](https://github.com/TheEightBot/Reactive-Examples)

[UnofficialGitterApp](https://github.com/flagbug/UnofficialGitterApp)

[XamarinEvolve2014 - Heavily commented demo app by Paul Betts](https://github.com/paulcbetts/XamarinEvolve2014)

### Android

[Burger ingredient streams demo with blog post](https://github.com/JonDouglas/BeingReactive)

[Espera.Mobile](https://github.com/flagbug/Espera.Mobile)

### iOS

[WorkoutWotch - Xamarin Forms Video Series](https://github.com/kentcb/WorkoutWotch) - The accompanying video series, referenced in the README, covers Xamarin iOS Native, but the codebase was later converted to Xamarin Forms to support other platforms.

[CodeHub - uses MVVMCross](https://github.com/CodeHubApp/CodeHub)

[ReactiveTableView & ReactiveCollectionView Demo](https://github.com/cabauman/ReactiveTableViewSource-Sample)

### WPF

[Sample code for the book "You, I, and ReactiveUI"](https://github.com/kentcb/YouIandReactiveUI)

[FirstsStepsRUI with blog post](https://github.com/kondaskondas/FirstsStepsRUI)

### UWP

[RxUI-UWP-Sample](https://github.com/moswald/RxUI-UWP-Sample)

## Learning Resources

[Github Repo](https://github.com/reactiveui/ReactiveUI)

[ReactiveUI Slack](https://reactivex.slack.com/messages/reactiveui/)

[ReactiveUI Docs](https://reactiveui.net)

[Book: You, I, and ReactiveUI](https://kent-boogaart.com/you-i-and-reactiveui/) - Love this book. Highly recommended.

[Custom Routing in ReactiveUI](https://kent-boogaart.com/blog/custom-routing-in-reactiveui)

## Videos

[Reactive Extensions for .NET Developers with Michael Stonis (November 2018)](https://channel9.msdn.com/Shows/On-NET/Reactive-Extensions-for-NET-Developers?WT.mc_id=ondotnet-channel9-cephilli)

[Why You Should Be Building Better Mobile Apps with Reactive Programming – Michael Stonis](https://www.youtube.com/watch?v=DYEbUF4xs1Q)

## Tips & Best Practices

### Inject Service Locator interface via constructor

```
public SuspensionHost(ISuspensionDriver driver = null)
{
    driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();
}
```

**Explanation:** This uses a Service Located interface for the default interface, but only if the caller didn't give an explicit one in the constructor. Far more straightforward to test in a unit test runner than trying to construct a sham IoC container, but still falls back to a default implementation at runtime.

**Source:** [https://stackoverflow.com/a/26924067/5984310](https://stackoverflow.com/a/26924067/5984310)

### Call async operations in the View constructor, rather than the ViewModel constructor.

```
this.WhenAnyValue(x => x.ViewModel.LoadItems)
    .SelectMany(x => x.ExecuteAsync())
    .Subscribe();
```

**Explanation:** Invoking async operations in the ViewModel constructor means that your ViewModel class becomes more difficult to test, because you always have to mock out the effects of calling LoadItems, even if the thing you are testing is unrelated.

**Source:** [https://codereview.stackexchange.com/a/74793](https://codereview.stackexchange.com/a/74793)

### When should I bother disposing of IDisposable objects?

1) No need

```
public MyViewModel()
{
    MyReactiveCommand
        .Execute()
        .Subscribe(...);
}
```

Quote by Kent Boogart (one of the ReactiveUI maintainers):

> When the execution of a ReactiveCommand completes, all observers are auto-unsubscribed anyway. Generally, subscriptions to pipelines that have a finite lifetime (eg. via a timeout) need not be disposed manually. Disposing of such a subscription is about as useful as disposing of a MemoryStream.

2) Do dispose

```
public MyView()
{
    this.WhenAnyValue(x => x.ViewModel)
        .Do(PopulateFromViewModel)
        .Subscribe();
}
```

This one is tricky. Disposing of this subscription is a must _if_ developing for a dependency property-based platform such as WPF or UWP. Quoting Paul Betts, this is because "[there's no non-leaky way to observe a dependency property](https://stackoverflow.com/a/22341350/5984310)," which is exactly what the ViewModel property of a ReactiveUserControl is. However, if you happen to know that your ViewModel won't change for the liftime of the view then you can make ViewModel a normal property, eliminating the need to dispose. For other platforms such as Xamarin.Forms, Xamarin.Android, and Xamarin.iOS there's no need to dispose because you're simply monitoring the property (ViewModel) on the view itself, so the subscription is attaching to PropertyChanged on that view. This means the view has a reference to itself and thus, doesn't prevent the it from being garbage collected.

3) Do dispose

```
public MyViewModel()
{
    SomeService.SomePipeline
        .Subscribe(...);
}
```

Services commonly have a longer lifetime than view models, especially in the case of singletons and global application variables. Therefore, it's vital that these kinds of subscriptions are disposed of.

4) No need

```
public MyViewModel()
{
    SomeService.SomePipelineModelingAsynchrony
        .Subscribe(...);
}
```

Pipelines modeling asynchrony can be relied upon to complete, and thus the subscription will be disposed of automatically via OnComplete (or OnError).

5) Do dispose

```
public MyView()
{
    this.WhenAnyValue(x => x.ViewModel.SomeProperty)
        .Do(AssignValueToViewControl)
        .Subscribe();
}
```

Now you're saying "attach to PropertyChanged on _this_ and tell me when the ViewModel property changes, then attach to PropertyChanged on _that_ (the view model) and tell me when SomeProperty changes." This implies the view model has a reference back to the view, which needs to be cleaned up or else the view model will keep the view alive.

6) Performance tip

```
public MyView()
{
    // For a dependency property-based platform such as WPF and UWP
    this.WhenActivated(
        disposables =>
        {
            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Do(PopulateFromViewModel)
                .Subscribe()
                .DisposeWith(disposables);
        });
        
        // For other platforms it can be simplified to the following
        this.WhenAnyValue(x => x.ViewModel)
            .Where(x => x != null)
            .Do(PopulateFromViewModel)
            .Subscribe()
}

private void PopulateFromViewModel(MyViewModel vm)
{
    // Assign values from vm to controls
}
```

More efficient than binding to properties. If your ViewModel properties don't change over time, definitely use this pattern. The _WhenActivated_ part is important for dependency property-based platforms (as mentioned in case 2) since it will handle disposing of the subscription every time the view is deactivated.

7) No need

```
// Should I dispose of the IDisposable that WhenActivated returns?
this.WhenActivated(
    disposables =>
    {
        ...
    })
```

Quote by Kent Boogart:

> If you're using WhenActivated in a view, when do you dispose of the disposable that it returns? You'd have to store it in a local field and make the view disposable. But then who disposes of the view? You'd need platform hooks to know when an appropriate time to dispose it is - not a trivial matter if that view is reused in virtualization scenarios. In addition to this, I have found that reactive code in VMs in particular tends to juggle a lot of disposables. Storing all those disposables away and attempting disposal tends to clutter the code and force the VM itself to be disposable, further confusing matters. Perf is another factor to consider, particularly on Android.

## Notable People to Follow

Paul Betts [@paulcbetts](https://twitter.com/paulcbetts)
* [Github](https://github.com/paulcbetts)
* [Book: Programming Reactive Extensions and LINQ](http://disq.us/url?url=http%3A%2F%2Fjliberty.me%2F2lYAZb8%3AI-DqcbqqiSnKie2L8J8U2hBCoS8&cuid=3716375)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:5728+[system.reactive])
* [Stack Overflow - ReactiveUI](https://stackoverflow.com/search?q=user:5728+[reactiveui])

Kent Boogaart [@kent_boogaart](https://twitter.com/kent_boogaart)
* [Blog](https://kent-boogaart.com/blog)
* [Book: You, I, and ReactiveUI](https://kent-boogaart.com/you-i-and-reactiveui/)
* [Github](https://github.com/kentcb)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:5380+[system.reactive])

[ReactiveUI Twitter](https://twitter.com/reactivexui)

[Stack Overflow Top Users for ReactiveUI](https://stackoverflow.com/tags/reactiveui/topusers)

## ReactiveUI Glossary

_WhenActivated_

**WhenActivated:** allows you to specify the things that should occur when a view or view model is activated and deactivated; requries that our view implements IActivatable; Typically, you don't need to worry about disposing of the disposable returned by WhenActivated. Views tend to deactivate naturally as a consequence of users navigating through your application and ReactiveUI's default IActivationForViewFetcher implementations.

**IActivatable:** think of it as IActivatableView; implemented by IViewFor; tag interface (no methods to implement)

**ISupportsActivation:** think of it as IActivatableViewModel; requires that the IViewFor invokes WhenActivated; can test view model activation and deactivation by calling Activate and Deactivate; implementing this interface more than once in a view model class hierarchy will result in view model activation failing to work correctly

**ViewModelActivator:** essentially a sink in which WhenActivated will register the blocks of activation logic provided by your view model

**IActivationForViewFetcher:** implements GetAffinityForView and GetActivationForView

**GetAffinityForView:** method of IActivationForViewFetcher; tells ReactiveUI how confident you are that your implementation of IActivationForViewFetcher can provide activation information for a given view; higher numbers returned by this method trump lower numbers returned by other implementations

**GetActivationForView:** method of IActivationForViewFetcher; returns IObservable<bool> that ticks true when the view is activated and false when the view is deactivated
    
_NAVIGATION_

**RoutingState:** NavigationStack, Navigate (ReactiveCommand), NavigateBack (ReactiveCommand), NavigateAndReset (ReactiveCommand)

**IScreen:** Router (RoutingState); root of a navigation stack; despite the name, its views don't _have to_ occupy the whole screen

**IRoutableViewModel:** UrlPathSegment (string), HostScreen (IScreen)

**RoutedViewHost:** platform-specific; monitors an instance of RoutingState, responding to any changes in the navigation stack by creating and embedding the appropriate view
