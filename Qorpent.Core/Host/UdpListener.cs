using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Host {
    public class UpdTraceListener : TraceListener {
        readonly IPEndPoint _endPoint;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="initializeData"></param>
        public UpdTraceListener(string initializeData) {
            try {
                IPAddress addressGroup = null;
                int groupPort = 0;

                string[] arguments = initializeData.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string argument in arguments) {
                    string[] items = argument.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string name = items[0];
                    string value = items.Length > 1 ? items[1] : string.Empty;

                    switch (name) {
                        case "ip":
                            addressGroup = IPAddress.Parse(value);
                            break;
                        case "port":
                            groupPort = int.Parse(value);
                            break;
                    }
                }

                if (addressGroup == null || groupPort == 0)
                    throw new ArgumentException(
                        "ip or port have not valid values.",
                        "initializeData");

                _endPoint = new IPEndPoint(addressGroup, groupPort);
            } catch (Exception e) {
                throw new ArgumentException(
                    "UpdTraceListener: Invalid or malformed initialize data (ip=<ip>;port=<port>;).",
                    "initializeData", e);
            }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
        public override void Write(string message) {
            // HACK:
            message = message.Replace("{", "<").Replace("}", ">");

            byte[] data = Encoding.Default.GetBytes(message);
			try {
				UdpHelper.SendToMulticastGroup(_endPoint, data);
			} catch {
				
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
        public override void WriteLine(string message) {
            string newMessage = message + Environment.NewLine;
            Write(newMessage);
        }
    }
}
