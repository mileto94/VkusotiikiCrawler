using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class SocketListener
    {
        private TcpListener _server;
        private TcpClient _client;

        public void Start(string listenerIPAddress, int listenPort)
        {
            _server = new TcpListener(IPAddress.Parse(listenerIPAddress), listenPort);
            _server.Start();
        }

        //public void Stop()
        //{
        //    _server.Stop();
        //}

        public void HandleRequest(Action<StreamWriter, string> requestHandler)
        {
            Console.WriteLine($"The server connected to issue JsonRpc requests from {_server.LocalEndpoint}");
            while (true)
            {
                try
                {
                    using (_client = _server.AcceptTcpClient())
                    {
                        using (var stream = _client.GetStream())
                        {
                            Console.WriteLine("Client connected..");
                            var reader = new StreamReader(stream, Encoding.UTF8);
                            var writer = new StreamWriter(stream, new UTF8Encoding(false));
                            string line = null;
                            while (!reader.EndOfStream)
                            //while(line == null)
                            {
                                line = reader.ReadLine();
                                requestHandler(writer, line);

                                Console.WriteLine($"REQUEST: {line}");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("RPCServer exception " + e);
                }
            }
        }
    }
}
