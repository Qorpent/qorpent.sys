using System;
using System.Collections.Generic;
using System.IO;
using qorpent.v2.security.user.storage;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.user
{
    /// <summary>
    /// Describtes minimal structure that can be used to authenticate against
    /// with password/or public key
    /// </summary>
    public class User : 
        IUser, 
        IJsonSerializable, 
        IJsonDeserializable,
        IJsonSerializationExtension,
        IUserSourceBound,
        IUserServiceBound
    {
        private IList<string> _groups;
        private IList<string> _roles;
        private IDictionary<string, string> _tags;
        private IDictionary<string, object> _custom;
        public string Login { get; set; }
        public bool IsAdmin { get; set; }
        public string PublicKey { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public string Email { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        public DateTime Expire { get; set; }
        public string ResetKey { get; set; }
        public DateTime ResetExpire { get; set; }
        public bool Logable { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public IList<string> Groups {
            get { return _groups??(_groups=new List<string>()); }
            set { _groups = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<string> Roles {
            get { return _roles??(_roles=new List<string>()); }
            set { _roles = value; }
        }

        public IDictionary<string, string> Tags {
            get { return _tags??(_tags = new Dictionary<string, string>()); }
            set { _tags = value; }
        }

        public bool Active { get; set; }
        public string MasterGroup { get; set; }
        public string Id { get; set; }

        public IDictionary<string, object> Custom {
            get { return _custom ?? (_custom=new Dictionary<string, object>()); }
            set { _custom = value; }
        }


        public void Write(TextWriter output, string usermode, ISerializationAnnotator annotator) {
            UserSerializer.WriteJson(this,output,usermode);
        }

  
        public void LoadFromJson(object jsonsrc) {
            UserSerializer.ReadJson(this,jsonsrc);
        }

        public override bool Equals(object obj) {
            var result = base.Equals(obj);
            if (result) return result;
            var other = obj as IUser;
            if (null == other) return false;
            return GetToken() == other.GetToken();
        }

        public string GetToken() {
            return ToString("store").GetMd5();
        }

        public override int GetHashCode() {
            return GetToken().GetHashCode();
        }

        public override string ToString() {
            return UserSerializer.GetJson(this,"store");
        }

        public virtual string ToString(string usermode) {
            return UserSerializer.GetJson(this, usermode);
        }

        public virtual void WriteExtensions(JsonWriter writer, string mode, ISerializationAnnotator annotator) {
        }

        public virtual void ReadExtensions(object jsonsrc) {
      
        }

        public IUserSource UserSource { get; set; }

        public IUserService UserService { get; set; }
    }
}
