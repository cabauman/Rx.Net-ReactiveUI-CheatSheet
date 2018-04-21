# Rx.Net-ReactiveUI-CheatSheet
A collection of links and snippets to resources, samples, answers, people, videos, etc. related to Rx.Net and ReactiveUI.

## Table of Contents

#### [Sample Projects](#sample-projects)
#### [Articles & Documentation](#sample-projects)
#### [Videos](#videos)
#### [People to Follow](#notable-people-to-follow)

## Sample Projects

### Rx.Net

### Rx.Net & ReactiveUI
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

#### The best way to use Service Locator

**Tip:** Inject service via constructor

```
public SuspensionHost(ISuspensionDriver driver = null)
{
    driver = driver ?? Locator.Current.GetService<ISuspensionDriver>();
}
```

**Explanation:** This uses a Service Located interface for the default interface, but only if the caller didn't give an explicit one in the constructor. Far more straightforward to test in a unit test runner than trying to construct a sham IoC container, but still falls back to a default implementation at runtime.

**Source:** [https://stackoverflow.com/a/26924067/5984310](https://stackoverflow.com/a/26924067/5984310)

**Tip:** Call async operations in the View constructor, rather than the ViewModel constructor.

```
this.WhenAnyValue(x => x.ViewModel.LoadItems)
    .SelectMany(x => x.ExecuteAsync())
    .Subscribe();
```

**Explanation:** Invoking async operations in the ViewModel constructor means that your ViewModel class becomes more difficult to test, because you always have to mock out the effects of calling LoadItems, even if the thing you are testing is unrelated.

**Source:** [https://codereview.stackexchange.com/a/74793](https://codereview.stackexchange.com/a/74793)

## Notable People to Follow

Paul Betts [@paulcbetts](https://twitter.com/paulcbetts)
* [Github](https://github.com/paulcbetts)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:5728+[system.reactive])
* [Stack Overflow - ReactiveUI](https://stackoverflow.com/search?q=user:5728+[reactiveui])

GeoffreyHuntley [@GeoffreyHuntley](https://twitter.com/GeoffreyHuntley)

Lee Campbell [@LeeRyanCampbell](https://twitter.com/leeryancampbell)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:393615+[system.reactive])

Kent Boogaart [@kent_boogaart](https://twitter.com/kent_boogaart)
* [Blog](https://kent-boogaart.com/blog)
* [Github](https://github.com/kentcb)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:5380+[system.reactive])

Dave Sexton [@IDaveSexton](https://twitter.com/idavesexton)
* [Stack Overflow - System.Reactive](https://stackoverflow.com/search?q=user:3970148+[system.reactive])

[ReactiveUI Slack](https://reactivex.slack.com/messages/reactiveui/)

[ReactiveUI Twitter](https://twitter.com/reactivexui)

[Stack Overflow Top Users for System.Reactive](https://stackoverflow.com/tags/system.reactive/topusers)

[Stack Overflow Top Users for ReactiveUI](https://stackoverflow.com/tags/reactiveui/topusers)

## Contributing

No contribution guidelines, at the moment. Just make a pull request if you have something to contribute, and we'll go from there.

## Authors

* **Colt Bauman** - *Initial work* - [cabauman](https://github.com/cabauman)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

TODO
