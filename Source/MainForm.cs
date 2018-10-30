using System;
using System.Windows.Forms;

namespace RTSPRedirect
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The GetRedirectUrlButton_Click method
        /// </summary>
        /// <param name="sender">The <paramref name="sender"/> parameter</param>
        /// <param name="args">The <paramref name="args"/> parameter</param>
        private void GetRedirectUrlButton_Click(object sender, EventArgs args)
        {
            if (string.IsNullOrEmpty(tbxRtspAddress.Text))
            {
                tbxRedirectUri.Text = @"Error: No URL Entered";
            }
            else
            {
                var urlRedirected = string.Empty;
                var errorMessage = string.Empty;
                var result = RtspRedirect.ObtainRedirect(tbxRtspAddress.Text, chbxPlayback.Checked, ref urlRedirected, ref errorMessage);
                tbxRedirectUri.Text = result ? urlRedirected : errorMessage;
            }
        }
    }
}
