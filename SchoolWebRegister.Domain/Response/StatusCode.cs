namespace SchoolWebRegister.Domain
{
    public enum StatusCode : ushort
    {
        // Users
        UserNotFound = 0,

        // HTTPs
        Successful = 200,
        NoContent = 204,
        BadRequest = 400,
        Forbidden = 403,
        Unauthorized = Forbidden,
        RequestTimeout = 408,
        InternalServerError = 500,
    }
}
