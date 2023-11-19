// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageReceivedArgs.cs" company="">
//   
// </copyright>
// <summary>
//   Message received event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NamedPipeChannel
{
    using System;

    /// <summary>
    ///     Message received event arguments.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message received.
        /// </param>
        public MessageReceivedEventArgs(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }

                throw new ArgumentException("Empty string.", "message");
            }

            this.Message = message;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the message received from the named-pipe.
        /// </summary>
        public string Message { get; private set; }

        #endregion
    }
}