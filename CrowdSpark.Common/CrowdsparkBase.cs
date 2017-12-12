using System;
using System.Collections.Generic;
using System.Text;

namespace CrowdSpark.Common
{
    public enum AttachmentTypes
    {
        /// <summary>
        /// The attachment is a plain text file.
        /// </summary>
        TEXT,
        /// <summary>
        /// The attachment is a bitmap image.
        /// </summary>
        BITMAP,
        /// <summary>
        /// The attachment is a pdf document.
        /// </summary>
        PDF
    }

    public enum SparkStatus
    {
        /// <summary>
        /// The current spark has been sent and a reply is pending.
        /// </summary>
        PENDING,
        /// <summary>
        /// The current spark is approved by the recipient.
        /// </summary>
        APPROVED,
        /// <summary>
        /// The current spark is declined.
        /// </summary>
        DECLINED
    }

    public enum ResponseLogic
    {
        SUCCESS,
        ERROR_CREATING,
        ERROR_UPDATING,
        ERROR_DELETING,
        NOT_FOUND,
        ALREADY_EXISTS
    }
    
}
