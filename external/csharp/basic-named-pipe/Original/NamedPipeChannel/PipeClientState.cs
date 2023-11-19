// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeClientState.cs" company="">
//   
// </copyright>
// <summary>
//   Asynchronous state for the pipe client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

[module:
    SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", 
        Justification = "Reviewed. Suppression is OK here.")]

namespace NamedPipeChannel
{
    using System.IO.Pipes;
    using System.Text;

    /// <summary>
    ///     Asynchronous state for the pipe client.
    /// </summary>
    internal class PipeClientState
    {
        #region Fields

        private const int BufferSize = 8125;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeClientState"/> class.
        /// </summary>
        /// <param name="pipeServer">
        /// The pipe server instance.
        /// </param>
        public PipeClientState(NamedPipeClientStream pipeServer)
            : this(pipeServer, new byte[BufferSize])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeClientState"/> class.
        /// </summary>
        /// <param name="pipeServer">
        /// The pipe server instance.
        /// </param>
        /// <param name="buffer">
        /// The byte buffer.
        /// </param>
        public PipeClientState(NamedPipeClientStream pipeServer, byte[] buffer)
        {
            this.PipeClient = pipeServer;
            this.Buffer = buffer;
            this.Message = new StringBuilder();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the byte buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets the pipe server.
        /// </summary>
        public NamedPipeClientStream PipeClient { get; private set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public StringBuilder Message { get; private set; }

        #endregion
    }
}