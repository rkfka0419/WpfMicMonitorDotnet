using ScottPlot;
using System.Windows;
namespace WaveRecorder;

public partial class MainWindow : Window
{
	MicReader? micReader;

	private readonly List<double> Xs = [];
	private readonly List<double> Ys = [];

	public MainWindow(MicReader micReader)
	{

		InitializeComponent();

		micPlot.Plot.Add.ScatterLine(Xs, Ys);
		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);
		this.micReader = micReader;
		this.micReader.DataReceived += MicReader_DataReceived;
	}

	private void MicReader_DataReceived(double[] micData)
	{
		Xs.Clear();
		Ys.Clear();
		Xs.AddRange(Enumerable.Range(0, micData.Length).Select(x => (double)x).ToArray());
		Ys.AddRange(micData);

		// X축만 자동 스케일, Y축은 고정
		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);
		micPlot.Plot.Axes.SetLimitsX(0, micData.Length - 1);

		micPlot.Refresh();
	}

	private void micPlot_Loaded(object sender, RoutedEventArgs e)
	{
		micReader?.Start();
	}
}