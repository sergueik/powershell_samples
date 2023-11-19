using System;

namespace Utils {

	public class MessageReceivedEventArgs : EventArgs {
		public MessageReceivedEventArgs(string message) {
			if (string.IsNullOrEmpty(message)) {
				if (message == null) {
					throw new ArgumentNullException( "message");
				}
			     throw new ArgumentException("Empty string.", "message");
			}
			this.Message = message;
		}

		public string Message { get; private set; }
	}
}