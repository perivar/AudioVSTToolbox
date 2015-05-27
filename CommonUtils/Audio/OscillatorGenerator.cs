using System;

namespace CommonUtils.Audio
{
	/// <summary>
	/// Description of OscillatorGenerator.
	/// </summary>
	public static class OscillatorGenerator
	{
		const double PI = Math.PI;
		const double TWO_PI = 2 * Math.PI;

		public static float[] Sine() {
			
			var data = new float[128];
			for (int j = 0; j < 128; j++)
			{
				data[j] = (float) Math.Sin(j * TWO_PI / 128.0F);
			}
			return data;
		}
		
		public static float[] SawRising() {
			
			const float saw_inc = -2.0f / 127.0f;
			float var = 0.0f;
			float saw = 1.0f;
			
			var data = new float[128];
			for ( int i = 0; i < 64; i++ )
			{
				var = 0.0f;
				var += saw;
				data[ i ] = -var;
				data[ 127 - i ] = var;
				saw += saw_inc;
			}
			return data;
		}

		public static float[] SawFalling() {

			const float saw_inc = -2.0f / 127.0f;
			float var = 0.0f;
			float saw = 1.0f;
			
			var data = new float[128];
			for ( int i = 0; i < 64; i++ )
			{
				var = 0.0f;
				var += saw;
				data[ i ] = var;
				data[ 127 - i ] = -var;
				saw += saw_inc;
			}
			return data;
		}

		public static float[] Triangle() {

			const float triangle_inc = 2.0f / 64.0f;
			float var = 0.0f;
			
			var data = new float[128];
			for ( int i = 0; i < 32; i++ )
			{
				var += triangle_inc;
				data[ i ] = var;
				data[ 127 - i ] = -var;
			}
			for ( int i = 32; i < 64; i++ )
			{
				var -= triangle_inc;
				data[ i ] = var;
				data[ 127 - i ] = -var;
			}
			return data;
		}

		public static float[] SquareHighLow() {
			float var = 0.0f;
			
			var data = new float[128];
			for ( int i = 0; i < 64; i++ )
			{
				var = 1.0f;
				data[ i ] = var;
				data[ 127 - i ] = -var;
			}
			return data;
		}

		public static float[] PulseHighLowI() {
			float var = 0.0f;
			int cut = 96;

			var data = new float[128];
			for ( int i = 0; i < cut; i++ )
			{
				var = 1.0f;
				data[ i ] = var;
			}
			for ( int i = cut; i < 128; i++ )
			{
				var = -1.0f;
				data[ i ] = var;
			}
			return data;
		}

		public static float[] PulseHighLowII() {
			float var = 0.0f;
			int cut = 111;

			var data = new float[128];
			for ( int i = 0; i < cut; i++ )
			{
				var = 1.0f;
				data[ i ] = var;
			}
			for ( int i = cut; i < 128; i++ )
			{
				var = -1.0f;
				data[ i ] = var;
			}
			return data;
		}

		public static float[] TriangleSaw() {
			const float tri_saw_inc = -2.0f / 127.0f;

			float var = 0.0f;
			float tri_saw = 1.0f;
			
			var data = new float[128];
			for ( int i = 0; i < 64; i++ )
			{
				var = 0.0f;
				var += tri_saw;
				data[ i ] = var;
				data[ 64 + i ] = -var;
				tri_saw += tri_saw_inc;
			}
			return data;
		}
		
	}
}
