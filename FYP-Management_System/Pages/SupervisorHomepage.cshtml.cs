using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NexGen.Models;

namespace FYP_Management_System.Pages
{
    public class SupervisorHomepageModel : PageModel
    {
        private readonly AppDbContext _context;
        public string SupervisorName { get; set; }
        public SupervisorHomepageModel(AppDbContext context)
        {
            _context = context;
        }

        public class FYPInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<string> StudentNames { get; set; }
            public string Status { get; set; }
        }

        public class SupervisorInfo
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
        public List<FYPInfo> Fyps { get; set; }
        public SupervisorInfo Supervisor { get; set; }
        public void OnGet(string id)
        {
            Supervisor = _context.Users
                          .Where(user => user.Email == id)
                          .Select(user => new SupervisorInfo
                          {
                              Email = user.Email,
                              Name = user.Name
                          })
                          .FirstOrDefault();

            Fyps = (from fyps in _context.FYPs
                    where fyps.SupervisorId == Supervisor.Email
                    select new FYPInfo
                    {
                        Name = fyps.Name,
                        Description = fyps.Details,
                        Status = fyps.Status
                    }).ToList();
        }


    }
}
