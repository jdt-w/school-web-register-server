﻿using HotChocolate.Types;

namespace SchoolWebRegister.Domain
{
    [InterfaceType("BaseResponse")]
    public interface IBaseResponse<T>
    {
        T? Data { get; }
    }

    public struct BaseResponse : IBaseResponse<object>
    {
        public string? Description { get; set; }
        public StatusCode StatusCode { get; set; }
        public object? Data { get; set; }
    }
    public struct BaseResponse<T> : IBaseResponse<T?>
    {
        public string? Description { get; set; }
        public StatusCode StatusCode { get; set; }
        public T? Data { get; set; }
    }
}
