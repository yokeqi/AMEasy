using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Yokeqi.AMEasy.Winform
{
    public partial class FrmMain : Form
    {
        private Dictionary<string, ListViewGroup> _groupDict = new Dictionary<string, ListViewGroup>();
        private List<Account> _accounts;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            using (var form = new FrmLogin())
            {
                if (form.ShowDialog() != DialogResult.OK) Application.Exit();
            }

            var ret = ActMgr.Instance.Load();
            if (!ret.OK)
            {
                MessageBox.Show(ret.Msg);
                Application.Exit();
            }

            _accounts = Account.Parse(ret.Data);
            LoadData();
        }

        private void Menu_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            switch (item.Name)
            {
                case "tsmiChangePwd":// 修改密码
                    using (var form = new FrmChangePwd())
                    {
                        form.ShowDialog();
                    }
                    break;
                case "tsmiAbout":// 关于
                    using (var form = new FrmAbout())
                    {
                        form.ShowDialog();
                    }
                    break;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var txt = tbSearch.Text.Trim();

            foreach (ListViewItem item in lsvAccountList.Items)
            {
                if (item.BackColor != SystemColors.Window)
                    item.BackColor = SystemColors.Window;

                if (string.IsNullOrWhiteSpace(txt))
                    continue;

                var acct = item.Tag as Account;
                if (acct.Title.ContainsIgnoreCase(txt)
                    || acct.UserName.ContainsIgnoreCase(txt)
                    || acct.Keyword.ContainsIgnoreCase(txt))
                    item.BackColor = Color.LightGreen;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (lsvAccountList.SelectedItems.IsEmpty())
                return;

            var item = lsvAccountList.SelectedItems[0];
            var acct = item.Tag as Account;

            switch (button.Name)
            {
                case "btnNav":
                    try
                    {
                        var uri = new Uri(acct.Link);
                        Process.Start(uri.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("解析Url失败," + ex.ToString());
                    }
                    break;
                case "btnDelete":
                    if (MessageBox.Show(string.Format("确认删除{0}?", acct.Title), "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        return;

                    _accounts.Remove(acct);
                    lsvAccountList.Items.Remove(item);
                    ActMgr.Instance.Save(Account.ToJArray(_accounts));
                    break;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FrmDetails())
            {
                form.CategoryList.AddRange(_groupDict.Keys);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    _accounts.Add(form.Account);
                    AddAccount(form.Account);
                    ActMgr.Instance.Save(Account.ToJArray(_accounts));
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lsvAccountList.SelectedItems.IsEmpty())
                return;

            var item = lsvAccountList.SelectedItems[0];
            var acct = item.Tag as Account;

            using (var form = new FrmDetails())
            {
                form.CategoryList.AddRange(_groupDict.Keys);
                form.Account = acct;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    item.Group = GetOrCreateGroup(acct.CategoryName);
                    item.Text = acct.Title;
                    item.SubItems["UserName"].Text = acct.UserName;
                    item.SubItems["Password"].Text = MaskPassword(acct.Password);
                    item.SubItems["ModifyDate"].Text = acct.ModifyDate.ToString();
                    item.SubItems["Url"].Text = acct.Link;
                    ActMgr.Instance.Save(Account.ToJArray(_accounts));
                }
            }
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                btnSearch.PerformClick();
        }

        private void lsvAccountList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                btnEdit.PerformClick();
            }
        }

        private void lsvAccountList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var enable = !lsvAccountList.SelectedItems.IsEmpty();
            btnEdit.Enabled = btnDelete.Enabled = btnNav.Enabled = enable;

            if (enable)
            {
                var item = lsvAccountList.SelectedItems[0].Tag as Account;
                btnNav.Enabled = (item != null && !string.IsNullOrWhiteSpace(item.Link));
            }
        }

        private void LoadData()
        {
            lsvAccountList.Items.Clear();

            _accounts.Sort((a, b) =>
            {
                return a.CategoryName.CompareTo(b.CategoryName);
            });
            foreach (var acct in _accounts)
            {
                AddAccount(acct);
            }
        }

        private void AddAccount(Account acct)
        {
            var group = GetOrCreateGroup(acct.CategoryName);
            var item = new ListViewItem(acct.Title, group);

            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, acct.UserName) { Name = "UserName" });
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, MaskPassword(acct.Password)) { Name = "Password" });
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, acct.ModifyDate.ToString()) { Name = "ModifyDate" });
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, acct.Link) { Name = "Url" });

            item.Tag = acct;
            lsvAccountList.Items.Add(item);
        }

        private ListViewGroup GetOrCreateGroup(string name)
        {
            if (_groupDict.ContainsKey(name))
                return _groupDict[name];

            var group = new ListViewGroup(name);
            lsvAccountList.Groups.Add(group);
            _groupDict.Add(name, group);

            return group;
        }

        private string MaskPassword(string pass)
        {
            if (string.IsNullOrWhiteSpace(pass) || pass.Length < 6)
                return "******";

            return string.Format("{0}***{1}", pass.First(), pass.Last());
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
                tbSearch.Focus();

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
