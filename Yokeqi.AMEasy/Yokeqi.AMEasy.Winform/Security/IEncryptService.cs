using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yokeqi.AMEasy.Winform.Security
{
    /// <summary>
    /// 加密服务接口
    /// </summary>
    public interface IEncryptService
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Encrypt(byte[] data);
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Decrypt(byte[] data);
    }

    /// <summary>
    /// AES256加密服务
    /// </summary>
    public class AES256Service : IEncryptService
    {
        /// <summary>
        /// 32位密钥
        /// </summary>
        public byte[] Key { get; set; }
        /// <summary>
        /// 16位密钥
        /// </summary>
        public byte[] IV { get; set; }

        public byte[] Encrypt(byte[] data)
        {
            if (data == null || data.Length == 0) return new byte[0];

            if (Key.IsEmpty() || IV.IsEmpty())
                throw new ArgumentNullException("Key和IV值不能为空。");

            if (Key.Length != 32 || IV.Length != 16)
                throw new ArgumentException("Key和IV的位数不对。");

            using (var aesManaged = new AesManaged())
            {
                aesManaged.Key = Key;
                aesManaged.IV = IV;
                var cryptoTransform = aesManaged.CreateEncryptor();
                return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            if (data == null || data.Length == 0) return new byte[0];

            if (Key.IsEmpty() || IV.IsEmpty())
                throw new ArgumentNullException("Key和IV值不能为空。");

            if (Key.Length != 32 || IV.Length != 16)
                throw new ArgumentException("Key和IV的位数不对。");

            using (var aesManaged = new AesManaged())
            {
                aesManaged.Key = Key;
                aesManaged.IV = IV;
                var cryptoTransform = aesManaged.CreateDecryptor();
                return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
            }
        }
    }

    /// <summary>
    /// 取反倒序加密服务
    /// </summary>
    public class ReverseNegateService : IEncryptService
    {
        public byte[] Encrypt(byte[] data)
        {
            byte[] result = new byte[data.Length];
            for (int i = data.Length - 1, j = 0; i >= 0; i--, j++)
            {
                result[j] = (byte)~data[i];
            }
            return result;
        }

        public byte[] Decrypt(byte[] data)
        {
            return Encrypt(data);
        }
    }
}
