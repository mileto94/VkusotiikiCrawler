using AustinHarris.JsonRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkusotiikiCrawler
{
    public class DatabaseCommunicator
    {
        private ICrawlerRecipesService _service = null;
        private SocketListener _listener;

        public string IPAddress { get; set; }
        public int Port { get; set; }

        public DatabaseCommunicator(string listenerIPAddress, int listenerPort, CrawlerRecipesService service)
        {
            _service = service;
            _listener = new SocketListener();
            IPAddress = listenerIPAddress;
            Port = listenerPort;
        }

        public void ConnectToDatabase()
        {
            // must new up an instance of the service so it can be registered to handle requests.
            //_service = new CrawlerRecipesService();

            var RPCResultHandler = new AsyncCallback(
                state =>
                {
                    var async = ((JsonRpcStateAsync)state);
                    var result = async.Result;
                    var writer = ((StreamWriter)async.AsyncState);
                    if (result != null)
                    {
                        writer.WriteLine(result);
                        writer.FlushAsync();
                    }
                });

            _listener.Start(IPAddress, Port);
            _listener.HandleRequest((writer, line) =>
           {
               var async = new JsonRpcStateAsync(RPCResultHandler, writer) { JsonRpc = line };
               JsonRpcProcessor.Process(async, writer);
           });
        }
    }
}
