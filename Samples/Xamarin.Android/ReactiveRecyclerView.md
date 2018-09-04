We need 4 classes:
1. A view model derived from `ReactiveObject`
2. An Activity derived from `ReactiveAppCompatActivity<MainViewModel>`
3. An adapter derived from `ReactiveRecyclerViewAdapter<MyObject, ObservableCollection<MyObject>>`
4. A view holder derived from `ReactiveRecyclerViewViewHolder<MyObject>`

And we need 2 XML layout files:
1. An activity that contains a RecyclerView
2. A view holder that contains list item controls

```csharp
public class MainViewModel : ReactiveObject
{
    private IObservable<int> _itemClicked;
    private IObservable<int> _itemLongClicked;

    public MainViewModel()
    {
        MyList = new ObservableCollection<MyObject>();
        MyList.AddRange(
            new MyObject[]
            {
                new MyObject() { MyText = "1" },
                new MyObject() { MyText = "2" },
                new MyObject() { MyText = "3" },
                new MyObject() { MyText = "4" },
                new MyObject() { MyText = "5" },
            });
            
        var clickedNoti = this
            .WhenAnyObservable(vm => vm.ItemLongClicked)
            .Select(index => $"Clicked item # {index}.");
            
        var longClickedNoti = this
            .WhenAnyObservable(vm => vm.ItemLongClicked)
            .Select(index => $"Long-clicked item # {index}.");
            
        DisplayNotification = Observable
            .Merge(clickedNoti, longClickedNoti);
    }

    public ObservableCollection<MyObject> MyList { get; }
    
    public IObservable<string> DisplayNotification { get; }
    
    public IObservable<int> ItemClicked
    {
        get => _itemClicked;
        set => this.RaiseAndSetIfChanged(ref _itemClicked, value);
    }

    public IObservable<int> ItemLongClicked
    {
        get => _itemClicked;
        set => this.RaiseAndSetIfChanged(ref _itemLongClicked, value);
    }
}
```

...

```csharp
[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
public class MainActivity : ReactiveAppCompatActivity<MainViewModel>
{
    private RecyclerView _recyclerView;
    private RecyclerView.LayoutManager _layoutManager;
    private MyRecyclerViewAdapter _adapter;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        ViewModel = new MainViewModel();

        SetContentView(Resource.Layout.activity_main);

        Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        SetSupportActionBar(toolbar);

        _recyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView);
        _layoutManager = new LinearLayoutManager(this);
        _recyclerView.SetLayoutManager(_layoutManager);
        _adapter = new MyRecyclerViewAdapter(ViewModel.MyList);
        _recyclerView.SetAdapter(_adapter);

        ViewModel.ItemClicked = _adapter.ItemClicked;
        ViewModel.ItemLongClicked = _adapter.ItemLongClicked;
        
        this.WhenActivated(
            disposables =>
            {
                ViewModel
                    .DisplayNotification
                    .Subscribe(
                        text =>
                        {
                            Toast.MakeText(this, text, ToastLength.Short).Show();
                        })
                    .DisposeWith(disposables);
            });
    }
}
```

...

```xml
<RelativeLayout
  xmlns:android="http://schemas.android.com/apk/res/android"
  xmlns:app="http://schemas.android.com/apk/res-auto"
  android:layout_width="match_parent"
  android:layout_height="match_parent">

  <android.support.v7.widget.RecyclerView
    android:id="@+id/RecyclerView"
    android:layout_width="match_parent"
    android:layout_height="match_parent" />

</RelativeLayout>
```

...

```csharp
public class MyRecyclerViewAdapter : ReactiveRecyclerViewAdapter<MyObject, ObservableCollection<MyObject>>
{
    public MyRecyclerViewAdapter(ObservableCollection<MyObject> backingList)
        : base(backingList)
    {
    }

    public override ReactiveRecyclerViewViewHolder<MyObject> OnCreateReactiveViewHolder(ViewGroup parent, int viewType)
    {
        View itemView = LayoutInflater
            .From(parent.Context)
            .Inflate(Resource.Layout.item_main, parent, false);

        var viewHolder = new MyRecyclerViewHolder(itemView);

        return viewHolder;
    }
}
```

...

```csharp
public class MyRecyclerViewHolder : ReactiveRecyclerViewViewHolder<MyObject>
{
  public MyRecyclerViewHolder(View view)
      : base(view)
  {
      this.WireUpControls();

      this.OneWayBind(this.ViewModel, x => x.MyText, x => x.MyTextView.Text);
  }

  public TextView MyTextView { get; private set; }
}
```

...

```xml
<?xml version="1.0" encoding="utf-8"?>
<RelativeLayout
  xmlns:android="http://schemas.android.com/apk/res/android"
  xmlns:app="http://schemas.android.com/apk/res-auto"
  android:layout_width="match_parent"
  android:layout_height="50dp"
  android:background="#efe5e9">

  <TextView
    android:id="@+id/MyTextView"
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:layout_centerVertical="true"
    android:layout_centerHorizontal="true"
    android:textSize="@dimen/abc_text_size_medium_material"
    android:text="Hello World!" />

</RelativeLayout>
```
