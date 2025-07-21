namespace WaveRecorder;
internal class Consts
{
	public const int SampleRate = 44100; // Sample rate in Hz
	public const int ChunkSize = 256; // Wave data chunk size
	public const int ChunkOverlap = 4; // Overlapping size for chunk processing
	public static int WaveLength => ChunkSize * ChunkOverlap; // Total length of wave data processed at once
}
