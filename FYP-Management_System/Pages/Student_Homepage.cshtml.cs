using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NexGen.Models;
using System.Net.Http.Headers;

namespace FYP_Management_System.Pages
{
    public class StudentHomePageModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentHomePageModel> _logger;
        private IWebHostEnvironment _environment;

        public StudentHomePageModel(AppDbContext context, ILogger<StudentHomePageModel> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        public class StudentInfo
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public int Batch { get; set; }
            public string Department { get; set; }
            public float CGPA { get; set; }
            public string FYP_Name { get; set; }
        }

        public class FYP_Info
        {
            public string FYP_Name { get; set; }
            public string Domain { get; set; }
            public string Details { get; set; }
            public string Supervisor { get; set; }
            public string Status { get; set; }
            public List<string> GroupMemberNames { get; set; } = new List<string>();
        }
        public class SupervisorInfo
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Domain { get; set; }
            public string Role { get; set; }
        }

        public StudentInfo Student { get; set; }
        public FYP_Info FYP { get; set; }
        public List<SupervisorInfo> Supervisor { get; set; }
        public string Email { get; set; }
        public string FilePath { get; set; }
        public async Task OnGetAsync(string id)
        {
            Student = await (from user in _context.Users
                             join student in _context.Students
                             on user.Email equals student.Email
                             where user.Email == id
                             select new StudentInfo
                             {
                                 Email = user.Email,
                                 Name = user.Name,
                                 Batch = student.Batch,
                                 Department = student.Department,
                                 CGPA = student.CGPA,
                                 FYP_Name = student.FYP_Name
                             }).FirstOrDefaultAsync();

            Supervisor = await (from supervisor in _context.Supervisors
                                join user in _context.Users
                                on supervisor.Email equals user.Email
                                where supervisor.Status == "Approved"
                                select new SupervisorInfo
                                {
                                    Email = supervisor.Email,
                                    Name = user.Name,
                                    Domain = supervisor.Domain,
                                    Role = supervisor.Role
                                }).ToListAsync();

            FYP = await (from student in _context.Students
                         join fyp in _context.FYPs on student.FYP_Name equals fyp.Name
                         join supervisor in _context.Supervisors on fyp.SupervisorId equals supervisor.Email
                         join user in _context.Users on supervisor.Email equals user.Email
                         where student.Email == id
                         select new FYP_Info
                         {
                             FYP_Name = fyp.Name,
                             Domain = fyp.Domain,
                             Details = fyp.Details,
                             Supervisor = user.Name, // Assuming user.Name is the supervisor's name
                             Status = fyp.Status
                         }).FirstOrDefaultAsync();
            var fypName = await _context.Students
                .Where(s => s.Email == id)
                .Select(s => s.FYP_Name)
                .FirstOrDefaultAsync();

            if (fypName != null)
            {
                // Assuming you've successfully fetched FYP_Info above and just need to populate emails now
                FYP.GroupMemberNames = await (from student in _context.Students
                                              join user in _context.Users on student.Email equals user.Email
                                              where student.FYP_Name == fypName
                                              select user.Name).ToListAsync();
            }
        }

        // Adjust the parameters of the OnPost method to include IFormFile for the document
        public async Task<IActionResult> OnPostAsync(string Email, string fypName, string domain, string details, string supervisorEmail, List<string> memberEmails, IFormFile postedFiles)
        {
            // Document upload handling
            string filePath = null;
            if (postedFiles != null && postedFiles.Length > 0)
            {
                var uploadsDirectory = Path.Combine(_environment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                // Ensure the file name is unique to avoid overwriting existing files
                var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(postedFiles.ContentDisposition).FileName.Trim('"'));
                filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postedFiles.CopyToAsync(stream);
                }
            }

            // Create the FYP entity with document path
            var fyp = new FYP
            {
                Name = fypName,
                SupervisorId = supervisorEmail,
                Details = details,
                Domain = domain,
                DocumentPath = filePath, // Save the document path
                Status = "Pending"
            };

            // Add the primary student and group members to the FYP
            memberEmails.Add(Email); // Ensure the signed-up student's email is included
            fyp.Students = [];
            foreach (var email in memberEmails.Distinct())
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
                if (student != null)
                {
                    student.FYP_Name = fypName; // Associate student with FYP by name
                    student.FYP = fyp;
                    fyp.Students.Add(student); // Add student to the FYP
                }
                else
                {
                    _logger.LogWarning($"Student not found: {email}");
                    // Optionally handle cases where a student email is not found
                }
            }

            // Save the FYP and its associated students
            await _context.FYPs.AddAsync(fyp);
            await _context.SaveChangesAsync();

            // Redirect to a confirmation page or refresh the current page
            return RedirectToPage("Student_Homepage", new { id = Email });
        }



    }
}

