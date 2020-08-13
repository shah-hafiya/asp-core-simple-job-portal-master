using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.ViewModels.Home
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsThirdPartyClient { get; set; }
    }

    public class UserWithRoleDto
    {
        public UserDto User { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; }
    }

    public class RoleDto
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }


}
