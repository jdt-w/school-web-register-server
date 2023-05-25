namespace SchoolWebRegister.Services.GraphQL
{
    public class GraphQLErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            error = error.RemoveExtensions().RemoveLocations();
            return error.WithMessage(error.Message);
        }
    }
}
