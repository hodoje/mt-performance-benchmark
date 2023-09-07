namespace Consumer.Exceptions
{
    /// <summary>
    /// Exception used for differentiating our custom exceptions from system exceptions.
    /// </summary>
    public abstract class CustomException : Exception
    {
        protected CustomException() : base()
        {
        }

        protected CustomException(string message) :
            base(message)
        {
        }
    }
}
