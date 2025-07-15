using NAudio.Wave;

namespace WaveRecorder;
public class MicReader : IDisposable
{
	private readonly WaveInEvent waveIn = new();
	public event Action<double[]>? DataReceived;
	const int Amplipication = 2; // 마이크 입력 증폭 비율

	public MicReader()
	{
		waveIn.WaveFormat = new WaveFormat(65536, 16, 1); // 44.1kHz, 16bit, mono
		waveIn.BufferMilliseconds = 125;

		waveIn.DataAvailable += (s, e) =>
		{
			var buffer = new double[e.BytesRecorded / 2];
			for (int i = 0; i < e.BytesRecorded; i += 2)
			{
				short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
				buffer[i / 2] = (sample / 32768.0) * Amplipication;
			}
			DataReceived?.Invoke(buffer);
		};

		this.Start();
	}

	public void Start() => waveIn?.StartRecording();

	public void Stop()
	{
		waveIn?.StopRecording();
		waveIn?.Dispose();
	}

	public void Dispose() => Stop();
}
