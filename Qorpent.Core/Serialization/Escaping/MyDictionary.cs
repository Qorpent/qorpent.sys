using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Serialization.Escaping
{
    /// <summary>
    /// strores information about length of keys
    /// </summary>
    class MyDictionary : Dictionary<string, char>
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
