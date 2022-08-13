using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using MB.OMS.Account.Repository;
using MB.Common.Helpers;


namespace MB.OMS.Account.Domain.Model
{
    public class CustomUserManager:UserManager<ApplicationUser>
    {        
        public CustomUserManager() : base(new CustomUserSore<ApplicationUser>())
        {            
            //We can retrieve Old System Hash Password and can encypt or decrypt old password using custom approach.
	    //When we want to reuse old system password as it would be difficult for all users to initiate pwd change as per Idnetity Core hashing.
            this.PasswordHasher = new OldSystemPasswordHasher();           
        }
       
        public override System.Threading.Tasks.Task<ApplicationUser> FindAsync(string userName, string password)
        {
            Task<ApplicationUser> taskInvoke = Task<ApplicationUser>.Factory.StartNew(() =>
                {
                    //First Verify Password...
                    UserRepository uRe = new UserRepository();
                    var user = uRe.GetUser(userName);
                    if (user == null || string.IsNullOrEmpty(password))
                        return null;
                    //string newPassword = this.PasswordHasher.HashPassword(password);
                    PasswordVerificationResult result = this.PasswordHasher.VerifyHashedPassword(user.Password, password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        return user;
                    }
                    return null;
                });
            return taskInvoke;
        }      
    }

    /// <summary>
    /// Use Custom approach to verify password
    /// </summary>
    //public class OldSystemPasswordHasher : PasswordHasher
    //{
    //    public override string HashPassword(string password)
    //    {
    //        return base.HashPassword(password);
    //    }

    //    public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
    //    {
    //        return base.VerifyHashedPassword(hashedPassword, providedPassword);
    //        //Here we will place the code of password hashing that is there in our current solucion.This will take cleartext anad hash
    //    //Just for demonstration purpose I always return true.	
    //        if (true)
    //        {


    //            return PasswordVerificationResult.SuccessRehashNeeded;
    //        }
    //        else
    //        {
    //            return PasswordVerificationResult.Failed;
    //        }
    //    }
    //}
}