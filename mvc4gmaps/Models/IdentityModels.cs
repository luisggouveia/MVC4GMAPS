using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace MVC4GMAPS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string UserEmail { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            //: base("DefaultConnection")
            : base("LocationDBContext")
        {
        }
    }
}