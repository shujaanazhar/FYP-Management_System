using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NexGen.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace FYP_Management_System.Pages
{
    public class SupervisorLoginModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SupervisorLoginModel> _logger;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public SupervisorLoginModel(AppDbContext context, ILogger<SupervisorLoginModel> logger)
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

            User user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == Email && u.Type == "SP");
            if (user != null)
            {
                Supervisor supervisor = await _context.Supervisors.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Email == user.Email && s.Status == "Approved");
                if (supervisor != null)
                {
                    var passwordHasher = new PasswordHasher<User>(); // Instantiate PasswordHasher here
                    if (passwordHasher.VerifyHashedPassword(user, user.Password, Password) == PasswordVerificationResult.Success)
                    {
                        _logger.LogInformation($"User {user.Email} logged in at {DateTime.UtcNow}");
                        LogSigninInformation(user);
                        return RedirectToPage("/SupervisorHomepage", new {id = user.Email});
                    }
                }
            }

            _logger.LogWarning($"Failed login attempt for user {Email}");
            Message = "Either invalid login attempt or not yet approved. Please contact Academic Officer.";
            return Page();
        }

        private void LogSigninInformation(User newUser)
        {
            string logFilePath = Path.Combine("logs", "supervisor_login_log.txt");

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
                    streamWriter.WriteLine($"Supervisor Logged in: {newUser.Email}, Name: {newUser.Name}, Time: {DateTime.UtcNow}");
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
