using System;
using System.Collections.Generic;
using System.IO;
using qorpent.v2.console;
using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.core {
    [ContainerComponent(Lifestyle.Transient,ServiceType = typeof(IReportContext),Name="qorpent.reports.context")]
    public class ReportContext : IReportContext {
        public ReportContext() {
            Agents = new List<IReportAgent>();
        }

        public ReportContext(IReportRequest request) {
            this.Request = request;
                Console = request.ConsoleContext;
                Response = request.WebContext.Response;
            
        }
        private BinaryWriter _binaryWriter;
        private TextWriter _textWriter;
        private Stream _stream;
        public IScope Scope { get; set; }
        public IReport Report { get; set; }
        public IReportRequest Request { get; set; }
        public IList<IReportAgent> Agents { get; private set; }
        public ILoggy Log { get; set; }
        public Exception Error { get; set; }

        public Stream Stream
        {
            get { return _stream ?? (_stream = GetStream()); }
            set { _stream = value; }
        }

        private  Stream GetStream() {
            if (null != Response) {
                return Response.Stream;
            }
            if (null != Console && null!=Console.OutStream) {
                return Console.OutStream;
            }
            return new MemoryStream();
        }

        public TextWriter TextWriter
        {
            get { return _textWriter ?? (_textWriter = GetTextWriter()); }
            set { _textWriter = value; }
        }

        private TextWriter GetTextWriter() {
            if (null != Console) {
                return Console.Out;
            }
            return new StreamWriter(Stream);
        }

        public BinaryWriter BinaryWriter
        {
            get { return _binaryWriter ?? (_binaryWriter = new BinaryWriter(Stream)); }
            set { _binaryWriter = value; }
        }

        public IHttpResponseDescriptor Response { get; set; }
        public IConsoleContext Console { get; set; }
        public IDictionary<string,object> Headers { get; private set; } 
        public virtual void SetHeader(string name, string value) {
            if (null != Response) {
                Response.SetHeader(name, value);
            }
            else {
                Headers = Headers ?? new Dictionary<string, object>();
                Headers[name] = value;
            }
        }

        public void Write(object data) {
            if(null==data)return;
            
            if (data is string) {
                WriteString(data as string);
            }
            else if (data is byte[]) {
                WriteBinary(data as byte[]);
            }
            else if (data.GetType().IsValueType) {
                WriteString(data.ToStr());
            }
            else {
                WriteString(data.stringify());
            }
           
        }

        private void WriteBinary(byte[] bytes) {
            if (null != Console && null == Console.OutStream) {
                WriteString(Convert.ToBase64String(bytes));
            }
            BinaryWriter.Write(bytes,0,bytes.Length);
        }

        public void Finish(Exception e) {
            WriteString(e.ToString());
            if (null != _binaryWriter) {
                _binaryWriter.Flush();
            }
            if (null != _textWriter) {
                _textWriter.Flush();
            }
            if (null != _stream) {
                _stream.Flush();
                _stream.Close();
            }
        }

        private void WriteString(string data) {
            TextWriter.Write(data);
        }
    }
}