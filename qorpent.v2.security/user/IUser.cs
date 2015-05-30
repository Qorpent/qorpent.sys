using System;
using System.Collections.Generic;

namespace qorpent.v2.security.user {
    public interface IUser {
        string Login { get; set; }
        bool IsAdmin { get; set; }
        string PublicKey { get; set; }
        int Version { get; set; }
        string Name { get; set; }
        bool IsGroup { get; set; }
        string Email { get; set; }
        string Salt { get; set; }
        string Hash { get; set; }
        DateTime Expire { get; set; }
        string ResetKey { get; set; }
        DateTime ResetExpire { get; set; }
        IList<string> Groups { get; set; }
        IList<string> Roles { get; set; }
        IDictionary<string, string> Tags { get; set; }
        bool Active { get; set; }
        string Domain { get; set; }
        string Id { get; set; }
        IDictionary<string, object> Custom { get; set; }
        string GetToken();
        bool Logable { get; set; }
        DateTime CreateTime { get; set; }
        DateTime UpdateTime { get; set; }
    }
}