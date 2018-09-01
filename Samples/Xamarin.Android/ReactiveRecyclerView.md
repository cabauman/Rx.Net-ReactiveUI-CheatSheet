```csharp
public class MainViewModel : ReactiveObject
{
    private Subject<string> _displayNotification;

    public MainViewModel()
    {
        MyList = new ObservableCollection<MyObject>();
        MyList.AddRange(
            new MyObject[]
            {
                new MyObject() { MyText = "A" },
                new MyObject() { MyText = "B" },
                new MyObject() { MyText = "C" },
                new MyObject() { MyText = "D" },
                new MyObject() { MyText = "E" },
                new MyObject() { MyText = "F" },
                new MyObject() { MyText = "G" },
                new MyObject() { MyText = "H" },
            });

        this
            .WhenAnyObservable(vm => vm.ItemClicked)
            .Subscribe(
                index =>
                {
                    _displayNotification.OnNext($"Clicked item # {index}.");
                });
    }

    public ObservableCollection<MyObject> MyList { get; }
    
    public IObservable<string> DisplayNotification => _displayNotification.AsObservable();

    private IObservable<int> _itemClicked;
    public IObservable<int> ItemClicked
    {
        get => _itemClicked;
        set => this.RaiseAndSetIfChanged(ref _itemClicked, value);
    }

    private IObservable<int> _itemLongClicked;
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

        _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
        _adapter = new MyRecyclerViewAdapter(ViewModel.MyList);
        _recyclerView.SetAdapter(_adapter);

        ViewModel.ItemClicked = _adapter.ItemClicked;

        _layoutManager = new LinearLayoutManager(this);
        _recyclerView.SetLayoutManager(_layoutManager);
        
        ViewModel
            .DisplayClickNotification
            .Subscribe(
                text =>
                {
                    Toast.MakeText(this, text, ToastLength.Short).Show();
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
    android:id="@+id/recyclerView"
    android:layout_width="match_parent"
    android:layout_height="match_parent" />

</RelativeLayout>
```

...

```csharp
public class MyRecyclerViewAdapter : ReactiveRecyclerViewAdapter<MyObject, ObservableCollection<MyObject>>
{
    public MyRecyclerViewAdapter2(ObservableCollection<MyObject> backingList)
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
