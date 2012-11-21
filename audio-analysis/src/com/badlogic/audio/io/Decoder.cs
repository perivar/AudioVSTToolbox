namespace com.badlogic.audio.io
{
	// * Interface for audio decoders that return successive
	// * amplitude frames.
	// * 
	// * @author mzechner
	public interface Decoder
	{
		//	 * Reads in samples.Length samples from the decoder. Returns
		//	 * the actual number read in. If this number is smaller than
		//	 * samples.Length then the end of stream has been reached.
		//	 * 
		//	 * @param samples The number of read samples.
		int ReadSamples(float[] samples);
	}
}