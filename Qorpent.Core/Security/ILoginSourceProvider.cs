namespace Qorpent.Security {
    public interface ILoginSourceProvider :ILoginSource{
        void Add(ILoginSource loginSource);
    }
}