using ScottPlot;
using System.Windows;
namespace WaveRecorder;

public partial class MainWindow : Window
{
	private readonly List<double> Xs = [];
	private readonly List<double> Ys = [];

	public MainWindow(MicReader micReader)
	{

		InitializeComponent();

		micPlot.Plot.Add.ScatterLine(Xs, Ys).LineWidth = 3;
		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);
		micReader.DataReceived += MicReader_DataReceived;
	}

	private void MicReader_DataReceived(double[] micData)
	{
		Xs.Clear();
		Ys.Clear();
		Xs.AddRange(Enumerable.Range(0, micData.Length).Select(x => (double)x).ToArray());
		Ys.AddRange(micData);

		// 스케일 설정
		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);
		micPlot.Plot.Axes.SetLimitsX(0, micData.Length - 1);

		micPlot.Refresh();
	}
}