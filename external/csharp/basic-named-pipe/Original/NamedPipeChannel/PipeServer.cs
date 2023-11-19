// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeServer.cs" company="">
//   
// </copyright>
// <summary>
//   A simple named-pipe server.
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
    /// A simple named-pipe server.
    /// </summary>
    public sealed class PipeServer : IDisposable
    {
        #region Fields

        private bool disposed;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeServer"/> class.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the pipe.
        /// </param>
        /// <param name="pipeDirection">
        /// Determines the direction of the pipe.
        /// </param>
        public PipeServer(string pipeName, PipeDirection pipeDirection)
        {
            this.ServerStream = new NamedPipeServerStream(
                pipeName, 
                pipeDirection, 
                1, 
                PipeTransmissionMode.Message, 
                PipeOptions.Asynchronous);
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a message is received from the named pipe.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the named-pipe server stream.
        /// </summary>
        public NamedPipeServerStream ServerStream { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Start the pipe server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object is disposed.</exception>
        public void Start()
        {
            this.Start(this.cancellationTokenSource.Token);
        }
        
        /// <summary>
        /// Start the pipe server.
        /// </summary>
        /// <param name="token"></param>
        public void Start(CancellationToken token)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(PipeServer).Name);
            }

            var state = new PipeServerState(this.ServerStream, token);
            this.ServerStream.BeginWaitForConnection(this.ConnectionCallback, state);
        }

        /// <summary>
        /// The connection callback.
        /// </summary>
        /// <param name="ar">
        /// The ar.
        /// </param>
        private void ConnectionCallback(IAsyncResult ar)
        {
            var pipeServer = (PipeServerState)ar.AsyncState;
            pipeServer.PipeServer.EndWaitForConnection(ar);

            pipeServer.PipeServer.BeginRead(pipeServer.Buffer, 0, 255, this.ReadCallback, pipeServer);
        }

        /// <summary>
        /// Stops the pipe server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object is disposed.</exception>
        public void Stop()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(PipeServer).Name);
            }

            this.cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Sends a string to the client.
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
            this.ServerStream.BeginWrite(buffer, 0, buffer.Length, this.SendCallback, this.ServerStream);
        }

        /// <summary>
        /// The send callback.
        /// </summary>
        /// <param name="iar">
        /// The iar.
        /// </param>
        private void SendCallback(IAsyncResult iar)
        {
            var pipeStream = (NamedPipeServerStream)iar.AsyncState;
            pipeStream.EndWrite(iar);
        }

        /// <summary>
        /// The read callback.
        /// </summary>
        /// <param name="ar">
        /// The ar.
        /// </param>
        private void ReadCallback(IAsyncResult ar)
        {
            var pipeState = (PipeServerState)ar.AsyncState;

            int received = pipeState.PipeServer.EndRead(ar);
            string stringData = Encoding.UTF8.GetString(pipeState.Buffer, 0, received);
            pipeState.Message.Append(stringData);
            if (pipeState.PipeServer.IsMessageComplete)
            {
                this.OnMessageReceived(new MessageReceivedEventArgs(stringData));
                pipeState.Message.Clear();
            }

            if (!(this.cancellationToken.IsCancellationRequested || pipeState.ExternalCancellationToken.IsCancellationRequested))
            {
                if (pipeState.PipeServer.IsConnected)
                {
                    pipeState.PipeServer.BeginRead(pipeState.Buffer, 0, 255, this.ReadCallback, pipeState);
                }
                else
                {
                    pipeState.PipeServer.BeginWaitForConnection(this.ConnectionCallback, pipeState);
                }
            }
        }

        /// <summary>
        /// The on message received.
        /// </summary>
        /// <param name="e">
        /// The e.
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
        /// Disposes the pipe server.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.cancellationTokenSource.Dispose();
                this.ServerStream.Dispose();

                this.disposed = true;
            }
        }

        #endregion
    }
}