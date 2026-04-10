namespace SumaryYoutubeBackend.Exceptions
{
    public class TranscriptServiceException : Exception
    {
        public int StatusCode { get; }
        public string Code { get; }

        public TranscriptServiceException(int statusCode, string code, string message)
            : base(message)
        {
            StatusCode = statusCode;
            Code = code;
        }
    }
}

