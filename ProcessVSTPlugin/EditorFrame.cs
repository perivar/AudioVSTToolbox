using System;
using System.Drawing;
using System.Windows.Forms;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;

namespace ProcessVSTPlugin
{
    /// <summary>
    /// The frame in which a custom plugin editor UI is displayed.
    /// </summary>
    public partial class EditorFrame : Form
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public EditorFrame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Plugin Command Stub.
        /// </summary>
        //public Jacobi.Vst.Core.Host.IVstPluginCommandStub PluginCommandStub { get; set; }
       
		public VstPluginContext PluginContext { get; set; }
		
        /// <summary>
        /// Shows the custom plugin editor UI.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public new DialogResult ShowDialog(IWin32Window owner)
        {
            Rectangle wndRect = new Rectangle();

            this.Text = PluginContext.PluginCommandStub.GetEffectName();

            if (PluginContext.PluginCommandStub.EditorGetRect(out wndRect))
            {
                this.Size = this.SizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
                PluginContext.PluginCommandStub.EditorOpen(this.Handle);
            }

            return base.ShowDialog(owner);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (e.Cancel == false)
            {
                PluginContext.PluginCommandStub.EditorClose();
            }
        }
        

        void SaveBtnClick(object sender, EventArgs e)
        {
        	
        }
        
        void LoadBtnClick(object sender, EventArgs e)
        {
        	OpenFileDialog dialog = new OpenFileDialog();
        	if (dialog.ShowDialog(this) == DialogResult.OK)
            {
            	string fxpFilePath = dialog.FileName;
				VstHost host = VstHost.Instance;
				host.PluginContext = this.PluginContext;			
            	
				host.LoadFXP(fxpFilePath);
				this.Refresh();
            }        	        	
        }
    }
}
