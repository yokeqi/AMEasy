using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Yokeqi.AMEasy.Winform
{
    public class Account
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CategoryName { get; set; } = "Default";
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Link { get; set; }
        public string Keyword { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; private set; } = DateTime.Now;
        public DateTime ModifyDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public object Tag { get; set; }

        public static List<Account> Parse(JArray src)
        {
            if (src.IsEmpty())
                return new List<Account>();

            var str = src.ToString();
            return JsonConvert.DeserializeObject<List<Account>>(str);
        }

        public static JArray ToJArray(List<Account> accounts)
        {
            if (accounts.IsEmpty())
                return new JArray();

            var str = JsonConvert.SerializeObject(accounts);
            return JArray.Parse(str);
        }
    }
}
