// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeClient.cs" company="">
//   
// </copyright>
// <summary>
//   A simple class for connecting to the named-pipe server.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[module:
    SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", 
        Justification = "Reviewed. Suppression is OK here.")]

namespace NamedPipeChannel
{
    using System;
    using System.IO.Pipes;
    using System.Text;
    using System.Threading;

    /// <summary>
    ///     A simple class for connecting to the named-pipe server.
    /// </summary>
    public sealed class PipeClient : IDisposable
    {
        #region Fields

        private bool disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeClient"/> class.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the pipe.
        /// </param>
        /// <param name="pipeDirection">
        /// Determines the direction of the pipe.
        /// </param>
        public PipeClient(string pipeName, PipeDirection pipeDirection)
        {
            this.ClientStream = new NamedPipeClientStream(".", pipeName, pipeDirection, PipeOptions.Asynchronous);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a message is received from the pipe server.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the client stream.
        /// </summary>
        private NamedPipeClientStream ClientStream { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Connects to the pipe server.
        /// </summary>
        /// <param name="timeout">
        /// The time to wait before timing out.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        /// <exception cref="TimeoutException">
        /// Could not connect to the server within the specified timeout period.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Timeout is less than 0 and not set to Infinite.
        /// </exception>
        public void Connect(int timeout = 1000)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(PipeClient).Name);
            }

            this.ClientStream.Connect(timeout);
            this.ClientStream.ReadMode = PipeTransmissionMode.Message;

            var clientState = new PipeClientState(this.ClientStream);
            this.ClientStream.BeginRead(
                clientState.Buffer,
                0,
                clientState.Buffer.Length, 
                this.ReadCallback, 
                clientState);
        }

        /// <summary>
        /// Sends a string to the server.
        /// </summary>
        /// <param name="value">
        /// The string to send to the server.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public void Send(string value)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(PipeClient).Name);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(value);
            this.ClientStream.BeginWrite(buffer, 0, buffer.Length, this.SendCallback, this.ClientStream);
        }

        /// <summary>
        /// The send callback.
        /// </summary>
        /// <param name="iar">
        /// The asynchronous result. 
        /// </param>
        private void SendCallback(IAsyncResult iar)
        {
            var pipeStream = (NamedPipeClientStream)iar.AsyncState;
            pipeStream.EndWrite(iar);
        }

        /// <summary>
        /// The read callback.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result.
        /// </param>
        private void ReadCallback(IAsyncResult ar)
        {
            var pipeState = (PipeClientState)ar.AsyncState;

            int received = pipeState.PipeClient.EndRead(ar);
            string stringData = Encoding.UTF8.GetString(pipeState.Buffer, 0, received);
            pipeState.Message.Append(stringData);
            if (pipeState.PipeClient.IsMessageComplete)
            {
                this.OnMessageReceived(new MessageReceivedEventArgs(pipeState.Message.ToString()));
                pipeState.Message.Clear();
            }

            if (pipeState.PipeClient.IsConnected)
            {
                pipeState.PipeClient.BeginRead(pipeState.Buffer, 0, 255, this.ReadCallback, pipeState);
            }
        }

        /// <summary>
        /// Trigger the message received event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Dispose the pipe client.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.ClientStream.Dispose();
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}