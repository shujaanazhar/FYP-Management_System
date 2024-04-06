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
            public string Domain { get; set; }
            public List<string> StudentNames { get; set; } = new List<string>();
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

            Fyps = (from fyp in _context.FYPs
                    where fyp.SupervisorId == Supervisor.Email
                    select new FYPInfo
                    {
                        Name = fyp.Name,
                        Description = fyp.Details,
                        Domain = fyp.Domain,
                        Status = fyp.Status,
                        StudentNames = (from student in fyp.Students
                                        join user in _context.Users on student.Email equals user.Email
                                        select user.Name).ToList() // Getting student names
                    }).ToList();
        }

        public async Task<IActionResult> OnPostApproveRequest(string fypName)
        {
            var fyp = await _context.FYPs
                .FirstOrDefaultAsync(f => f.Name== fypName);

            if (fyp != null)
            {
                fyp.Status = "Approved";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = fyp.SupervisorId });
        }

        public async Task<IActionResult> OnPostRejectRequest(string fypName)
        {
            var fyp = await _context.FYPs
                .FirstOrDefaultAsync(f => f.Name == fypName);

            if (fyp != null)
            {
                fyp.Status = "Reject";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = fyp.SupervisorId});
        }

        public async Task<IActionResult> OnGetDownloadDocumentAsync(string fypName)
        {
            var fyp = await _context.FYPs.FirstOrDefaultAsync(f => f.Name == fypName);
            if (fyp == null || string.IsNullOrWhiteSpace(fyp.DocumentPath))
            {
                return NotFound("Document not found.");
            }

            var filePath = fyp.DocumentPath;
            var fileName = Path.GetFileName(filePath); // Assuming you want to use the actual file name

            // Ensure the MIME type is correct; for example, "application/pdf" for PDF files. 
            // You might need to determine this dynamically based on the file type.
            var mimeType = "APPLICATION/octet-stream"; // Generic MIME type for binary data
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, mimeType, fileName);
        }

    }
}
