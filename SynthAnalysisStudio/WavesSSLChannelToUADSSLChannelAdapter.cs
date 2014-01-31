using System;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of WavesSSLChannelToUADSSLChannelAdapter.
	/// </summary>
	public class WavesSSLChannelToUADSSLChannelAdapter
	{
		WavesSSLChannel wavesSSLChannel = null;
		
		public WavesSSLChannelToUADSSLChannelAdapter(WavesSSLChannel wavesSSLChannel)
		{
			this.wavesSSLChannel = wavesSSLChannel;
		}
		
		public UADSSLChannel DoConvert() {
			
			UADSSLChannel uadSSLChannel = new UADSSLChannel();
			uadSSLChannel.PresetName = wavesSSLChannel.PresetName;
			
			uadSSLChannel.CMPThresh = uadSSLChannel.FindClosestValue("CMP Thresh", wavesSSLChannel.CompThreshold);
			uadSSLChannel.CMPRatio = uadSSLChannel.FindClosestValue("CMP Ratio", wavesSSLChannel.CompRatio);
			uadSSLChannel.CMPAttack = Convert.ToSingle(wavesSSLChannel.CompFastAttack);
			uadSSLChannel.CMPRelease = uadSSLChannel.FindClosestValue("CMP Release", wavesSSLChannel.CompRelease);
			
			uadSSLChannel.EXPThresh = uadSSLChannel.FindClosestValue("EXP Thresh", wavesSSLChannel.ExpThreshold);
			uadSSLChannel.EXPRange = uadSSLChannel.FindClosestValue("EXP Range", wavesSSLChannel.ExpRange);
			if (wavesSSLChannel.ExpGate) {
				uadSSLChannel.Select = uadSSLChannel.FindClosestValue("Select", 0.5f);
			} else {
				uadSSLChannel.Select = uadSSLChannel.FindClosestValue("Select", 0.0f);
			}
			uadSSLChannel.EXPAttack = Convert.ToSingle(wavesSSLChannel.ExpFastAttack);
			uadSSLChannel.EXPRelease = uadSSLChannel.FindClosestValue("EXP Release", wavesSSLChannel.ExpRelease);
			
			uadSSLChannel.CompIn = Convert.ToSingle(!wavesSSLChannel.DynToByPass);
			uadSSLChannel.DYNIn = Convert.ToSingle(!wavesSSLChannel.DynToByPass);
			uadSSLChannel.PreDyn = Convert.ToSingle(wavesSSLChannel.DynToChannelOut);

			uadSSLChannel.LFBell = Convert.ToSingle(wavesSSLChannel.LFTypeBell);
			uadSSLChannel.LFGain = uadSSLChannel.FindClosestValue("LF Gain", wavesSSLChannel.LFGain);
			uadSSLChannel.LFFreq = uadSSLChannel.FindClosestValue("LF Freq", wavesSSLChannel.LFFrq);
			
			uadSSLChannel.LMFGain = uadSSLChannel.FindClosestValue("LMF Gain", wavesSSLChannel.LMFGain);
			uadSSLChannel.LMFFreq = uadSSLChannel.FindClosestValue("LMF Freq", wavesSSLChannel.LMFFrq*1000);
			uadSSLChannel.LMFQ = uadSSLChannel.FindClosestValue("LMF Q", wavesSSLChannel.LMFQ);
			
			uadSSLChannel.HMFGain = uadSSLChannel.FindClosestValue("HMF Gain", wavesSSLChannel.HMFGain);
			uadSSLChannel.HMFFreq = uadSSLChannel.FindClosestValue("HMF Freq", wavesSSLChannel.HMFFrq*1000);
			uadSSLChannel.HMFQ = uadSSLChannel.FindClosestValue("HMF Q", wavesSSLChannel.HMFQ);
			
			uadSSLChannel.HFBell = Convert.ToSingle(wavesSSLChannel.HFTypeBell);
			uadSSLChannel.HFGain = uadSSLChannel.FindClosestValue("HF Gain", wavesSSLChannel.HFGain);
			uadSSLChannel.HFFreq = uadSSLChannel.FindClosestValue("HF Freq", wavesSSLChannel.HFFrq*1000);

			uadSSLChannel.EQIn = Convert.ToSingle(!wavesSSLChannel.EQToBypass);
			uadSSLChannel.EQDynSC = Convert.ToSingle(wavesSSLChannel.EQToDynSC);
			
			uadSSLChannel.HPFreq = uadSSLChannel.FindClosestValue("HP Freq", wavesSSLChannel.HPFrq);
			if (wavesSSLChannel.LPFrq == 30) {
				uadSSLChannel.LPFreq = 0;
			} else {
				uadSSLChannel.LPFreq = uadSSLChannel.FindClosestValue("LP Freq", wavesSSLChannel.LPFrq*1000);
			}
			//wavesSSLChannel.FilterSplit;
			
			uadSSLChannel.Output = uadSSLChannel.FindClosestValue("Output", wavesSSLChannel.Gain);
			//wavesSSLChannel.Analog;
			//wavesSSLChannel.VUShowOutput;
			uadSSLChannel.Phase = Convert.ToSingle(wavesSSLChannel.PhaseReverse);
			uadSSLChannel.Input = uadSSLChannel.FindClosestValue("Input", wavesSSLChannel.InputTrim);
			
			uadSSLChannel.Power = 1.0f;
			
			return uadSSLChannel;
		}
		
	}
}
