using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace WaveRecorder;
public partial class App : Application
{

	public static IServiceProvider Services { get; private set; } = null!;

	protected override void OnStartup(StartupEventArgs e)
	{
		var services = new ServiceCollection();

		services.AddSingleton<IWaveReceiver, MicReader>();
		services.AddTransient<MainWindow>();

		Services = services.BuildServiceProvider();

		var mainWindow = Services.GetRequiredService<MainWindow>();
		mainWindow.Show();

		base.OnStartup(e);
	}
}