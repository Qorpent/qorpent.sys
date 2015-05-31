using System.Collections.Generic;

namespace qorpent.v2.security.messaging {
    public interface IMessageQueue {
        PostMessage PushMessage(PostMessage message);
        PostMessage GetMessage(string id);
        void MarkSent(string id);
        IEnumerable<PostMessage> SearchMessages(object query);
        IEnumerable<PostMessage> GetRequireSendMessages(int count = -1);
    }
}