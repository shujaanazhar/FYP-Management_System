using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexGen.Models;

namespace FYP_Management_System.Pages
{
    public class AcademicOfficerSignupModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AcademicOfficerSignupModel> _logger;
        private readonly PasswordHasher<User> _passwordHasher;

        public string Message { get; set; }

        [BindProperty]
        public User NewUser { get; set; }

        public AcademicOfficerSignupModel(AppDbContext context, ILogger<AcademicOfficerSignupModel> logger)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            NewUser.Type = "AO"; // Assuming "AO" represents Academic Officer

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogInformation($"Validation errors: {string.Join(", ", errors)}");
                return Page();
            }

            // Check if a user with this email already exists
            var existingUser = await _context.Users.FindAsync(NewUser.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                return Page();
            }

            // Hash the password before saving to the database
            NewUser.Password = _passwordHasher.HashPassword(NewUser, NewUser.Password);

            try
            {
                _context.Users.Add(NewUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving new academic officer: {ex.Message}");
                ModelState.AddModelError("", "An error occurred saving the user. Please try again.");
                return Page();
            }

            // Log the signup information
            LogSignupInformation(NewUser);

            return RedirectToPage("/AcademicOfficerLogin");
        }

        private void LogSignupInformation(User newUser)
        {
            string logFilePath = Path.Combine("logs", "academicofficer_signup_log.txt");

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
                    streamWriter.WriteLine($"New academic officer signed up: {newUser.Email}, Name: {newUser.Name}, Type: {newUser.Type}, Time: {DateTime.UtcNow}");
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
