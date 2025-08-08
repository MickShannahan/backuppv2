namespace backuppv2;

public partial class App : Application
{
	private readonly DirectoryWatcherService _w;
	private readonly AppState _a;
	public App(DirectoryWatcherService watcher, AppState appState)
	{
		_a = appState;
		_w = watcher;
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) { Title = "backuppv2" };
	}
}
