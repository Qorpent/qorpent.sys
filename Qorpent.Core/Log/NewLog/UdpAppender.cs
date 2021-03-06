﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Qorpent.IoC;
using Qorpent.Security;

namespace Qorpent.Log.NewLog {
    /// <summary>
    /// </summary>
    public class UdpAppender : AppenderBase {
        public int AutoFlushSize = 1;
        private readonly ConcurrentStack<string> _messageBuffer;
        private readonly UdpClient _udpClient;
        private readonly object sync = new object();

        /// <summary>
        /// </summary>
        public UdpAppender()
            : this("localhost", 7071) {
        }

        /// <summary>
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public UdpAppender(string host, int port) {
            Level = LogLevel.Debug;
            _messageBuffer = new ConcurrentStack<string>();
            _udpClient = new UdpClient();
            _udpClient.Connect(host, port);
        }

        protected override void InternalWrite(LoggyMessage message) {
            var xml = GetEventXml(message);
            lock (sync) {
                _messageBuffer.Push(xml);
                if (_messageBuffer.Count >= AutoFlushSize) {
                    
                    Flush();
                }
            }
        }

        public override void Flush() {
            lock (sync) {
                foreach (var xmlMessage in _messageBuffer) {
                    var payload = Encoding.UTF8.GetBytes(xmlMessage);
                    try {
                        _udpClient.SendAsync(payload, payload.Length);

                    }
                    catch {
                        
                    }
                }

                _messageBuffer.Clear();
            }
        }


        public override void Dispose() {
            Flush();
            _udpClient.Close();
        }

        public string GetExceptionString(Exception ex) {
			var result = ex.GetType() + ": " + ex.Message;
	        var aggregate = ex as AggregateException;
			if (aggregate != null) {
				result += "\n";
				foreach (var exception in aggregate.InnerExceptions) {
					var message = exception.ToString();
					result += "\t" + message.Replace("\r\n", "\n").Replace("\n", "\n\t") + "\n";
				}
			}
	        return result;
        }

        [Inject]
        public IPrincipalSource PrincipalSource { get; set; }

        public string GetEventXml(LoggyMessage _message) {
            var username = "unknown";
            if (null != PrincipalSource) {
                username = PrincipalSource.CurrentUser.Identity.Name;
            }
            else {
                if (Applications.Application.HasCurrent) {
                    if (Applications.Application.Current.Principal != null) {
                        username = Applications.Application.Current.Principal.CurrentUser.Identity.Name;
                    }
                }
            }
            // The format:
            //<log4j:event logger="{LOGGER}" level="{LEVEL}" thread="{THREAD}" timestamp="{TIMESTAMP}">
            //  <log4j:message><![CDATA[{ERROR}]]></log4j:message>
            //  <log4j:NDC><![CDATA[{MESSAGE}]]></log4j:NDC>
            //  <log4j:throwable><![CDATA[{EXCEPTION}]]></log4j:throwable>
            //  <log4j:locationInfo class="org.apache.log4j.chainsaw.Generator" method="run" file="Generator.java" line="94"/>
            //  <log4j:properties>
            //	<log4j:data name="log4jmachinename" value="{SOURCE}"/>
            //	<log4j:data name="log4japp" value="{APP}"/>
            //  </log4j:properties>
            //</log4j:event>
            var category = _message.Level.ToString();
            var level = category;
            var message = GetText(_message);
            var loggername = _message.LoggerName;


            var builder = new StringBuilder();

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            var writer = XmlWriter.Create(builder, settings);
            WriteLog4jElement(writer, "event");
            writer.WriteAttributeString("logger", loggername);
            writer.WriteAttributeString("timestamp", DateTime.Now.ToString("o"));

            writer.WriteAttributeString("level", level);
            writer.WriteAttributeString("thread", Thread.CurrentThread.ManagedThreadId.ToString());
            writer.WriteAttributeString("domain", Environment.MachineName);
            writer.WriteAttributeString("username", username);

            writer.WriteElementString("message", Only30KBytes(RemoveInvalidXmlChars(message)));

            WriteLog4jElement(writer, "properties");
            WriteLog4jElement(writer, "data");
            writer.WriteAttributeString("name", "log4netmachinename");
            writer.WriteAttributeString("value", Environment.MachineName);
            writer.WriteEndElement();


            writer.WriteEndElement();
            if (_message.Exception != null) writer.WriteElementString("exception", GetExceptionString(_message.Exception));

            writer.WriteEndElement();

            writer.Flush();
            return builder.ToString();
        }

        private string Only30KBytes(string message) {
            if (message.Length > 30000) {
                message = message.Substring(0, 30000) + "...";
            }
            return message;
        }

        private string RemoveInvalidXmlChars(string text) {
            var validXmlChars = text.Where(x => XmlConvert.IsXmlChar(x)).ToArray();
            return new string(validXmlChars);
        }

        private void WriteLog4jElement(XmlWriter writer, string name) {
            //writer.WriteStartElement(_xmlPrefix, name, _xmlNamespace);
            writer.WriteStartElement(name);
        }
    }
}