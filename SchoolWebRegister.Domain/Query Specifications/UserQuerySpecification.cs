using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Domain.Specifications;
using SchoolWebRegister.Domain.Helpers;

namespace SchoolWebRegister.Domain.QuerySpecifications
{
    public class UserQuerySpecification : BaseSpecification<ApplicationUser>
    {
        public UserQuerySpecification(IQueryCollection query)
        {
            var names = query[nameof(ApplicationUser.UserName).ToLower()];
            var ids = query[nameof(ApplicationUser.Id).ToLower()];
            var emails = query[nameof(ApplicationUser.Email).ToLower()];

            Expression<Func<ApplicationUser, bool>> result = null;
            foreach (string name in names)
            {
                result = result.Or((user) => user.UserName.Equals(name));
            }            

            Expression<Func<ApplicationUser, bool>> filter1 = null;
            foreach (string id in ids)
            {
                filter1 = filter1.Or((user) => user.Id.Equals(id));
            }

            Expression<Func<ApplicationUser, bool>> filter2 = null;
            foreach (string email in emails)
            {
                filter2 = filter2.Or((user) => user.Email.Equals(email));
            }

            result = result.Or(filter1);
            result = result.Or(filter2);
            Criteria = result;
        }
    }
}
