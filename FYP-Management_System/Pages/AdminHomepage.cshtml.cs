using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        }
        public List<string> Fyps { get; set; }
        public List<string> Instructors { get; set; }

        public List<SupervisorInfo> ApprovedSupervisors { get; set; }
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

            Fyps = new List<string> { "Cross-Site Scripting (XSS) Prevention", "SQL Injection Prevention", "Firewall Rules Analyzer" };

            Instructors = new List<string> { "Shujan", "Hameed", "Ibrhaim" };
        }

        // Implement logic for approving supervisor request
        public IActionResult OnPostApproveRequest(int supervisorId)
        {
            // Your logic here
            return RedirectToPage("/AdminHomepage");
        }

        // Implement logic for rejecting supervisor request
        public IActionResult OnPostRejectRequest(int supervisorId)
        {
            // Your logic here
            return RedirectToPage("AdminHomepage");
        }

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

    }
}