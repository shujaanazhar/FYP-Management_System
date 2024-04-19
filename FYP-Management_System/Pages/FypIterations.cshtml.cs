using FYP_Management_System.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NexGen.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace FYP_Management_System.Pages
{
    public class FypIterationsModel : PageModel
    {
        private readonly AppDbContext _context;

        public FypIterationsModel(AppDbContext context)
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
        public FYPInfo Fyp { get; set; }
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

        public async Task<IActionResult> OnPostGenerateReportAsync(string fypName)
        {
            var fypDetails = await _context.FYPs
                .Include(f => f.Iterations)
                .Include(f => f.Students)
                .FirstOrDefaultAsync(f => f.Name == fypName);

            if (fypDetails == null)
            {
                return NotFound($"FYP with name {fypName} not found.");
            }

            var studentEmails = fypDetails.Students.Select(s => s.Email).ToList();
            var studentNames = await _context.Users
                .Where(u => studentEmails.Contains(u.Email))
                .ToDictionaryAsync(u => u.Email, u => u.Name);

            // Create MemoryStream
            var memoryStream = new MemoryStream();

            // Initialize the PDF writer and document
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            try
            {
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                Paragraph spacer = new Paragraph(new Text("\n"));

                // Main Title
                Paragraph title = new Paragraph(new Text("FYP Details").SetFont(boldFont).SetFontSize(18))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20); // Adds space after the title
                document.Add(title);

                // FYP Name
                document.Add(new Paragraph(new Text("FYP Name").SetFont(boldFont).SetFontSize(14))
                    .SetTextAlignment(TextAlignment.LEFT));
                document.Add(new Paragraph(fypDetails.Name).SetFont(font).SetFontSize(12));

                // Domain
                document.Add(new Paragraph(new Text("Domain").SetFont(boldFont).SetFontSize(14))
                    .SetTextAlignment(TextAlignment.LEFT));
                document.Add(new Paragraph(fypDetails.Domain).SetFont(font).SetFontSize(12));

                // Description
                document.Add(new Paragraph(new Text("Description").SetFont(boldFont).SetFontSize(14))
                    .SetTextAlignment(TextAlignment.LEFT));
                document.Add(new Paragraph(fypDetails.Details).SetFont(font).SetFontSize(12));

                // Status
                document.Add(new Paragraph(new Text("Status").SetFont(boldFont).SetFontSize(14))
                    .SetTextAlignment(TextAlignment.LEFT));
                document.Add(new Paragraph(fypDetails.Status ?? "N/A").SetFont(font).SetFontSize(12));

                // Students
                document.Add(new Paragraph(new Text("Students").SetFont(boldFont).SetFontSize(14))
    .SetTextAlignment(TextAlignment.LEFT));
                List studentList = new List().SetSymbolIndent(12).SetListSymbol("\u2022");
                foreach (var email in studentEmails)
                {
                    var name = studentNames.ContainsKey(email) ? studentNames[email] : "Unknown";
                    studentList.Add((ListItem)new ListItem(name).SetFont(font).SetFontSize(12)); // Using strings directly
                }
                document.Add(studentList);


                // Iterations
                document.Add(new Paragraph(new Text("Iterations").SetFont(boldFont).SetFontSize(14))
                    .SetTextAlignment(TextAlignment.LEFT));
                foreach (var iteration in fypDetails.Iterations)
                {
                    document.Add(spacer);

                    document.Add(new Paragraph(iteration.Name).SetFont(font).SetFontSize(12).SetBold());
                    document.Add(new Paragraph("Details: " + iteration.Details).SetFont(font).SetFontSize(12));
                    document.Add(new Paragraph("Status: " + iteration.Status).SetFont(font).SetFontSize(12));
                    document.Add(new Paragraph("Due Date: " + iteration.DueDate.ToString()).SetFont(font).SetFontSize(12));

                    document.Add(spacer);

                    document.Add(new Paragraph("Tasks:").SetFont(font).SetFontSize(12).SetBold());
                    document.Add(new Paragraph("Task 1: " + iteration.Task1).SetFont(font).SetFontSize(12));
                    document.Add(new Paragraph("Task 2: " + iteration.Task2).SetFont(font).SetFontSize(12));
                    document.Add(new Paragraph("Task 3: " + iteration.Task3).SetFont(font).SetFontSize(12));
                }

                document.Close();
            }

            catch (Exception ex)
            {
                // Ensure resources are cleaned up properly to avoid memory leaks
                document.Close();
                pdf.Close();
                writer.Close();
                throw new InvalidOperationException("Failed to generate PDF", ex);
            }

            // Reset the memory stream position to the beginning
            //memoryStream.Position = 0;

            // Return the PDF as a FileResult
            return File(memoryStream.ToArray(), "application/pdf", fypName + "_FYP_Report.pdf");
        }





    }
}
