using Employee_Api.Applicationdbcontext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Api.Identity
{
    public class ApplicationRoleStore:RoleStore<ApplicationRole,ApplicationDbContext>
    {
        public ApplicationRoleStore(ApplicationDbContext context,IdentityErrorDescriber errorDescriber):base(context,errorDescriber)
        {

        }
    }
}
