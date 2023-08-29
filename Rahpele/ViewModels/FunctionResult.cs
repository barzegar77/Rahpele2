namespace Rahpele.ViewModels
{
    public class FunctionResult
    {
        public FunctionResult(bool result, string message)
        {
            Result = result;
            Message = message;
        }

        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
