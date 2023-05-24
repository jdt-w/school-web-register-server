using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;

namespace SchoolWebRegister.Services.GraphQL
{
    public class GraphQLResponseFormatter : DefaultHttpResponseFormatter
    {
        public GraphQLResponseFormatter(HttpResponseFormatterOptions options) : base(options) { }

        protected override HttpStatusCode OnDetermineStatusCode(
            IQueryResult result, FormatInfo format,
            HttpStatusCode? proposedStatusCode)
        {
            //var data = result.ContextData.FirstOrDefault(x => x.Key.Equals("HotChocolate.Execution.Transport.HttpStatusCode"));
            //if (Enum.TryParse(typeof(HttpStatusCode), data.Value.ToString(), out object? statusCode))
            //{
            //    return (HttpStatusCode)statusCode;
            //}

            // In all other cases let Hot Chocolate figure out the
            // appropriate status code.
            return base.OnDetermineStatusCode(result, format, proposedStatusCode);
        }
    }

}
