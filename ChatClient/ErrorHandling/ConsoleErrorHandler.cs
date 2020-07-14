using System;

namespace ChatClient.ErrorHandling
{
    class ConsoleErrorHandler : IErrorHandler
    {
        public void HandleError(Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
