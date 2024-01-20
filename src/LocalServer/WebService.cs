using Newtonsoft.Json;
using Nightingale.Core.Mock.Models;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Models;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer
{
    /// <remarks>
    /// Source: http://www.gabescode.com/dotnet/2018/11/01/basic-HttpListener-web-service.html
    /// </remarks>
    internal static class WebService
    {
        /// <summary>
        /// The port the HttpListener should listen on
        /// </summary>
        private const int Port = 1337;

        /// <summary>
        /// This is the heart of the web server
        /// </summary>
        private static readonly HttpListener Listener = new HttpListener { Prefixes = { $"http://localhost:{Port}/" } };

        /// <summary>
        /// A flag to specify when we need to stop
        /// </summary>
        private static bool _keepGoing = true;

        /// <summary>
        /// Keep the task in a static variable to keep it alive
        /// </summary>
        private static Task _mainLoop;

        private static readonly IRequestProcessor _requestProcessor = new RequestProcessor();

        /// <summary>
        /// Call this to start the web server
        /// </summary>
        public static void StartWebServer(ServerConfiguration config, DocumentFile ncf)
        {
            Console.WriteLine("Initializing...");

            try
            {
                _requestProcessor.Initialize(config, ncf);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
                throw;
            }

            Console.WriteLine("Ready!");
            Console.WriteLine();
            Console.WriteLine("Send requests to http://localhost:" + Port.ToString());

            if (_mainLoop != null && !_mainLoop.IsCompleted) return; //Already started
            _mainLoop = MainLoop();
        }

        /// <summary>
        /// Call this to stop the web server. It will not kill any requests currently being processed.
        /// </summary>
        public static void StopWebServer()
        {
            _keepGoing = false;
            lock (Listener)
            {
                //Use a lock so we don't kill a request that's currently being processed
                Listener.Stop();
            }
            try
            {
                _mainLoop.Wait();
            }
            catch { /* je ne care pas */ }
        }

        /// <summary>
        /// The main loop to handle requests into the HttpListener
        /// </summary>
        /// <returns></returns>
        private static async Task MainLoop()
        {
            Listener.Start();
            while (_keepGoing)
            {
                try
                {
                    //GetContextAsync() returns when a new request come in
                    var context = await Listener.GetContextAsync();
                    lock (Listener)
                    {
                        if (_keepGoing) ProcessRequest(context);
                    }
                }
                catch (Exception e)
                {
                    if (e is HttpListenerException) return; //this gets thrown when the listener is stopped
                    //TODO: Log the exception
                }
            }
        }

        /// <summary>
        /// Handle an incoming request
        /// </summary>
        /// <param name="context">The context of the incoming request</param>
        private static void ProcessRequest(HttpListenerContext context)
        {
            Console.Write($"{context.Request.HttpMethod} {context.Request.Url.AbsolutePath}... ");

            using (var response = context.Response)
            {
                try
                {
                    var handled = false;

                    MockData returnValue = _requestProcessor.GetReturnValue(context.Request.Url.AbsolutePath, context.Request.HttpMethod);
                    if (returnValue != null)
                    {
                        response.StatusCode = returnValue.StatusCode ?? 0;
                        response.ContentType = returnValue.ContentType;
                        var buffer = Encoding.UTF8.GetBytes(returnValue.Body);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                        handled = true;
                    }

                    if (!handled)
                    {
                        response.StatusCode = 404;
                    }
                }
                catch (Exception e)
                {
                    //Return the exception details the client - you may or may not want to do this
                    response.StatusCode = 500;
                    response.ContentType = "application/json";
                    var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);

                    //TODO: Log the exception
                }

                Console.WriteLine(response.StatusCode);
            }
        }
    }
}
