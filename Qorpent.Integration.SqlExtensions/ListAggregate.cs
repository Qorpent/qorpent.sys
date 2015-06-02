using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.SqlExtensions {
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [SqlUserDefinedAggregate(Format.UserDefined,IsInvariantToNulls = true,IsInvariantToOrder = true,IsNullIfEmpty = false, Name = "ConcatList",MaxByteSize = -1)]
    public struct ListAggregate :IBinarySerialize{
        private string _result;

        public void Read(BinaryReader r)
        {
            _result = r.ReadString();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(this._result);
        }

        public void Init() {
            _result = "";
        }


        public void Accumulate(SqlString value) {
            if (value.IsNull) return;
            var split = value.Value.SmartSplit();
            foreach (var s in split) {
                if (!_result.Contains("/"+s+"/")) {
                    _result += "/" + s + "/";
                }
            }
        }


        public void Merge(ListAggregate value)
        {
            var split = value._result.SmartSplit();
            foreach (var s in split)
            {
                if (!_result.Contains("/" + s + "/"))
                {
                    _result += "/" + s + "/";
                }
            }
        }


        public SqlString Terminate() {
            var split = _result.SmartSplit().Distinct().OrderBy(_ => _).ToArray();
            return "/" + string.Join("/", split) + "/";
        }
    }
}