using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexGen.Models;
using System;
using System.Threading.Tasks;

namespace FYP_Management_System.Pages
{
    public class SupervisorSignupModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AcademicOfficerSignupModel> _logger;
        private readonly PasswordHasher<User> _passwordHasher;

        public string Message { get; set; }

        [BindProperty]
        public User NewUser { get; set; }
        public class SupervisorModel : PageModel
        {

            private readonly AppDbContext _context;
            private readonly ILogger<SupervisorModel> _logger; // Add this line

            public SupervisorModel(AppDbContext context, ILogger<SupervisorModel> logger)
            {
                _context = context;
                _logger = logger; // Assign the logger
            }

            [BindProperty]
            public User NewUser { get; set; }
            public void OnGet()
            {
            }

            public async Task<IActionResult> OnPostRegisterAsync()
            {
                System.IO.File.AppendAllText("log.txt", "OnPostAsync called at " + DateTime.UtcNow + Environment.NewLine); // Quick file logging

                Console.WriteLine("OnPostAsync called"); // Console logging

                _logger.LogInformation("Attempting to sign up a new user.");

                if (!ModelState.IsValid)
                {
                    foreach (var modelStateKey in ModelState.Keys)
                    {
                        var modelStateVal = ModelState[modelStateKey];
                        foreach (var error in modelStateVal.Errors)
                        {
                            var key = modelStateKey;
                            var errorMessage = error.ErrorMessage;
                            System.IO.File.AppendAllText("log.txt", $"Error in {key}: {errorMessage}\n");
                        }
                    }
                    // Return the page to show validation errors
                    return Page();
                }
                NewUser.Type = "SV";
                // Hash the password
                NewUser.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: NewUser.Password,
                    salt: System.Text.Encoding.ASCII.GetBytes("YourSaltHere"), // Ensure this is securely managed in real applications
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                try
                {
                    _context.Users.Add(NewUser);
                    await _context.SaveChangesAsync();
                    System.IO.File.AppendAllText("log.txt", "User successfully signed up and saved to the database.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while signing up a new user.");
                    // Optionally, add ModelState.AddModelError to display an error message on the page
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                    return Page();
                }

                return RedirectToPage("/SupervisorLogin");
            }

        }
    }
}
