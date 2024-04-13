//using FYP_Management_System.DAL;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using NexGen.Models;

//namespace FYP_Management_System.Pages
//{
//    public class tempModel : PageModel
//    {
//        private readonly AppDbContext _context;

//        public tempModel(AppDbContext context)
//        {
//            _context = context;
//        }
//        public void OnGet()
//        {
//        }
//        public IActionResult OnPostCreateIterationAsync(string name, int number, string details, List<string> Tasks)
//        {
//            if (!ModelState.IsValid)
//            {
//                return new JsonResult(new { success = false, message = "Invalid data" });
//            }

//            // Simulate adding to the database by printing the iteration details

//            return new JsonResult(new { success = true, message = "Iteration received successfully" });
//        }

//        public IActionResult OnPostCreateToDoItemAsync([FromBody] ToDoItem toDoItem)
//        {
//            if (!ModelState.IsValid)
//            {
//                return new JsonResult(new { success = false, message = "Invalid data" });
//            }

//            // Simulate adding to the database by printing the ToDoItem details

//            return new JsonResult(new { success = true, message = "ToDoItem received successfully"});
//        }

//    }
//}
