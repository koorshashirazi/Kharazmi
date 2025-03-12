using System;

namespace Kharazmi.AspNetCore.Core.Functional
{
    public class ResultSuccessException : Exception
    {
        internal ResultSuccessException() : base(ResultMessages.ErrorIsInaccessibleForSuccess)
        {
        }
    }

    public class ResultFailureException : Exception
    {
        internal ResultFailureException(string error) : base(ResultMessages.ValueIsInaccessibleForFailure)
        {
            Error = error;
        }

        public string Error { get; }
    }

    public class ResultFailureException<TE> : ResultFailureException
    {
        internal ResultFailureException(TE error) : base(ResultMessages.ValueIsInaccessibleForFailure)
        {
            Error = error;
        }

        public new TE Error { get; }
    }
}