using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yokeqi.AMEasy.Winform.Security
{
    /// <summary>
    /// 压缩服务接口
    /// </summary>
    public interface ICompressService
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Compress(byte[] data);
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Decompress(byte[] data);
    }

    /// <summary>
    /// GZip压缩服务
    /// </summary>
    public class GZipService : ICompressService
    {
        protected const int C_PROCESS_SIZE = 1024 * 1024;//每次处理1M数据

        public byte[] Compress(byte[] data)
        {
            var result = new byte[0];
            using (var ms = new MemoryStream())
            {
                using (var zip = new GZipStream(ms, CompressionMode.Compress))
                {
                    zip.Write(data, 0, data.Length);
                    zip.Close();
                }

                ms.Close();
                result = ms.ToArray();
            }

            return result;
        }

        public byte[] Decompress(byte[] data)
        {
            var result = new byte[0];
            using (var ms1 = new MemoryStream())
            {
                using (var ms = new MemoryStream(data, 0, data.Length))
                {
                    using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        var tmp = new byte[C_PROCESS_SIZE];
                        int len = 0;
                        while ((len = zip.Read(tmp, 0, tmp.Length)) > 0)
                        {
                            ms1.Write(tmp, 0, len);
                        }
                        zip.Close();
                    }
                    ms.Close();
                }
                ms1.Close();
                result = ms1.ToArray();
            }

            return result;
        }
    }
}
