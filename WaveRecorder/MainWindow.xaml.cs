using ScottPlot;
using System.Windows;
namespace WaveRecorder;

public partial class MainWindow : Window
{
	MicReader micReader = new();

	private readonly List<double> Xs = [];
	private readonly List<double> Ys = [];
	public MainWindow()
	{
		InitializeComponent();

		// add the scatter plot
		micPlot.Plot.Add.ScatterLine(Xs, Ys);

		// Y축 스케일을 -1 ~ 1로 고정
		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);

		micReader.DataReceived += MicReader_DataReceived;
	}

	private void MicReader_DataReceived(double[] obj)
	{
		Xs.Clear();
		Ys.Clear();
		Xs.AddRange(Enumerable.Range(0, obj.Length).Select(x => (double)x).ToArray());
		Ys.AddRange(obj);

		// X축만 자동 스케일, Y축은 고정
		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);
		micPlot.Plot.Axes.SetLimitsX(0, obj.Length - 1);

		micPlot.Refresh();
	}

	private void micPlot_Loaded(object sender, RoutedEventArgs e)
	{
		micReader.Start();
	}
}