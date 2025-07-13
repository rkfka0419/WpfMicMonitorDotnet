using ScottPlot;
using System.Windows;
namespace WaveRecorder;

public partial class MainWindow : Window
{
	System.Timers.Timer SystemTimer = new() { Interval = 10 };
	private readonly System.Windows.Threading.DispatcherTimer DispatcherTimer = new() { Interval = TimeSpan.FromMilliseconds(10) };

	private readonly List<double> Xs = [];
	private readonly List<double> Ys = [];
	public MainWindow()
	{
		InitializeComponent();

		// pre-populate lists with valid data
		ChangeDataLength();

		// add the scatter plot
		micPlot.Plot.Add.ScatterLine(Xs, Ys);

		SystemTimer.Elapsed += (s, e) =>
		{
			// Changing data length will throw an exception if it occurs mid-render.
			// Operations performed while the sync object will occur outside renders.
			lock (micPlot.Plot.Sync)
			{
				ChangeDataLength();
			}
			micPlot.Refresh();
		};

		DispatcherTimer.Tick += (s, e) =>
		{
			// Locking the sync object does not seem to be required
			// when data is changed using the dispatcher timer in WPF apps
			ChangeDataLength();
			micPlot.Refresh();
		};

	}


	private void ChangeDataLength(int minLength = 10_000, int maxLength = 20_000)
	{
		int newLength = Random.Shared.Next(minLength, maxLength);
		Xs.Clear();
		Ys.Clear();
		Xs.AddRange(Generate.Consecutive(newLength));
		Ys.AddRange(Generate.RandomWalk(newLength));
		micPlot.Plot.Axes.AutoScale(true);
	}

	private void micPlot_Loaded(object sender, RoutedEventArgs e)
	{
		DispatcherTimer.Start();
	}
}