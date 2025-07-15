using System.Numerics;

namespace WaveRecorder;
public static class MathUtil
{
	public static double[] FFT(double[] input)
	{
		input = input.RemoveDcOffset().ApplyHanning();
		int n = input.Length;

		Complex[] buffer = new Complex[n];
		for (int i = 0; i < n; i++)
			buffer[i] = new Complex(input[i], 0);

		// FFT  (in-place)
		FFT(buffer);

		// Double-sided FFT Result
		double[] magnitude = new double[n];
		for (int i = 0; i < n; i++)
			magnitude[i] = buffer[i].Magnitude;

		return magnitude.FftShift();
	}

	private static void FFT(Complex[] buffer)
	{
		int n = buffer.Length;
		if ((n & (n - 1)) != 0)
			throw new ArgumentException("FFT input length must be a power of 2.");

		// Bit-reversal
		int bits = (int)Math.Log2(n);
		for (int i = 0; i < n; i++)
		{
			int j = ReverseBits(i, bits);
			if (j > i)
				(buffer[i], buffer[j]) = (buffer[j], buffer[i]);
		}

		// FFT Calculate
		for (int len = 2; len <= n; len <<= 1)
		{
			double angle = -2 * Math.PI / len;
			Complex wlen = new Complex(Math.Cos(angle), Math.Sin(angle));

			for (int i = 0; i < n; i += len)
			{
				Complex w = Complex.One;
				for (int j = 0; j < len / 2; j++)
				{
					Complex u = buffer[i + j];
					Complex v = buffer[i + j + len / 2] * w;
					buffer[i + j] = u + v;
					buffer[i + j + len / 2] = u - v;
					w *= wlen;
				}
			}
		}
	}

	private static int ReverseBits(int n, int bits)
	{
		int result = 0;
		for (int i = 0; i < bits; i++)
		{
			result <<= 1;
			result |= n & 1;
			n >>= 1;
		}
		return result;
	}
	private static double[] FftShift(this double[] fft)
	{
		int n = fft.Length;
		int half = n / 2;
		double[] shifted = new double[n];
		Array.Copy(fft, half, shifted, 0, half);
		Array.Copy(fft, 0, shifted, half, half);
		return shifted;
	}
	public static double[] RemoveDcOffset(this double[] input)
	{
		double mean = input.Average();
		return input.Select(x => x - mean).ToArray();
	}
	public static double[] ApplyHanning(this double[] input)
	{
		int n = input.Length;
		double[] windowed = new double[n];
		for (int i = 0; i < n; i++)
			windowed[i] = input[i] * (0.5 - 0.5 * Math.Cos(2 * Math.PI * i / (n - 1)));
		return windowed;
	}
}
