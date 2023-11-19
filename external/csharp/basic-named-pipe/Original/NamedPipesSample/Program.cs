namespace NamedPipesSample
{
    using System;
    using System.Collections.Generic;
    using System.IO.Pipes;
    using System.Linq;

    using NamedPipeChannel;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            var pipeServer = new PipeServer("demo", PipeDirection.InOut);
            pipeServer.MessageReceived += (s, o) => pipeServer.Send(o.Message); 
            pipeServer.Start();

            var pipeClient = new PipeClient("demo", PipeDirection.InOut);
            pipeClient.MessageReceived += (s, o) => Console.WriteLine("Client Received: value: {0}", o.Message);
            pipeClient.Connect();

            while (true)
            {
                string s = GetString().First();

                pipeClient.Send(s);
            }
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private static IEnumerable<string> GetString()
        {
            string line;
            while (null != (line = Console.ReadLine()))
            {
                yield return line;
            }
        }
    }
}
