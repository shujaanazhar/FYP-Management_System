using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FYP_Management_System.Pages
{
    public class AdministratorHomePageModel : PageModel
    {
        private readonly AppDbContext _context;

        public AdministratorHomePageModel(AppDbContext context)
        {
            _context = context;
        }

        public class SupervisorInfo
        {
            
            public string Email { get; set; }
            public string Name { get; set; }
            public string Domain { get; set; }
        }

        public class FypInfo
        {
            public string Name { get; set; }
            public string SupervisorID { get; set; }
            public string Details { get; set; }
            public string Domain { get; set; }
        }

        public List<FypInfo> Fyps { get; set; }
        public List<SupervisorInfo> ApprovedSupervisors { get; set; }
        public List<SupervisorInfo> AllSupervisors { get; set; }

        public void OnGet()
        {
            ApprovedSupervisors = (from supervisor in _context.Supervisors
                                   join user in _context.Users on supervisor.Email equals user.Email
                                   where supervisor.Status == "Reject"
                                   select new SupervisorInfo // Create new instances of SupervisorInfo
                                   {
                                       Email = supervisor.Email,
                                       Name = user.Name
                                   }).ToList();

            AllSupervisors = (from supervisor in _context.Supervisors
                              join user in _context.Users on supervisor.Email equals user.Email
                              where supervisor.Status == "Approved"
                              select new SupervisorInfo
                              {
                                  Email = supervisor.Email,
                                  Name = supervisor.Role + " " + user.Name,
                                  Domain = supervisor.Domain
                              }).ToList();

            Fyps = (from fyp in _context.FYPs
                    select new FypInfo
                    {
                        Name = fyp.Name,
                        Details = fyp.Details,
                        Domain = fyp.Domain,
                    }).ToList();

            //Instructors = new List<string> { "Shujan", "Hameed", "Ibrhaim" };
        }

        // Implementing logic for approving supervisor request

        public async Task<IActionResult> OnPostApproveRequest(string supervisorEmail)
        {
            var supervisor = await _context.Supervisors
                .FirstOrDefaultAsync(s => s.Email == supervisorEmail);

            if (supervisor != null)
            {
                supervisor.Status = "Approved";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectRequest(string supervisorEmail)
        {
            var supervisor = await _context.Supervisors
                .FirstOrDefaultAsync(s => s.Email == supervisorEmail);

            if (supervisor != null)
            {
                supervisor.Status = "Reject";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<JsonResult> OnGetSupervisorDetailsAsync(string email)
        {
            var supervisorDetails = await _context.Supervisors
                .Where(s => s.Email == email)
                .Select(s => new
                {
                    s.Email,
                    s.Role,
                    s.Domain,
                    FYPs = s.FYP.Select(f => new { f.Name }).ToList()
                    //canDelete = !s.FYP.Any() // Ensure this correctly reflects whether the supervisor can be deleted
                })
                .FirstOrDefaultAsync();

            return new JsonResult(supervisorDetails);
        }


        //public async Task<IActionResult> OnPostDeleteSupervisorAsync(string email)
        //{
        //    var supervisor = await _context.Supervisors
        //        .Include(s => s.FYP)
        //        .FirstOrDefaultAsync(s => s.Email == email);
        //    if (supervisor != null && !supervisor.FYP.Any())
        //    {
        //        _context.Supervisors.Remove(supervisor);
        //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //        if (user != null)
        //        {
        //            _context.Users.Remove(user);
        //        }

        //        await _context.SaveChangesAsync();

        //    }

        //    return new JsonResult(new { success = true });
        //}
    }
}