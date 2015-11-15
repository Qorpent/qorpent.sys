using System;
using Qorpent.Experiments;

namespace qorpent.v2.security.management {
    public class ClientRecord :IJsonDeserializable{
        public string Name { get; set; }
        public string SysName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool IsDemo { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Operation { get; set; }
        public DateTime Expire { get; set; }
        public void LoadFromJson(object jsonsrc) {
            var j = jsonsrc.nestorself("_source");
            Name = j.str(nameof(Name).ToLowerInvariant());
            SysName = j.str(nameof(SysName).ToLowerInvariant());
            UserName = j.str(nameof(UserName).ToLowerInvariant());
            UserEmail = j.str(nameof(UserEmail).ToLowerInvariant());
            IsDemo = j.bul(nameof(IsDemo).ToLowerInvariant());
            Password = j.str(nameof(Password).ToLowerInvariant());
            Phone = j.str(nameof(Phone).ToLowerInvariant());
            Operation = j.str(nameof(Operation).ToLowerInvariant())??"init" ;
            Expire = j.date(nameof(Expire).ToLowerInvariant());
            if (null != SysName && SysName.Contains("@")) {
                SysName = SysName.Replace("@groups", "");
            }
        }
    }
}