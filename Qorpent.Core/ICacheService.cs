using System;

namespace Qorpent {
    public interface ICacheService<TItem,TLeaseType> : IExtensibleService<TLeaseType>,ICacheLease where TLeaseType : ICacheLease {
        TItem Get(string key, Func<string, TItem> retriever);
        void Clear();
        int RefreshRate { get; set; }
    }
}