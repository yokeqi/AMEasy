using Yokeqi.AMEasy.Winform.Security;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Yokeqi.AMEasy.Winform
{
    public partial class FrmChangePwd : Form
    {
        public FrmChangePwd()
        {
            InitializeComponent();
        }

        private void FrmChangePassword_Shown(object sender, EventArgs e)
        {
            tbOldUserName.Text = tbNewUserName.Text = ActMgr.Instance.UserName;
            tbOldPassword.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var oldName = tbOldUserName.Text.Trim();
            var oldPass = tbOldPassword.Text;
            var newName = tbNewUserName.Text.Trim();
            var newPass = tbNewPassword.Text;

            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrEmpty(oldPass)
                || string.IsNullOrWhiteSpace(newName) || string.IsNullOrEmpty(newPass))
            {
                tbOldPassword.Focus();
                MessageBox.Show("请输入用户信息");
                return;
            }

            if (!tbNewPassword.Text.Equals(tbNewPassword1.Text))
            {
                tbNewPassword.SelectAll();
                MessageBox.Show("两次输入密码不相同");
                return;
            }

            try
            {
                var ret = ActMgr.Instance.ChangePwd(oldName, oldPass, newName, newPass, tbNewTips.Text.Trim());
                if (!ret.OK)
                {
                    MessageBox.Show(ret.Msg);
                }
                else
                {
                    MessageBox.Show("用户信息修改成功！");
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
