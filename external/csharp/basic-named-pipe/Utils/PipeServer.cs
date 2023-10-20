using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace Utils {

	public sealed class PipeServer : IDisposable {

		private bool disposed;
		private CancellationTokenSource cancellationTokenSource;
		private CancellationToken cancellationToken;
        
		public event EventHandler<MessageReceivedEventArgs> MessageReceived;
		public NamedPipeServerStream ServerStream { get; private set; }
        
		public PipeServer(string pipeName, PipeDirection pipeDirection) {
			var pipeSecurity = new PipeSecurity();
			PipeAccessRule pipeAccessRule = new System.IO.Pipes.PipeAccessRule("Everyone", System.IO.Pipes.PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
			pipeSecurity.AddAccessRule(pipeAccessRule);
			this.ServerStream = new NamedPipeServerStream(
				pipeName, 
				pipeDirection, 
				1, // maxNumberOfServerInstances
				PipeTransmissionMode.Message, 
				PipeOptions.Asynchronous, 
				1024, // inBufferSize 
				1024, // outBufferSize
				pipeSecurity );
        	
			this.cancellationTokenSource = new CancellationTokenSource();
		}

		public void Start() {
			Start(cancellationTokenSource.Token);
		}
        
		public void Start(CancellationToken token) {
			if (disposed) {
				throw new ObjectDisposedException(typeof(PipeServer).Name);
			}

			var state = new PipeServerState(this.ServerStream, token);
			this.ServerStream.BeginWaitForConnection(this.ConnectionCallback, state);
		}

		private void ConnectionCallback(IAsyncResult asyncResult) {
			var pipeServer = (PipeServerState)asyncResult.AsyncState;
			pipeServer.PipeServer.EndWaitForConnection(asyncResult);

			pipeServer.PipeServer.BeginRead(pipeServer.Buffer, 0, 255, this.ReadCallback, pipeServer);
		}

		public void Stop() {
			if (this.disposed) {
				throw new ObjectDisposedException(typeof(PipeServer).Name);
			}

			this.cancellationTokenSource.Cancel();
		}

		public void Send(string value) {
			if (this.disposed) {
				throw new ObjectDisposedException(typeof(PipeClient).Name);
			}

			byte[] buffer = Encoding.UTF8.GetBytes(value);
			this.ServerStream.BeginWrite(buffer, 0, buffer.Length, this.SendCallback, this.ServerStream);
		}

		private void SendCallback(IAsyncResult asyncResult) {
			var pipeStream = (NamedPipeServerStream)asyncResult.AsyncState;
			pipeStream.EndWrite(asyncResult);
		}

		private void ReadCallback(IAsyncResult asyncResult) {
			var pipeState = (PipeServerState)asyncResult.AsyncState;

			int received = pipeState.PipeServer.EndRead(asyncResult);
			string stringData = Encoding.UTF8.GetString(pipeState.Buffer, 0, received);
			pipeState.Message.Append(stringData);
			if (pipeState.PipeServer.IsMessageComplete) {
				this.OnMessageReceived(new MessageReceivedEventArgs(stringData));
				pipeState.Message.Clear();
			}

			if (!(this.cancellationToken.IsCancellationRequested || pipeState.ExternalCancellationToken.IsCancellationRequested)) {
				if (pipeState.PipeServer.IsConnected) {
					pipeState.PipeServer.BeginRead(pipeState.Buffer, 0, 255, this.ReadCallback, pipeState);
				} else {
					pipeState.PipeServer.BeginWaitForConnection(this.ConnectionCallback, pipeState);
				}
			}
		}

		private void OnMessageReceived(MessageReceivedEventArgs e) {
			EventHandler<MessageReceivedEventArgs> handler = this.MessageReceived;
			if (handler != null) {
				handler(this, e);
			}
		}

		public void Dispose() {
			if (!this.disposed) {
				this.cancellationTokenSource.Dispose();
				this.ServerStream.Dispose();

				this.disposed = true;
			}
		}

	}
}