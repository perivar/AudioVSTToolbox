using System;
using System.Windows.Forms;
using NAudio.Wave;

namespace MidiVstTest
{
    public partial class MixerForm : Form
    {
        WaveChannel32 waveChannel = null;

        public MixerForm(WaveChannel32 channel)
        {
            InitializeComponent();

            waveChannel = channel;
            tbPan.Value = 50+(int)(channel.Pan*50.0f);
            tbVolume.Value = (int)(channel.Volume*100.0f);
        }

        private void tbPan_ValueChanged(object sender, EventArgs e)
        {
            waveChannel.Pan = ((float)tbPan.Value / 50.0f) - 1.0f;
        }

        private void MixerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel=true;
            Visible = false;
        }

        private void tbVolume_ValueChanged(object sender, EventArgs e)
        {
            waveChannel.Volume = ((float)tbVolume.Value) / 100.0f;
        }
    }
}
