using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Text;

[module:
    SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", 
        Justification = "Reviewed. Suppression is OK here.")]

namespace Utils {

    internal class PipeClientState
    {
        private const int BufferSize = 8125;
        public PipeClientState(NamedPipeClientStream pipeServer)
            : this(pipeServer, new byte[BufferSize])
        {
        }

        public PipeClientState(NamedPipeClientStream pipeServer, byte[] buffer)
        {
            this.PipeClient = pipeServer;
            this.Buffer = buffer;
            this.Message = new StringBuilder();
        }

        public byte[] Buffer { get; private set; }

        public NamedPipeClientStream PipeClient { get; private set; }

        public StringBuilder Message { get; private set; }

    }
}