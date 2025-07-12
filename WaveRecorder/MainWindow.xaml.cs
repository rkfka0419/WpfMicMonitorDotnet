using ScottPlot;
using System.Windows;
namespace WaveRecorder;

public partial class MainWindow : Window
{
	Timer timer;
	public MainWindow()
	{
		InitializeComponent();

		timer = new Timer(UpdateChart, null, 0, 1000);
	}

	Random rand = new Random();
	public double[] GetRandomData()
	{
		double[] data = new double[100];
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = rand.NextDouble() * 100;
		}
		return data;
	}

	public void UpdateChart(object? o)
	{
		micPlot.Plot.Clear();
		micPlot.Plot.Add.Signal(GetRandomData());
		micPlot.Refresh();
	}
}