using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NexGen.Models;
using System.ComponentModel.DataAnnotations;

namespace FYP_Management_System.Pages
{
    public class SupervisorSignupModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SupervisorSignupModel> _logger;
        private readonly PasswordHasher<User> _passwordHasher;

        [BindProperty]
        public Input NewSupervisor { get; set; }
        public string Message { get; set; }

        public class Input
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }

            [Required]
            public string Name { get; set; }
            [Required]
            public string Role { get; set; }

            [Required]
            public string Domain { get; set; }

            public string? Type { get; set; }
        }

        public SupervisorSignupModel(AppDbContext context, ILogger<SupervisorSignupModel> logger)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            NewSupervisor.Type = "SP"; // Assuming "SP" represents Supervisor

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogInformation($"Validation errors: {string.Join(", ", errors)}");
                return Page();
            }

            // Check if a user with this email already exists
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == NewSupervisor.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                return Page();
            }

            // Create a new User instance
            var newUser = new User
            {
                Email = NewSupervisor.Email,
                Password = _passwordHasher.HashPassword(null, NewSupervisor.Password), // Hash the password
                Name = NewSupervisor.Name,
                Type = NewSupervisor.Type
            };

            // Create a new Supervisor instance and associate it with the user
            var newSupervisor = new Supervisor
            {
                Email = NewSupervisor.Email, // This assumes the email is the key for both User and Supervisor
                Role = NewSupervisor.Role,
                Domain = NewSupervisor.Domain,
                User = newUser // Associate the User entity with the Supervisor entity
            };

            try
            {
                // Add both the User and Supervisor to the DbContext
                _context.Users.Add(newUser);
                _context.Supervisors.Add(newSupervisor);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Log the signup information
                LogSignupInformation(newUser);

                return RedirectToPage("/SupervisorLogin");
            }
            catch (Exception ex)

            {
                _logger.LogError($"Error saving new supervisor: {ex.Message}");
                ModelState.AddModelError("", "An error occurred saving the user. Please try again.");
                return Page();
            }
        }

        private void LogSignupInformation(User newUser)
        {
            string logFilePath = Path.Combine("logs", "supervisor_signup_log.txt");

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
                    streamWriter.WriteLine($"New supervisor signed up: {newUser.Email}, Name: {newUser.Name}, Type: {newUser.Type}, Time: {DateTime.UtcNow}");
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
