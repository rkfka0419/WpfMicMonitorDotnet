namespace WaveRecorder;
public static class SystemExtensions
{
	public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> values, int? maxCount)
	{
		foreach (var value in values)
		{
			if(maxCount.HasValue && queue.Count >= maxCount.Value)
				queue.Dequeue();
			queue.Enqueue(value);
		}
	}
}
