using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Utils {

    internal class PipeServerState {
        private const int BufferSize = 8125;

        public PipeServerState(NamedPipeServerStream pipeServer, CancellationToken token)
            : this(pipeServer, new byte[BufferSize], token) {
        }

        public PipeServerState(NamedPipeServerStream pipeServer, byte[] buffer, CancellationToken token) {
            this.PipeServer = pipeServer;
            this.Buffer = buffer;
            this.ExternalCancellationToken = token;
            this.Message = new StringBuilder();
        }

        public byte[] Buffer { get; private set; }
        public NamedPipeServerStream PipeServer { get; private set; }
        public CancellationToken ExternalCancellationToken { get; private set; }
        public StringBuilder Message { get; private set; }

    }
}