namespace SchoolWebRegister.Domain
{
    public interface IBaseResponse<T>
    {
        T Data { get; }
    }

    public struct BaseResponse<T> : IBaseResponse<T?>
    {
        public string Description { get; set; }
        public StatusCode StatusCode { get; set; }
        public T? Data { get; set; }
    }
}
