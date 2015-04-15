﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using System.Reflection;

namespace Qorpent.Host {
    /// <summary>
    /// 
    /// </summary>
    public class UdpTraceListener : TraceListener {
        private UdpClient _udpClient;
        private ConcurrentStack<string> _messageBuffer;
        private string _loggerName;

        /// <summary>
        /// 
        /// </summary>
        public override bool IsThreadSafe {
            get {
                return true;
            }
        }

        public string GetExceptionString(Exception ex) {
            return ex.GetType().ToString() + ": " + ex.Message;
        }

        /// <summary>
        /// 
        /// </summary>
        public UdpTraceListener()
            : this("localhost", 7071, "UdpTraceListener") {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="loggerName"></param>
        public UdpTraceListener(string host, int port, string loggerName) {
            _loggerName = loggerName;
            _messageBuffer = new ConcurrentStack<string>();
            _udpClient = new UdpClient();
            _udpClient.Connect(host, port);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message) {
            Write(message, "info");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message) {
            Write(message, "info");
            Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public override void WriteLine(string message, string category) {
            _messageBuffer.Push(GetEventXml(message, category));
            Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        public override void Write(string message, string category) {
            _messageBuffer.Push(GetEventXml(message, category));
        }

        public string GetEventXml(string message, string category) {
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


            string level = "INFO";
            if (string.IsNullOrEmpty(category))
                category = "info";

            switch (category.ToLower()) {
                case "fatal":
                    level = "FATAL";
                    break;

                case "warning":
                case "warn":
                    level = "WARN";
                    break;

                case "error":
                    level = "ERROR";
                    break;

                case "debug":
                    level = "DEBUG";
                    break;

                case "trace":
                    level = "TRACE";
                    break;

                default:
                    break;
            }

            StringBuilder builder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            XmlWriter writer = XmlWriter.Create(builder, settings);
            WriteLog4jElement(writer, "event");
            writer.WriteAttributeString("logger", _loggerName);
            writer.WriteAttributeString("timestamp", DateTime.Now.ToString("o"));

            writer.WriteAttributeString("level", level);
            writer.WriteAttributeString("thread", Thread.CurrentThread.ManagedThreadId.ToString());
            writer.WriteAttributeString("domain", "todo");
            writer.WriteAttributeString("username", "todo\\username");

            writer.WriteElementString("message", Only30KBytes(RemoveInvalidXmlChars(message)));

            WriteLog4jElement(writer, "properties");
            WriteLog4jElement(writer, "data");
            writer.WriteAttributeString("name", "log4jmachinename");
            writer.WriteAttributeString("value", Environment.MachineName);
            writer.WriteEndElement();


            

            writer.WriteEndElement();

            if (level == "ERROR") {
                writer.WriteElementString("exception", GetExceptionString(new NullReferenceException("fuck")));
            }

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

        private void WriteLog4jElementString(XmlWriter writer, string name, string value) {
            //writer.WriteElementString(_xmlPrefix, name, _xmlNamespace, value);
            writer.WriteElementString(name,value);
        }

        private double ConvertToUnixTimestamp(DateTime date) {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan sinceEpoch = date.ToUniversalTime() - epoch;
            return Math.Floor(sinceEpoch.TotalMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush() {
            foreach (string xmlMessage in _messageBuffer) {
                byte[] payload = Encoding.UTF8.GetBytes(xmlMessage);
                _udpClient.Send(payload, payload.Length);
            }

            _messageBuffer.Clear();
            base.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            Flush();
            _udpClient.Close();

            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventCache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args) {
            switch (eventType) {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    WriteLine(string.Format(format, args), "error");
                    break;

                case TraceEventType.Verbose:
                    WriteLine(string.Format(format, args), "debug");
                    break;

                case TraceEventType.Warning:
                    WriteLine(string.Format(format, args), "warn");
                    break;

                case TraceEventType.Information:
                case TraceEventType.Resume:
                case TraceEventType.Start:
                case TraceEventType.Stop:
                case TraceEventType.Suspend:
                case TraceEventType.Transfer:
                default:
                    WriteLine(string.Format(format, args), "error");
                    break;
            }
        }
    }



}
