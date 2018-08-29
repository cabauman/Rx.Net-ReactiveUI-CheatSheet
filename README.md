# Rx.Net-ReactiveUI-CheatSheet
A collection of links and snippets to resources, samples, answers, people, videos, etc. related to Rx.Net and ReactiveUI.

## Table of Contents

#### [Sample Projects](#sample-projects)
#### [Articles & Documentation](#sample-projects)
#### [Videos](#videos)
#### [Tips & Best Practices - ReactiveUI](#tips--best-practices---reactiveui)
#### [People to Follow](#notable-people-to-follow)
#### [ReactiveUI Glossary](reactiveui-glossary)

## Sample Projects

### Rx.Net

### Rx.Net & ReactiveUI

[Sample code for the book "You, I, and ReactiveUI"](https://github.com/kentcb/YouIandReactiveUI)

[WorkoutWotch - Xamarin Forms Video Series](https://github.com/kentcb/WorkoutWotch)

[Reactive Examples with Xamarin Forms by TheEightBot](https://github.com/TheEightBot/Reactive-Examples)

## Articles, Documentation, & Resources

[IntroToRx](http://www.introtorx.com)

[ReactiveUI Docs](https://reactiveui.net)

[101 Rx Samples](http://rxwiki.wikidot.com/101samples)

[Custom Routing in ReactiveUI](https://kent-boogaart.com/blog/custom-routing-in-reactiveui)

## Videos

[Why You Should Be Building Better Mobile Apps with Reactive Programming – Michael Stonis](https://www.youtube.com/watch?v=DYEbUF4xs1Q)

## Tips & Best Practices - ReactiveUI

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

1) Do dispose

```
public MyView()
{
    this.WhenAnyValue(x => x.ViewModel)
        .Do(PopulateFromViewModel)
        .Subscribe();
}
```

This one is tricky. Disposing of this subscription is a must _if_ developing for a XAML-based platform such as Xamarin.Forms, WPF, or UWP. This is because "there's no non-leaky way to observe a dependencyProperty. (quoting Paul Betts)," which is exaclty what the ViewModel property of a ReactiveUserControl is. However, if you happen to know that your ViewModel won't change for the liftime of the view then you can make ViewModel a normal property, eliminating the need to dispose. For a non-XAML platform, such as Xamarin.Android and Xamarin.iOS, there's no need to dispose because you're simply monitoring the property (ViewModel) on the view itself, so the subscription is attaching to PropertyChanged on that view. This means the view has a reference to itself and thus, doesn't prevent the it from being garbage collected.

2) Do dispose

```
public MyViewModel()
{
    SomeService.SomePipeline
        .Subscribe(...);
}
```

3) No need

```
public MyViewModel()
{
    SomeService.SomePipelineModelingAsynchrony
        .Subscribe(...);
}
```

Pipelines modeling asynchrony can be relied upon to complete, and thus the subscription will be disposed of automatically via OnComplete (or OnError).

4) Do dispose

```
public MyView()
{
    this.WhenAnyValue(x => x.ViewModel.SomeProperty)
        .[do some stuff]
        .Subscribe();
}
```

Now you're saying "attach to PropertyChanged on _this_ and tell me when the ViewModel property changes, then attach to PropertyChanged on _that_ (the view model) and tell me when SomeProperty changes." This implies the view model has a reference back to the view, which needs to be cleaned up or else the view model will keep the view alive.

5) Performance tip

```
public MyView()
{
    this.WhenAnyValue(x => x.ViewModel)
        .Where(x => x != null)
        .Do(PopulateFromViewModel)
        .Subscribe();
}

private void PopulateFromViewModel(MyViewModel vm)
{
    // Assign values from vm to controls
}
```

More efficient than binding to properties. If your ViewModel properties don't change over time, definitely use this pattern.

## Notable People to Follow

Paul Betts [@paulcbetts](https://twitter.com/paulcbetts)
* [Github](https://github.com/paulcbetts)
* [Book: Programming Reactive Extensions and LINQ](http://disq.us/url?url=http%3A%2F%2Fjliberty.me%2F2lYAZb8%3AI-DqcbqqiSnKie2L8J8U2hBCoS8&cuid=3716375)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:5728+[system.reactive])
* [Stack Overflow - ReactiveUI](https://stackoverflow.com/search?q=user:5728+[reactiveui])

GeoffreyHuntley [@GeoffreyHuntley](https://twitter.com/GeoffreyHuntley)

Lee Campbell [@LeeRyanCampbell](https://twitter.com/leeryancampbell)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:393615+[system.reactive])

Kent Boogaart [@kent_boogaart](https://twitter.com/kent_boogaart)
* [Blog](https://kent-boogaart.com/blog)
* [Book: You, I, and ReactiveUI](https://kent-boogaart.com/you-i-and-reactiveui/)
* [Github](https://github.com/kentcb)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:5380+[system.reactive])

Dave Sexton [@IDaveSexton](https://twitter.com/idavesexton)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:3970148+[system.reactive])

[ReactiveUI Slack](https://reactivex.slack.com/messages/reactiveui/)

[ReactiveUI Twitter](https://twitter.com/reactivexui)

[Stack Overflow Top Users for System.Reactive](https://stackoverflow.com/tags/system.reactive/topusers)

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

## Contributing

No contribution guidelines, at the moment. Just make a pull request if you have something to contribute, and we'll go from there.

## Authors

* **Colt Bauman** - *Initial work* - [cabauman](https://github.com/cabauman)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

TODO
