namespace Consumer.Exceptions
{
    public class InfrastructureFailureException : CustomException
    {
        public InfrastructureFailureException(string message)
            : base(message)
        {

        }
    }
}
