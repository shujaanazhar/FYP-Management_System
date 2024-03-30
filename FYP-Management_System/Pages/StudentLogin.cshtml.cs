using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NexGen.Models;

namespace FYP_Management_System.Pages
{
    public class StudentLoginModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentLoginModel> _logger;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public StudentLoginModel(AppDbContext context, ILogger<StudentLoginModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogInformation($"Validation errors: {string.Join(", ", errors)}");
                return Page();
            }

            User user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == Email && u.Type == "ST");
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>(); // Instantiate PasswordHasher here
                if (passwordHasher.VerifyHashedPassword(user, user.Password, Password) == PasswordVerificationResult.Success)
                {
                    _logger.LogInformation($"User {user.Email} logged in at {DateTime.UtcNow}");
                    LogSigninInformation(user);
                    return RedirectToPage("/Student_Homepage", new { id = user.Email});
                }
            }

            _logger.LogWarning($"Failed login attempt for user {Email}");
            Message = "Invalid login attempt. Please try again.";
            return Page();
        }

        private void LogSigninInformation(User newUser)
        {
            string logFilePath = Path.Combine("logs", "student_login_log.txt");

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
                    streamWriter.WriteLine($"Student Logged in: {newUser.Email}, Name: {newUser.Name}, Time: {DateTime.UtcNow}");
                }
            }
            catch (Exception ex)
            {
                // Optionally handle the exception, e.g., log to a different output
                _logger.LogError($"Failed to log login information: {ex.Message}");
            }

        }
    }
}
