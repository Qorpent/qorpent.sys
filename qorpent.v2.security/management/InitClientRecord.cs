using Qorpent.Experiments;

namespace qorpent.v2.security.management {
    public class InitClientRecord :IJsonDeserializable{
        public string Name { get; set; }
        public string SysName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool IsDemo { get; set; }
        public string Password { get; set; }
        public void LoadFromJson(object jsonsrc) {
            var j = jsonsrc.nestorself("_source");
            Name = j.str(nameof(Name).ToLowerInvariant());
            SysName = j.str(nameof(SysName).ToLowerInvariant());
            UserName = j.str(nameof(UserName).ToLowerInvariant());
            UserEmail = j.str(nameof(UserEmail).ToLowerInvariant());
            IsDemo = j.bul(nameof(IsDemo).ToLowerInvariant());
            Password = j.str(nameof(Password).ToLowerInvariant());
        }
    }
}