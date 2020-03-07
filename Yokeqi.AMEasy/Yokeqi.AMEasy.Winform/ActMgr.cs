using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yokeqi.AMEasy.Winform.Security;

namespace Yokeqi.AMEasy.Winform
{
    /// <summary>
    /// 账号管理器
    /// </summary>
    public class ActMgr
    {
        static class Nested
        {
            internal static ActMgr Instance = new ActMgr();
        }
        public static ActMgr Instance => Nested.Instance;

        private const string C_DATA_PATH = "am_repo";// 数据存放路径
        private const string C_SALT = "yokeqi.ameasy";

        private JObject _data;
        private bool _isLogin = false;
        private IEncryptService _aes;
        private IEncryptService _rn = new ReverseNegateService();
        private ICompressService _gzip = new GZipService();

        /// <summary>
        /// 标识
        /// </summary>
        public string Key
        {
            get
            {
                if (_data == null) return string.Empty;

                return _data["key"].ToString();
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                if (_data == null) return string.Empty;

                return _data["user"]["name"].ToString();
            }
        }

        private ActMgr()
        {
            _aes = new AES256Service()
            {
                Key = Encoding.Default.GetBytes("Yokeqi.AMEasy.AccountManager.AES256.Key").Take(32).ToArray(),
                IV = Encoding.Default.GetBytes("Yokeqi.AMEasy.AccountManager.AES256.IV").Take(16).ToArray()
            };

            try
            {
                if (File.Exists(C_DATA_PATH))
                {
                    var buff = File.ReadAllBytes(C_DATA_PATH);
                    _data = JObject.Parse(Decrypt(buff));
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据文件解析失败！\r\n" + ex.ToString());
                Application.Exit();
            }

            _data = new JObject();
            _data["ver"] = Application.ProductVersion;
            _data["key"] = Guid.NewGuid().ToString();
            _data["modify_date"] = _data["create_date"] = DateTime.Now.ToString();

            var user = new JObject();
            user["name"] = "admin";
            user["pass"] = GeneratePassword("admin", C_SALT, "123456");
            user["tips"] = "123456";
            _data["user"] = user;

            var accts = new JArray();
            var acct = new JObject();
            acct["Title"] = "Demo";
            acct["CategoryName"] = "Demo";
            acct["UserName"] = "admin";
            acct["Password"] = "123456";
            acct["Link"] = "http://soye360.com";
            acct["Remark"] = "这是一条示例。\r\n按Ctrl+Enter可以换行。";
            accts.Add(acct);
            _data["data_list"] = accts;

            InnerSave();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public JResult Login(string name, string pass)
        {
            var ret = Check(name, pass);
            _isLogin = ret.OK;
            return ret;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        public JResult<JArray> Load()
        {
            if (!_isLogin) return new JResult<JArray> { Msg = "用户未登陆。" };

            return new JResult<JArray>
            {
                OK = true,
                Data = _data["data_list"] as JArray,
            };
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="oldPass"></param>
        /// <param name="newName"></param>
        /// <param name="newPass"></param>
        /// <param name="tips"></param>
        /// <returns></returns>
        public JResult ChangePwd(string oldName, string oldPass, string newName, string newPass, string tips)
        {
            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newPass))
                return new JResult { Msg = "用户名、密码不能为空。" };

            var ret = Check(oldName, oldPass);
            if (!ret.OK) return ret;

            try
            {
                var md5 = MD5.Create();
                var buff = md5.ComputeHash(Encoding.UTF8.GetBytes(newName + C_SALT + newPass));
                newPass = Convert.ToBase64String(buff);

                var user = _data["user"];
                user["name"] = newName;
                user["pass"] = newPass;
                user["tips"] = tips;

                InnerSave();
                ret.OK = true;
            }
            catch (Exception ex)
            {
                ret.Msg = ex.ToString();
            }
            return ret;
        }

        /// <summary>
        /// 保存数据包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JResult Save(JArray data)
        {
            if (_isLogin) return new JResult { Msg = "用户未登陆。" };

            var ret = new JResult();
            try
            {
                _data["modify_date"] = DateTime.Now.ToString();
                _data["data_list"] = data;

                InnerSave();
                ret.OK = true;
            }
            catch (Exception ex)
            {
                ret.Msg = ex.ToString();
            }
            return ret;
        }

        private byte[] Encrypt(string data)
        {
            var buff = _aes.Encrypt(Encoding.UTF8.GetBytes(data));
            buff = _gzip.Compress(buff);
            buff = _rn.Encrypt(buff);

            return buff;
        }

        private string Decrypt(byte[] data)
        {
            var buff = _rn.Decrypt(data);
            buff = _gzip.Decompress(buff);
            buff = _aes.Decrypt(buff);
            return Encoding.UTF8.GetString(buff);
        }

        private JResult Check(string name, string pass)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pass))
                return new JResult { Msg = "用户名、密码不能为空。" };

            var ret = new JResult();

            pass = GeneratePassword(name, C_SALT, pass);

            var user = _data["user"];
            if (user["name"].ToString() != name || user["pass"].ToString() != pass)
            {
                ret.Msg = "用户名、密码错误，Tips:" + user["tips"].ToString();
                return ret;
            }

            ret.OK = true;
            return ret;
        }

        private string GeneratePassword(params string[] args)
        {
            if (args.IsEmpty()) return string.Empty;
            Array.Sort(args);

            var md5 = MD5.Create();
            var buff = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join("", args)));
            return Convert.ToBase64String(buff);
        }

        private void InnerSave()
        {
            var buff = Encrypt(_data.ToString());
            File.WriteAllBytes(C_DATA_PATH, buff);
            File.SetAttributes(C_DATA_PATH, FileAttributes.Hidden | FileAttributes.Encrypted | FileAttributes.Compressed);
        }
    }
}
