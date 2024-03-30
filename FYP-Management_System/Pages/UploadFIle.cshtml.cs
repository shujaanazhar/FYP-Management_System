using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
namespace FYP_Management_System.Pages
{
    public class UploadFIleModel : PageModel
    {

        private IWebHostEnvironment _environment;

        public string Message { get; private set; }
        public List<string> Files = new List<string>();
        [Obsolete]
        public UploadFIleModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public void OnGet()
        {

        }

        public void OnPostUpload(List<IFormFile> postedFiles)
        {
            string uploadsDirectory = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(uploadsDirectory))
            {
                Directory.CreateDirectory(uploadsDirectory);
            }

            foreach (var postedFile in postedFiles)
            {
                if (postedFile.Length > 0)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(uploadsDirectory, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }
                    Files.Add(fileName); // Add file name to the list of uploaded files
                    Message = $"{Files.Count} file(s) uploaded successfully.";
                }
            }


        }
    }
}
