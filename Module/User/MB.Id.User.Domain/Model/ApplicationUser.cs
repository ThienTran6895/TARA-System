using Microsoft.AspNet.Identity;
using System;


namespace MB.OMS.Account.Domain.Model
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, 
    //please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    
    public class ApplicationUser : IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool Visible { get; set; }
        public DateTime BirthDate { get; set; }
        
        public ApplicationUser()
        {
            CreatedDate = DateTime.Now;           
        }

        public string Id
        {
            get;
            set;
        }

        public string UserName
        {
            get
            {
                return "duong";
            }
            set
            {
               ;
            }
        }
        

        public int Total { get; set; }
    }
 
}