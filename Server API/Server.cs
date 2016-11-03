using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Server_API
{

    // Used to store the data we want from the API request
    class MapData
    {
        public IList<double> latitude { get; set; }
        public IList<double> longitude { get; set; }
        public string polyline { get; set; }
        public int totalDistance { get; set; }
    }

    

    class Server
    {
 
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> responseMethod;


        public Server(string[] prefix, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Your system OS is not supported");

            if (prefix == null || prefix.Length == 0)
                throw new ArgumentException("Prefixes cannot be null");

            if (method == null)
                throw new ArgumentException("Method cannot be null");

            foreach (string s in prefix)
                _listener.Prefixes.Add(s);

            responseMethod = method;
            
            _listener.Start();

        }

        public Server(Func<HttpListenerRequest, string> method, params string[] prefix)
            : this(prefix, method)
        {}

        public void StartListening()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Listening...");

                while(_listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem((c) =>
                    {
                        var context = c as HttpListenerContext;
                        try
                        {
                            string responseStr = responseMethod(context.Request);
                            byte[] buffer = Encoding.UTF8.GetBytes(responseStr);
                            context.Response.ContentLength64 = buffer.Length;
                            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        }
                        catch { }
                        finally
                        {
                            context.Response.Close();
                        }

                    }, _listener.GetContext());
                }
            });
        }
    }
}
