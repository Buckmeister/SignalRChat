using System;

namespace ChatClient.ErrorHandling
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
