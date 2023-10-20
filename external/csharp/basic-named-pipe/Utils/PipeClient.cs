using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace Utils {

	public sealed class PipeClient : IDisposable {
		private bool disposed;

		public PipeClient(string pipeName, PipeDirection pipeDirection){
			this.ClientStream = new NamedPipeClientStream(".", pipeName, pipeDirection, PipeOptions.Asynchronous);
		}

		public event EventHandler<MessageReceivedEventArgs> MessageReceived;

		private NamedPipeClientStream ClientStream { get; set; }

		public void Connect(int timeout = 1000) {
			if (this.disposed) {
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

		public void Send(string value) {
			if (this.disposed) {
				throw new ObjectDisposedException(typeof(PipeClient).Name);
			}

			byte[] buffer = Encoding.UTF8.GetBytes(value);
			this.ClientStream.BeginWrite(buffer, 0, buffer.Length, this.SendCallback, this.ClientStream);
		}
        
		private void SendCallback(IAsyncResult asyncResult) {
			var pipeStream = (NamedPipeClientStream)asyncResult.AsyncState;
			pipeStream.EndWrite(asyncResult);
		}

		private void ReadCallback(IAsyncResult asyncResult) {
			var pipeState = (PipeClientState)asyncResult.AsyncState;

			int received = pipeState.PipeClient.EndRead(asyncResult);
			string stringData = Encoding.UTF8.GetString(pipeState.Buffer, 0, received);
			pipeState.Message.Append(stringData);
			if (pipeState.PipeClient.IsMessageComplete) {
				this.OnMessageReceived(new MessageReceivedEventArgs(pipeState.Message.ToString()));
				pipeState.Message.Clear();
			}

			if (pipeState.PipeClient.IsConnected) {
				pipeState.PipeClient.BeginRead(pipeState.Buffer, 0, 255, this.ReadCallback, pipeState);
			}
		}

		private void OnMessageReceived(MessageReceivedEventArgs e) {
			EventHandler<MessageReceivedEventArgs> handler = this.MessageReceived;
			if (handler != null) {
				handler(this, e);
			}
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if (!this.disposed) {
				if (disposing) {
					this.ClientStream.Dispose();
				}

				this.disposed = true;
			}
		}

	}
}