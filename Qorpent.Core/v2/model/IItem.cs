using System.IO;
using Qorpent.Model;

namespace qorpent.v2.model {
    public interface IItem : 
        IWithStringId,
        IWithIntVersion,
        IWithName,
        IWithCustom
       
    {
        void Read(object src);
        void Write(TextWriter writer, string mode = null);
        void Merge(IItem src);
        IItem Clone();
    }
}