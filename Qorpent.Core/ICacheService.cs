using System;

namespace Qorpent {
    public interface ICacheService<TItem,TLeaseType> : IExtensibleService<TLeaseType>,ICacheLease where TLeaseType : ICacheLease {
        TItem Get(string key, Func<string, TItem> retriever);
        object Clear();
        int RefreshRate { get; set; }
        bool Exists(string key);
        TItem UpSet(string key, Func<string, TItem, TItem> setup);
    }
}