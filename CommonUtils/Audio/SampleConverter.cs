namespace CommonUtils.Audio
{
    using System;
    
    /// <summary>
    ///   Static methods to convert between different sample formats.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Code is mainly based on information available on the original
    ///   C source code pa_converters.c from Portable Audio I/O Library.
    /// </para>
    /// <para>
    ///   This class try to be as fast as possible without using unsafe code.
    /// </para>
    /// <para>
    ///   Dither isn't currently supported. Currently supported conversions
    ///   are 'to' and 'from' conversions between the following value types:
    /// </para>
    /// <para>
    ///     Integer 8-bit  (byte), 
    ///     Integer 16-bit (Int16), 
    ///     Integer 32-bit (Int32), 
    ///     Single precision 32-bit floating point (float).
    /// </para>
    /// <para>
    ///   Which are the most common PCM formats.
    /// </para>
    /// <example>
    /// To use it, just call Convert. The compiler will automatically detect
    /// which method to call based on your data types. Couldn't be simplier.
    /// <code>
    ///   int[]   srcPcm16samples = new int[] { 1, 2, 3};
    ///   float[] destIeeeFloat32samples = new float[3];
    /// 
    ///   SampleConverter.Convert(srcPcm16samples, destIeeeFloat32samples);
    /// 
    ///   // destIeeeFloat32samples now contains the converted samples.
    /// </code>
    /// </example>
    /// http://crsouza.blogspot.com/2009/08/converting-between-different-audio.html
    /// </remarks>
    public static class SampleConverter
    {
        
        #region Consts: Standard values used in almost all conversions.
        private const float const_1_div_128_ = 1.0f / 128.0f;  // 8 bit multiplier
        private const float const_1_div_32768_ = 1.0f / 32768.0f; // 16 bit multiplier
        private const double const_1_div_2147483648_ = 1.0 / 2147483648.0; // 32 bit
        #endregion
        
        
        #region From UInt8 (byte)
        
        #region From UInt8 (byte) to Int16 (short)
        /// <summary>
        ///   Converts a array of signed 8-bit int samples
        ///   into a array of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte[][] from, Int16[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[0].Length; i++)
                to[i][j] = (Int16)((from[i][j] - 128) << 8);
        }
        
        /// <summary>
        ///   Converts a array of signed 8-bit int samples
        ///   into a array of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte[] from, Int16[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (Int16)((from[i] - 128) << 8);
        }
        
        /// <summary>
        ///   Converts a signed 8-bit int sample
        ///   into a 32-bit float-point sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte from, out Int16 to)
        {
            to = (Int16)((from - 128) << 8);
        }
        #endregion
        
        #region From UInt8 (byte) to Int32 (int)
        /// <summary>
        ///   Converts a matrix of unsigned 8-bit int samples
        ///   into a matrix of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte[][] from, int[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                to[i][j] = ((from[i][j] - 128) << 24);
        }
        
        /// <summary>
        ///   Converts a array of unsigned 8-bit int samples
        ///   into a array of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte[] from, int[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = ((from[i] - 128) << 24);
        }
        
        /// <summary>
        ///   Converts a unsigned 8-bit int sample
        ///   into a 32-bit float-point sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte from, out int to)
        {
            to = ((from - 128) << 24);
        }
        #endregion
        
        #region From UInt8 (byte) to Single (float)
        /// <summary>
        ///   Converts a matrix of unsigned 8-bit int samples
        ///   into a matrix of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte[][] from, float[][] to)
        {
            for (int i = 0; i < from.Length; i++)
                for (int j = 0; j < from[0].Length; j++)
                to[i][j] = (from[i][j] - 128) * const_1_div_128_;
        }
        
        /// <summary>
        ///   Converts a array of unsigned 8-bit int samples
        ///   into a array of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte[] from, float[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (from[i] - 128) * const_1_div_128_;
        }
        
        /// <summary>
        ///   Converts a unsigned 8-bit int sample
        ///   into a 32-bit float-point sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(byte from, out float to)
        {
            to = (from - 128) * const_1_div_128_;
        }
        #endregion
        
        #endregion
        
        
        #region From Int16 (short)
        
        #region From Int16 (short) to UInt8 (byte)
        /// <summary>
        ///   Converts a matrix of signed 16-bit int samples
        ///   into a matrix of 8-bit unsigned byte samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16[][] from, byte[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (byte)(((from[i][j]) >> 8) + 128);
        }
        
        /// <summary>
        ///   Converts a array of signed 16-bit int samples
        ///   into a array of 8-bit unsigned byte samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16[] from, byte[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(((from[i]) >> 8) + 128);
        }
        
        /// <summary>
        ///   Converts a signed 16-bit int sample
        ///   into a 8-bit unsigned byte sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16 from, out byte to)
        {
            to = (byte)(((from) >> 8) + 128);
        }
        #endregion
        
        #region From Int16 (short) to Int32 (int)
        /// <summary>
        ///   Converts a matrix of signed 16-bit int samples
        ///   into a matrix of 32-bit signed integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16[][] from, Int32[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (byte)(((from[i][j]) >> 8) + 128);
        }
        
        /// <summary>
        ///   Converts a array of signed 16-bit int samples
        ///   into a array of 32-bit signed integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16[] from, Int32[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(((from[i]) >> 8) + 128);
        }
        
        /// <summary>
        ///   Converts a signed 16-bit int sample
        ///   into a 32-bit signed integer sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16 from, out Int32 to)
        {
            to = (byte)(((from) >> 8) + 128);
        }
        #endregion
        
        #region From Int16 (short) to Single (float)
        /// <summary>
        ///   Converts a matrix of signed 16-bit int samples
        ///   into a matrix of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16[][] from, float[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (float)(from[i][j]*const_1_div_32768_);
        }
        
        /// <summary>
        ///   Converts a array of signed 16-bit int samples
        ///   into a array of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16[] from, float[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (float)(from[i]*const_1_div_32768_);
        }
        
        /// <summary>
        ///   Converts a signed 16-bit int sample
        ///   into a 32-bit float-point sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int16 from, out float to)
        {
            to = (float)(from*const_1_div_32768_);
        }
        #endregion
        
        #endregion
        
        
        #region From Int32 (int)
        
        #region From Int32 (int) To UInt8 (byte)
        /// <summary>
        ///   Converts a matrix of signed 32-bit int samples
        ///   into a matrix of 8-bit unsigned byte samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32[][] from, byte[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (byte)(((from[i][j]) >> 24) + 128);
        }
        
        /// <summary>
        ///   Converts a array of signed 32-bit int samples
        ///   into a array of 8-bit unsigned byte samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32[] from, byte[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(((from[i]) >> 24) + 128);
        }
        
        /// <summary>
        ///   Converts a signed 32-bit int sample
        ///   into a 8-bit unsigned byte sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32 from, out byte to)
        {
            to = (byte)((from >> 24) + 128);
        }
        #endregion
        
        #region From Int32 (int) to Int16 (short)
        /// <summary>
        ///   Converts a matrix of signed 32-bit int samples
        ///   into a matrix of 16-bit signed integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32[][] from, Int16[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (Int16)(from[i][j] >> 16);
        }
        
        /// <summary>
        ///   Converts a array of signed 32-bit int samples
        ///   into a array of 16-bit signed integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32[] from, Int16[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (Int16)(from[i] >> 16);
        }
        
        /// <summary>
        ///   Converts a signed 32-bit int sample
        ///   into a 16-bit signed integer sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32 from, out Int16 to)
        {
            to = (Int16)(from >> 16);
        }
        #endregion
        
        #region From Int32 (int) to Single (float)
        /// <summary>
        ///   Converts a matrix of signed 32-bit int samples
        ///   into a matrix of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32[][] from, float[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (float)(from[i][j]*const_1_div_2147483648_);
        }
        
        /// <summary>
        ///   Converts a array of signed 32-bit int samples
        ///   into a array of 32-bit float-point samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32[] from, float[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (float)((double)from[i]*const_1_div_2147483648_);
        }
        
        /// <summary>
        ///   Converts a signed 32-bit int sample
        ///   into a 32-bit float-point sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(Int32 from, out float to)
        {
            to = (float)((double)from*const_1_div_2147483648_);
        }
        #endregion
        
        #endregion
        
        
        #region From Single (float)
        
        #region From Single (float) to UInt8 (byte)
        /// <summary>
        ///   Converts a matrix of signed 32-bit float samples
        ///   into a matrix of 8-bit unsigned byte samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[][] from, byte[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (byte)(128 + ((byte)(to[i][j]*(127.0f))));
        }
        
        /// <summary>
        ///   Converts a array of signed 32-bit float samples
        ///   into a array of 8-bit unsigned byte samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[] from, byte[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (byte)(128 + ((byte)(to[i]*(127.0f))));
        }
        
        /// <summary>
        ///   Converts a signed 32-bit float sample
        ///   into a 8-bit unsigned byte sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float from, out byte to)
        {
            to = (byte)(128 + ((byte)(from*(127.0f))));
        }
        #endregion
        
        #region From Single (float) to Int16 (short)
        /// <summary>
        ///   Converts a matrix of signed 32-bit float samples
        ///   into a matrix of signed 16-bit integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[][] from, short[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (short) (from[i][j] * (32767.0f));
        }
        
        /// <summary>
        ///   Converts a array of signed 32-bit float samples
        ///   into a array of signed 16-bit integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[] from, short[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (short) (from[i] * (32767.0f));
        }
        
        /// <summary>
        ///   Converts a signed 32-bit float sample
        ///   into a signed 16-bit integer sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float from, out short to)
        {
            to = (short) (from * (32767.0f));
        }
        #endregion
        
        #region From Single (float) to Int32 (int)
        /// <summary>
        ///   Converts a matrix of signed 32-bit float samples
        ///   into a matrix of signed 32-bit integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[][] from, Int32[][] to)
        {
            for (int j = 0; j < from.Length; j++)
                for (int i = 0; i < from[i].Length; i++)
                to[i][j] = (int)((double)from[i][j] * 0x7FFFFFFF);
        }
        
        /// <summary>
        ///   Converts a array of signed 32-bit float samples
        ///   into a array of signed 32-bit integer samples.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float[] from, Int32[] to)
        {
            for (int i = 0; i < from.Length; i++)
                to[i] = (int)((double)from[i] * 0x7FFFFFFF);
        }
        
        /// <summary>
        ///   Converts a signed 32-bit floating-point sample
        ///   into a signed 32-bit integer sample.
        /// </summary>
        /// <param name="from">The original sample.</param>
        /// <param name="to">The resulting sample.</param>
        public static void Convert(float from, out Int32 to)
        {
            to = (int)((double)from * 0x7FFFFFFF);
        }
        #endregion
        
        #endregion
        
    }
}