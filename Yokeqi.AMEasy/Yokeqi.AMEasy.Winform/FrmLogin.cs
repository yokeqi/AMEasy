using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Yokeqi.AMEasy.Winform
{
    public partial class FrmLogin : Form
    {
        private const int C_ORIENTATION_SHOW = 1;
        private const int C_ORIENTATION_HIDE = 0;

        private int _actionType = C_ORIENTATION_SHOW;
        private Action _actionHide;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Shown(object sender, EventArgs e)
        {
            tbName.Text = ActMgr.Instance.UserName;
            tbPassword.Focus();
            tmrMovie.Start();
        }

        private void tmrMovie_Tick(object sender, EventArgs e)
        {
            switch (_actionType)
            {
                case C_ORIENTATION_SHOW:
                    this.Opacity = Math.Min(1, this.Opacity + 0.1);
                    if (this.Opacity >= 100)
                    {
                        tmrMovie.Stop();
                    }
                    break;
                case C_ORIENTATION_HIDE:
                    this.Opacity = Math.Max(0, this.Opacity - 0.3);
                    this.Size = new Size(Math.Max(0, this.Size.Width - 150), Math.Max(0, this.Size.Height - 80));
                    this.Location = new Point(this.Location.X + 75, this.Location.Y + 50);
                    if (this.Opacity <= 0)
                    {
                        tmrMovie.Stop();
                        _actionHide?.Invoke();
                    }
                    break;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var name = tbName.Text.Trim();
            var pass = tbPassword.Text;
            if (string.IsNullOrEmpty(pass))
            {
                ShowTip("请输入密码");
                tbPassword.Focus();
                return;
            }

            var ret = ActMgr.Instance.Login(name, pass);
            if (!ret.OK)
            {
                this.ShowTip(ret.Msg);
                tbPassword.SelectAll();
                return;
            }

            _actionHide = () =>
            {
                DialogResult = DialogResult.OK;
            };
            _actionType = C_ORIENTATION_HIDE;
            tmrMovie.Start();
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var name = (sender as LinkLabel).Name;
            switch (name)
            {
                case "lnkForgetPassword":
                    if (MessageBox.Show("暂不支持密码找回，需要请联系作者。\r\nKey为：" + ActMgr.Instance.Key + "\r\nEmail: ww@soye360.com\r\n主页：http://soye360.com", "温馨提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        Process.Start("mailto://ww@soye360.com");
                    break;
                case "lnkAbout":
                    using (var form = new FrmAbout())
                    {
                        form.ShowDialog();
                    }
                    break;
            }
        }

        private void ShowTip(string tip)
        {
            lblTip.Text = tip;
            lblTip.Location = new Point((Width - lblTip.Size.Width) / 2, lblTip.Location.Y);
            lblTip.Visible = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    _actionHide = () =>
                    {
                        this.Close();
                    };
                    _actionType = C_ORIENTATION_HIDE;
                    tmrMovie.Start();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
