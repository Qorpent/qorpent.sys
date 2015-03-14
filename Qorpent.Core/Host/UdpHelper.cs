using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Host {
    static class UdpHelper {
        /// <summary>
        /// http://blogs.ugidotnet.org/markino/articles/14685.aspx
        /// </summary>
        class MulticastUdpClient : UdpClient {
            /// <summary>
            /// Multicast is the term used to describe communication where a piece of information is sent from one or more 
            /// points to a set of other points. In this case there is may be one or more senders, and the information is 
            /// distributed to a set of receivers (theer may be no receivers, or any other number of receivers).
            /// </summary>
            /// <param name="ipEndPoint"></param>
            public MulticastUdpClient(IPEndPoint ipEndPoint) {
                //http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winsock/winsock/setsockopt_2.asp
                //SO_REUSEADDR (BOOL) Allows the socket to be bound to an address that is already in use.  
                Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                Client.Bind(new IPEndPoint(IPAddress.Any, ipEndPoint.Port));
                JoinMulticastGroup(ipEndPoint.Address);
            }
        }

        /// <summary>
        /// Creates an udp client to receice messages from multicast group.
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <returns></returns>
        public static UdpClient CreateMulticastGroupClient(IPEndPoint ipEndPoint) {
            return new MulticastUdpClient(ipEndPoint);
        }

        /// <summary>
        /// Methods to send data to a specific endpoint.
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="bytes"></param>
        public static void SendToMulticastGroup(IPEndPoint ipEndPoint, byte[] bytes) {
            UdpClient udp = new UdpClient();
            udp.Send(bytes, bytes.Length, ipEndPoint);
        }
    }


}
