namespace qorpent.v2.security.messaging {
    public interface IMessageSender {
        void Send(PostMessage message);
    }
}