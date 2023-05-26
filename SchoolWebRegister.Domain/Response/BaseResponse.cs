namespace SchoolWebRegister.Domain
{
    public struct ErrorType
    {
        public string[] Type { get; set; }
        public string Message { get; set; }
        public object CustomData { get; set; }

        public static implicit operator ErrorType(string type)
        {
            return new ErrorType { Type = new string[] { type } };
        }
        public static implicit operator ErrorType(string[] types)
        {
            return new ErrorType { Type = types };
        }
    }

    public enum StatusCode { Success, Error }
    public interface IBaseResponse<T>
    {
        public string Status { get; set; }
        T? Data { get; }
        public object? Error { get; set; }
    }
    public struct BaseResponse : IBaseResponse<object>
    {
        public string Status { get; set; }
        public object? Data { get; set; }
        public object? Error { get; set; }

        public BaseResponse(StatusCode code, object data = null, object error = null) 
        {
            Status = code.ToString();
            Data = data;
            Error = error;
        }
        public BaseResponse(string code, object data = null, object error = null)
        {
            Status = code;
            Data = data;
            Error = error;
        }
    }
    public struct BaseResponse<T> : IBaseResponse<T?>
    {
        public string Status { get; set; }
        public T? Data { get; set; }
        public object Error { get; set; }
        public string Message { get; set; }

        public BaseResponse(StatusCode code, T? data, object error = null)
        {
            Status = code.ToString();
            Data = data;
            Error = error;
        }
    }
}
