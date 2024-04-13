using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NexGen.Models;

namespace FYP_Management_System.Pages
{
    public class FypIterationsModel : PageModel
    {
        private readonly AppDbContext _context;

        public FypIterationsModel(AppDbContext context)
        {
            _context = context;
        }

        public class Iterations
        {
            public string Name { get; set; }
            public string Details { get; set; }
            public List<string> Tasks { get; set; }
            public DateOnly DueDate { get; set; }
            public string Status { get; set; }
            public string Task1 { get; set; }
            public string Task2 { get; set; }
            public string Task3 { get; set; }
        }
        [BindProperty]
        public string fypName { get; set; }
        [BindProperty]
        public string Id { get; set; }
        [BindProperty]
        public string Role { get; set; }
        public List<Iterations> iterations { get; set; }
        public async Task OnGetAsync(string Name, string id, string role)
        {
            fypName = Name; // Or use another method to set this
            Id = id;
            Role = role;
            iterations = await (from iter in _context.Iterations
                                where iter.FYPName == fypName
                                orderby iter.DueDate // It's good to order results
                                select new Iterations
                                {
                                    Name = iter.Name,
                                    Details = iter.Details,
                                    DueDate = iter.DueDate,
                                    Status = iter.Status,
                                    Task1 = iter.Task1,
                                    Task2 = iter.Task2,
                                    Task3 = iter.Task3
                                }).ToListAsync();
        }

        public async Task<IActionResult> OnPost(string Name, string details, DateOnly dueDate, string Task1, string Task2, string Task3)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Page(); // Stay on the same page if there are errors
            }

            Iteration iteration = new Iteration
            {
                Name = Name,
                Details = details,
                DueDate = dueDate,
                Task1 = Task1,
                Task2 = Task2,
                Task3 = Task3,
                FYPName = fypName
            };
            _context.Iterations.Add(iteration);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { Name = fypName, id = Id, role = Role }); // Redirect to the same page to refresh the state
        }

        public async Task<IActionResult> OnPostCompleteIteration(string Name, string fypName)
        {
            this.fypName = fypName;  // Make sure fypName is set
            var iteration = await _context.Iterations.FirstOrDefaultAsync(s => s.Name == Name);
            if (iteration != null)
            {
                iteration.Status = "Complete";
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { Name = fypName, id = Id, role = Role });
        }


    }
}
