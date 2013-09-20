using System.Collections.Generic;

namespace Qorpent.Serialization
{
    /// <summary>
    /// strores information about length of keys
    /// </summary>
    class OptimizedEscapeDictionary : Dictionary<string, char>
    {
        private HashSet<int> _len = new HashSet<int>();

        public new void Add(string k, char v)
        {
            base.Add(k, v);

            _len.Add(k.Length);
        }

        public HashSet<int> KeysLength
        {
            get { return _len; }
        }
    }
}
