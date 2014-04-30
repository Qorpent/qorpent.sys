using System.Collections.Generic;

namespace Qorpent.Serialization
{
    /// <summary>
    /// strores information about length of keys
    /// </summary>
    public class OptimizedEscapeDictionary : Dictionary<string, char>
    {
        private HashSet<int> _len = new HashSet<int>();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="k"></param>
		/// <param name="v"></param>
        public new void Add(string k, char v)
        {
            base.Add(k, v);

            _len.Add(k.Length);
        }
		/// <summary>
		/// 
		/// </summary>
        public HashSet<int> KeysLength
        {
            get { return _len; }
        }
    }
}
