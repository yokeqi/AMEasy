using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Yokeqi.AMEasy.Winform
{
    public partial class FrmDetails : Form
    {
        public List<string> CategoryList { get; set; } = new List<string>();
        public Account Account { get; set; } = new Account();

        private bool _readOnly = false;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                foreach (var c in Controls)
                {
                    if (c is TextBoxBase)
                    {
                        var tb = c as TextBoxBase;
                        tb.ReadOnly = value;
                        tb.BackColor = SystemColors.Window;
                    }
                }

                cboCategory.Enabled = !value;
            }
        }

        public FrmDetails()
        {
            InitializeComponent();
        }

        private void FrmDetails_Load(object sender, EventArgs e)
        {
            cboCategory.DataSource = CategoryList;

            tbTitle.Text = Account.Title;
            cboCategory.Text = Account.CategoryName;
            tbUserName.Text = Account.UserName;
            tbPassword.Text = Account.Password;
            tbKeyword.Text = Account.Keyword;
            tbUrl.Text = Account.Link;
            rtbRemark.Text = Account.Remark;
        }

        private void FrmDetails_Shown(object sender, EventArgs e)
        {
            tbTitle.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateData())
                return;

            Account.Title = tbTitle.Text;
            Account.CategoryName = cboCategory.Text;
            Account.UserName = tbUserName.Text;
            Account.Password = tbPassword.Text;
            Account.Keyword = tbKeyword.Text;
            Account.Link = tbUrl.Text;
            Account.Remark = rtbRemark.Text;
            Account.ModifyDate = DateTime.Now;

            DialogResult = DialogResult.OK;
        }

        private void tbUrl_TextChanged(object sender, EventArgs e)
        {
            btnGoUrl.Enabled = !string.IsNullOrWhiteSpace(tbUrl.Text);
        }

        private void btnGoUrl_Click(object sender, EventArgs e)
        {
            try
            {
                var uri = new Uri(tbUrl.Text.Trim());
                Process.Start(uri.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("解析Url失败," + ex.ToString());
            }
        }

        private bool ValidateData()
        {
            tbTitle.Text = tbTitle.Text.Trim();

            if (string.IsNullOrEmpty(tbTitle.Text))
            {
                errorProvider1.SetError(tbTitle, "标题不能为空。");
                return false;
            }

            return true;
        }
    }
}
