using ScottPlot;
using System.Windows;
namespace WaveRecorder;

public partial class MainWindow : Window
{
	private readonly List<double> rawXs = [];
	private readonly List<double> rawYs = [];
	private readonly List<double> fftXs = [];
	private readonly List<double> fftYs = [];

	public MainWindow(MicReader micReader)
	{

		InitializeComponent();

		micPlot.Plot.Add.ScatterLine(rawXs, rawYs).LineWidth = 3;
		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);

		fftPlot.Plot.Add.ScatterLine(fftXs, fftYs).LineWidth = 3;
		fftPlot.Plot.Axes.SetLimitsY(0, 1);
		micReader.DataReceived += MicReader_DataReceived;
	}

	private void MicReader_DataReceived(double[] micData)
	{
		rawXs.Clear();
		rawYs.Clear();
		rawXs.AddRange(Enumerable.Range(0, micData.Length).Select(x => (double)x).ToArray());
		rawYs.AddRange(micData);
		micPlot.Refresh();

		var doubleSidedSpectrum = MathUtil.FFT(micData);
		fftXs.Clear();
		fftYs.Clear();
		fftXs.AddRange(Enumerable.Range(0, doubleSidedSpectrum.Length).Select(x => (double)x).ToArray());
		fftYs.AddRange(doubleSidedSpectrum);
		fftPlot.Refresh();
	}
}