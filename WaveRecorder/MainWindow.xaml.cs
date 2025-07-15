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
		micReader.WaveChunkReceived += WaveChunk_Received;
	}

	private void WaveChunk_Received(double[] waveChunk)
	{
		rawXs.Clear();
		rawYs.Clear();
		rawXs.AddRange(Enumerable.Range(0, waveChunk.Length).Select(x => (double)x).ToArray());
		rawYs.AddRange(waveChunk);
		micPlot.Refresh();

		var doubleSidedSpectrum = MathUtil.FFT(waveChunk);
		fftXs.Clear();
		fftYs.Clear();
		fftXs.AddRange(Enumerable.Range(0, doubleSidedSpectrum.Length).Select(x => (double)x).ToArray());
		fftYs.AddRange(doubleSidedSpectrum);
		fftPlot.Refresh();
	}
}