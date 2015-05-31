using System;
using System.IO;
using System.Security.Principal;
using qorpent.v2.security.authentication;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;

namespace qorpent.v2.security.user {
    /// <summary>
    /// </summary>
    public class Identity : IIdentity,IJsonSerializable {
        /// <summary>
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// </summary>
        public Exception Error { get; set; }

        public IIdentity Native { get; set; }
        public IUser User { get; set; }
        public IIdentity ImpersonationSource { get; set; }
        public Token Token { get; set; }
        public string Name { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public void Write(TextWriter output, string mode, ISerializationAnnotator annotator) {
            var jw = new JsonWriter(output);
            jw.OpenObject();
            jw.WriteProperty("name",Name);
            jw.WriteProperty("isauth",IsAuthenticated);
            jw.WriteProperty("authtype",AuthenticationType);
            jw.WriteProperty("isadmin",IsAdmin,true);
            jw.WriteProperty("isguest",IsGuest,true);
            jw.WriteProperty("iserror",IsError,true);
            if (null != Token) {
                jw.OpenProperty("token");
                jw.WriteNative(Token.stringify());
                jw.CloseProperty();
            }
            if (null != User) {
                jw.OpenProperty("user");
                jw.WriteNative(User.stringify());
                jw.CloseProperty();
            }
            jw.CloseObject();
        }
    }
}