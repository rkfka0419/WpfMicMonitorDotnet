using NAudio.Wave;
using System.Threading.Channels;

namespace WaveRecorder;

public interface IWaveReceiver
{
	public event Action<double[]>? WaveChunkReceived;
}


public class MicReader : IDisposable, IWaveReceiver
{
	Channel<double[]> waveQueue = Channel.CreateUnbounded<double[]>(new UnboundedChannelOptions
	{
		SingleReader = true,
		SingleWriter = true
	});

	private readonly WaveInEvent waveIn = new();
	public event Action<double[]>? WaveChunkReceived;
	const int Amplipication = 2; // 마이크 입력 증폭 비율
	CancellationTokenSource cts;

	public MicReader()
	{		
		cts = new CancellationTokenSource();
		waveIn.WaveFormat = new WaveFormat(Consts.SampleRate, 16, 1); // 44.1kHz, 16bit, mono
		waveIn.BufferMilliseconds = 100;

		waveIn.DataAvailable += (s, e) =>
		{
			var buffer = new double[e.BytesRecorded / 2];
			for (int i = 0; i < e.BytesRecorded; i += 2)
			{
				short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
				buffer[i / 2] = (sample / 32768.0) * Amplipication;
			}
			waveQueue.Writer.TryWrite(buffer);
		};

		this.Start();
		_ = Task.Run(() => ReadLoopAsync(cts.Token));
	}

	private async Task ReadLoopAsync(CancellationToken cts)
	{
		var chunk = new List<double>(Consts.ChunkSize);

		try
		{
			await foreach (var sample in waveQueue.Reader.ReadAllAsync(cts))
			{
				chunk.AddRange(sample);
				if (chunk.Count >= Consts.ChunkSize)
				{
					WaveChunkReceived?.Invoke(chunk.ToArray());
					chunk.Clear();
				}
			}
			System.Diagnostics.Debug.WriteLine("MicReader: ReadLoopAsync completed.");
		}
		catch (OperationCanceledException)
		{
		}
	}

	public void Start() => waveIn?.StartRecording();

	public void Stop()
	{
		cts.Cancel();
		waveIn?.StopRecording();
		waveIn?.Dispose();
	}

	public void Dispose() => Stop();
}
