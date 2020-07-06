using System.Net;

namespace EmployeeManagementCommon
{
    public class ResultOrHttpError<TResult, TError>
    {
        public ResultOrHttpError(TResult result)
        {
            this.Result = result;
            this.IsError = false;
        }

        public ResultOrHttpError(TError error)
        {
            this.Error = error;
            this.IsError = true;
        }
        public ResultOrHttpError(TError error, HttpStatusCode status)
        {
            this.Error = error;
            this.IsError = true;
            this.StatusCode = status;
        }

        public TResult Result { get; }
        public TError Error { get; }
        public bool IsError { get; }
        public HttpStatusCode? StatusCode { get; }
    }
}
