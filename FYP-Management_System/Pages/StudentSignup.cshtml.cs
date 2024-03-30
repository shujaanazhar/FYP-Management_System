using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NexGen.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP_Management_System.Pages
{
    public class StudentSignupModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentSignupModel> _logger;
        private readonly PasswordHasher<User> _passwordHasher;

        [BindProperty]
        public Input NewStudent { get; set; }
        public string Message { get; set; }
    
       

        public class Input
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            public string Department { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public int Batch { get; set; }
            [Required]
            public float CGPA { get; set; }
            public string? Type { get; set; }
        }

        public StudentSignupModel(AppDbContext context, ILogger<StudentSignupModel> logger)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            NewStudent.Type = "ST"; // Assuming "ST" represents Student

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogInformation($"Validation errors: {string.Join(", ", errors)}");
                return Page();
            }

            try
            {
                // Check if a user with this email already exists
                var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == NewStudent.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "A user with this email already exists.");
                    return Page();
                }

                // Create a new User instance
                var newUser = new User
                {
                    Email = NewStudent.Email,
                    Password = _passwordHasher.HashPassword(null, NewStudent.Password), // Hash the password
                    Name = NewStudent.Name,
                    Type = NewStudent.Type
                };

                // Create a new Student instance and associate it with the user
                var newStudent = new Student
                {
                    Email = NewStudent.Email, // This assumes the email is the key for both User and Supervisor
                    Batch = NewStudent.Batch,
                    Department = NewStudent.Department,
                    CGPA = NewStudent.CGPA,
                    User = newUser // Associate the User entity with the Supervisor entity
                };

                // Add both the User and Student to the DbContext
                _context.Users.Add(newUser);
                _context.Students.Add(newStudent);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Log the signup information
                LogSignupInformation(newUser);

                return RedirectToPage("/StudentLogin");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving new Student: {ex.Message}");

                // Log inner exception for detailed error information
                _logger.LogError($"Inner Exception: {ex.InnerException?.Message}");

                ModelState.AddModelError("", "An error occurred saving the user. Please try again.");
                return Page();
            }
        }


        private void LogSignupInformation(User newUser)
        {
            string logFilePath = Path.Combine("logs", "student_signup_log.txt");

            try
            {
                // Ensure the directory exists
                var logDirectory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Log the information to the file
                using (var streamWriter = new StreamWriter(logFilePath, append: true))
                {
                    streamWriter.WriteLine($"New student signed up: {newUser.Email}, Name: {newUser.Name}, Type: {newUser.Type}, Time: {DateTime.UtcNow}");
                }
            }
            catch (Exception ex)
            {
                // Optionally handle the exception, e.g., log to a different output
                _logger.LogError($"Failed to log signup information: {ex.Message}");
            }
        }
    }
}
    