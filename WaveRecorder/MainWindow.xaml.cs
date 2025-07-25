using ScottPlot;
using System.Windows;
namespace WaveRecorder;

public partial class MainWindow : Window
{
	private readonly List<double> rawXs = [];
	private readonly List<double> rawYs = [];
	private readonly List<double> fftXs = [];
	private readonly List<double> fftYs = [];

	public MainWindow(IWaveReceiver waveReceiver)
	{

		InitializeComponent();

		// ScottPlot 색상 설정 (필수)
		micPlot.Plot.FigureBackground.Color = ScottPlot.Colors.Black;
		micPlot.Plot.DataBackground.Color = ScottPlot.Colors.Black;
		micPlot.Plot.Axes.Color(ScottPlot.Colors.White);

		fftPlot.Plot.FigureBackground.Color = ScottPlot.Colors.Black;
		fftPlot.Plot.DataBackground.Color = ScottPlot.Colors.Black;
		fftPlot.Plot.Axes.Color(ScottPlot.Colors.White);

		// 형광 초록 라인
		var micLine = micPlot.Plot.Add.ScatterLine(rawXs, rawYs);
		micLine.LineWidth = 3;
		micLine.Color = ScottPlot.Color.FromHex("#00FF41");

		var fftLine = fftPlot.Plot.Add.ScatterLine(fftXs, fftYs);
		fftLine.LineWidth = 3;
		fftLine.Color = ScottPlot.Color.FromHex("#00FF41");

		micPlot.Plot.Axes.SetLimitsY(-0.5, 0.5);
		micPlot.Plot.Axes.SetLimitsX(0, Consts.WaveLength);
		fftPlot.Plot.Axes.SetLimitsY(0, 1);
		fftPlot.Plot.Axes.SetLimitsX(-Consts.WaveLength / 2, Consts.WaveLength / 2);

		waveReceiver.WaveChunkReceived += WaveChunk_Received;
	}

	Queue<double> waveOverlap = new(Consts.WaveLength);
	private void WaveChunk_Received(double[] waveChunk)
	{
		waveOverlap.Enqueue(waveChunk, Consts.WaveLength);

		var waveOverlapArray = waveOverlap.ToArray();
		if (waveOverlapArray.Length < Consts.WaveLength)
			return;

		Dispatcher.BeginInvoke(() => { UpdatePlot(waveChunk); });
	}

	private void UpdatePlot(double[] waveOverlapArray)
	{
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
	}
}