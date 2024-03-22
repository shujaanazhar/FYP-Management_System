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

            return RedirectToPage("/AcademicOfficerSignin");
        }

        private void LogSignupInformation(User newUser)
        {
            // Logging with ILogger
            _logger.LogInformation($"New academic officer signed up: {newUser.Email}, Name: {newUser.Name}, Type: {newUser.Type}, Time: {DateTime.UtcNow}");
        }

        //public class AcademicOfficerModel : PageModel
        //{

        //    private readonly AppDbContext _context;
        //    private readonly ILogger<AcademicOfficerModel> _logger; // Add this line

        //    public AcademicOfficerModel(AppDbContext context, ILogger<AcademicOfficerModel> logger)
        //    {
        //        _context = context;
        //        _logger = logger; // Assign the logger
        //    }

        //    [BindProperty]
        //    public User NewUser { get; set; }
        //    public void OnGet()
        //    {
        //    }

        //    public async Task<IActionResult> OnPostRegisterAsync()
        //    {
        //        System.IO.File.AppendAllText("log.txt", "OnPostAsync called at " + DateTime.UtcNow + Environment.NewLine); // Quick file logging

        //        Console.WriteLine("OnPostAsync called"); // Console logging

        //        _logger.LogInformation("Attempting to sign up a new user.");

        //        if (!ModelState.IsValid)
        //        {
        //            foreach (var modelStateKey in ModelState.Keys)
        //            {
        //                var modelStateVal = ModelState[modelStateKey];
        //                foreach (var error in modelStateVal.Errors)
        //                {
        //                    var key = modelStateKey;
        //                    var errorMessage = error.ErrorMessage;
        //                    System.IO.File.AppendAllText("log.txt", $"Error in {key}: {errorMessage}\n");
        //                }
        //            }
        //            // Return the page to show validation errors
        //            return Page();
        //        }
        //        NewUser.Type = "AO";
        //        // Hash the password
        //        NewUser.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //            password: NewUser.Password,
        //            salt: System.Text.Encoding.ASCII.GetBytes("YourSaltHere"), // Ensure this is securely managed in real applications
        //            prf: KeyDerivationPrf.HMACSHA256,
        //            iterationCount: 10000,
        //            numBytesRequested: 256 / 8));

        //        try
        //        {
        //            _context.Users.Add(NewUser);
        //            await _context.SaveChangesAsync();
        //            System.IO.File.AppendAllText("log.txt", "User successfully signed up and saved to the database.");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "An error occurred while signing up a new user.");
        //            // Optionally, add ModelState.AddModelError to display an error message on the page
        //            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
        //            return Page();
        //        }

        //        return RedirectToPage("/AcademicOfficerLogin");
        //    }

        //}
    }
}
