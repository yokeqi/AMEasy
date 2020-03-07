using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yokeqi.AMEasy.Winform
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();

            lblVersion.Text = $"版本：v{Application.ProductVersion}";
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var name = (sender as LinkLabel).Name;

            switch (name)
            {
                case "lnkContract":
                    Process.Start("mailto://ww@soye360.com");
                    break;
                case "lnkWebsite":
                    Process.Start("http://soye360.com");
                    break;
                case "lnkGithub":
                    Process.Start("https://github.com/yokeqi/AMEasy");
                    break;
                case "lnkGitee":
                    Process.Start("https://gitee.com/yokeqi/tools_ameasy");
                    break;

            }
        }
    }
}
