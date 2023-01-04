using Employee_Api.Identity;
using Employee_Api.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Api.ServiceContract
{
  public  interface IUserService
    {
        Task<ApplicationUser> Authenticate(Loginviewmodel loginviewmodel);
    }
}
