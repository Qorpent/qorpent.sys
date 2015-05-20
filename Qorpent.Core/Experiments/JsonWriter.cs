using System;
using System.ComponentModel;
using System.IO;
using Qorpent.Experiments;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.Experiments {
    public class JsonWriter
    {
        private TextWriter _out;
        private SerializeMode _options;
        private IScope scope;

        public JsonWriter(TextWriter writer, SerializeMode options = SerializeMode.Serialize)
        {
            this._out = writer;
            this._options = options;
            scope= new Scope();
        }

        public void Flush() {
            this._out.Flush();
        }

        public int ArrayCount {
            get { return scope.Get("ac", 0); }
            set {
                scope.Set("ac",value);
            }
        }
        public int PropCount
        {
            get { return scope.Get("pc", 0); }
            set
            {
                scope.Set("pc", value);
            }
        }

        public void WriteValue(object obj, bool ignorechecking = false) {
            if (!ignorechecking) {
                if (InArray) {
                    if (ArrayCount > 0) {
                        _out.Write(",");

                    }
                    ArrayCount++;
                }
                if (InObj) {
                    throw new Exception("invalid state");
                }
            }
            Qorpent.Experiments.Json.Write(obj, _out, _options);
        }

        public void WriteProperty(string name, object value, bool notnullonly = false) {
            if(notnullonly && !value.ToBool())return;
            if (!InObj) {
                throw new Exception("invalid state");
            }
            if (PropCount > 0) {
                _out.Write(",");
            }
            PropCount++;
            _out.Write("\""+name.Escape(EscapingType.JsonValue)+"\"");
            _out.Write(":");
            WriteValue(value,true);
        }

        public bool InObj {
            get { return scope.Get("state", "") == "inobj"; }
        }

        public bool InArray
        {
            get { return scope.Get("state", "") == "inarray"; }
        }

        



        public void OpenObject() {
            if (InArray)
            {
                if (ArrayCount > 0)
                {
                    _out.Write(",");

                }
                ArrayCount++;
            }
            this._out.Write("{");
            scope = new Scope(scope);
            scope.Set("pc",0);
            scope.Set("state","inobj");
        }

        public void OpenArray() {
            this._out.Write("[");
            scope = new Scope(scope);
            scope.Set("ac",0);
            scope.Set("state", "inarray");
        }

        public void CloseObject() {
            this._out.Write("}");
            scope = scope.GetParent();
        }
        public void CloseArray()
        {
            this._out.Write("]");
            scope = scope.GetParent();
        }

        public void Close() {
            while (true) {
                if(InArray)CloseArray();
                if(InObj)CloseObject();
                if(!(InArray||InObj))break;
            }
            Flush();
            _out.Close();
        }

        public void OpenProperty(string name) {
            if (!InObj) {
                throw new Exception("invalid state");
            }
            if (PropCount > 0)
            {
                _out.Write(",");
            }
            PropCount++;
            scope = new Scope(scope);
            scope.Set("state", "inproperty");
            _out.Write("\"" + name.Escape(EscapingType.JsonValue) + "\"");
            _out.Write(":");
        }

        public void CloseProperty() {
            scope = scope.GetParent();
        }

        public void WriteNative(string stringify) {
            
                if (InArray)
                {
                    if (ArrayCount > 0)
                    {
                        _out.Write(",");

                    }
                    ArrayCount++;
                }
                if (InObj)
                {
                    throw new Exception("invalid state");
                }
            _out.Write(stringify);
        }
    }
}