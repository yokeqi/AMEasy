using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yokeqi.AMEasy.Winform
{
    public class JResult
    {
        public bool OK { get; set; }
        public string Msg { get; set; }
    }
    public class JResult<T> : JResult
    {
        public T Data { get; set; }
        public JResult() { }
        public JResult(T data)
        {
            OK = true;
            Data = data;
        }
    }
}
