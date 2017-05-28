﻿namespace VSPCleaner.Infrastructure.ProgressDialog
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ProgressDialogCancellationExcpetion : Exception
    {
        public ProgressDialogCancellationExcpetion()
        {
        }

        public ProgressDialogCancellationExcpetion(string message)
            : base(message)
        {
        }

        public ProgressDialogCancellationExcpetion(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ProgressDialogCancellationExcpetion(string format, params object[] arg)
            : base(string.Format(format, arg))
        {
        }

        protected ProgressDialogCancellationExcpetion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}