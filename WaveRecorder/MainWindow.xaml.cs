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
		micPlot.Plot.Axes.SetLimitsX(0, Consts.WaveLength); // x축 범위 명시

		fftPlot.Plot.Add.ScatterLine(fftXs, fftYs).LineWidth = 3;
		fftPlot.Plot.Axes.SetLimitsY(0, 1);
		fftPlot.Plot.Axes.SetLimitsX(-Consts.WaveLength / 2, Consts.WaveLength / 2); // FFT x축 범위 명시
		micReader.WaveChunkReceived += WaveChunk_Received;
	}

	Queue<double> waveOverlap = new(Consts.WaveLength);
	private void WaveChunk_Received(double[] waveChunk)
	{
		Dispatcher.Invoke(() =>
		{
			waveOverlap.Enqueue(waveChunk, Consts.WaveLength);

			var waveOverlapArray = waveOverlap.ToArray();
			if(waveOverlapArray.Length < Consts.WaveLength)
				return;

			rawXs.Clear();
			rawYs.Clear();
			rawXs.AddRange(Enumerable.Range(0, Consts.WaveLength).Select(x => (double)x).ToArray());
			rawYs.AddRange(waveOverlapArray);
			micPlot.Refresh();

			var doubleSidedSpectrum = MathUtil.FFT(waveOverlapArray);
			fftXs.Clear();
			fftYs.Clear();
			fftXs.AddRange(Enumerable.Range(0, Consts.WaveLength).Select(x => (double)(x - Consts.WaveLength / 2)).ToArray());
			fftYs.AddRange(doubleSidedSpectrum);
			fftPlot.Refresh();
		});
	}
}