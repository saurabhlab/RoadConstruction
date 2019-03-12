using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.ViewModel.Common
{
    public class ResponseViewModel
    {
        public long Id { get; set; }
        public long Code { get; set; }
        public string Status { get; set; }
        /// <summary>
        ///     Gets or sets a value indicating whether this instance is success.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>

        public string Message { get; set; }


        public bool ReturnStatus { get; set; }
        public List<string> ReturnMessage { get; set; }

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>
        ///     The content.
        /// </value>
        public object Content { get; set; }
    }
}