using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CVGenerationSolution.Models;

namespace CKEditorIntegration.Controllers
{
    public class EditorController : Controller
    {
        private readonly string _templatePath = "wwwroot/template.html";
        private readonly string _dataPath = "wwwroot/data.json";

        // Index() - läser data.json och ersätter platshållare i template.html
        public async Task<IActionResult> Index()
        {
            // Kolla att både template.html och data.json finns
            if (!System.IO.File.Exists(_templatePath) || !System.IO.File.Exists(_dataPath))
            {
                return Content("Missing template.html or data.json in wwwroot folder.");
            }

            // Läs JSON-filen
            string jsonData = await System.IO.File.ReadAllTextAsync(_dataPath);
            var resumeData = JsonConvert.DeserializeObject<ResumeData>(jsonData);

            if (resumeData == null)
            {
                return Content("Error: Could not parse data.json.");
            }

            // Läs in mallen (template.html) och ersätt platshållare
            string htmlTemplate = await System.IO.File.ReadAllTextAsync(_templatePath);
            string populatedHtml = ReplacePlaceholders(htmlTemplate, resumeData);

            // Skicka det färdiga HTML-innehållet till vyn via ViewData
            ViewData["HtmlContent"] = populatedHtml;
            return View(); // -> Laddar Views/Editor/Index.cshtml
        }

        // Metod som ersätter {{name}}, {{address}} osv. i template.html
        private string ReplacePlaceholders(string template, ResumeData data)
        {
            template = template.Replace("{{name}}", data.Name ?? "")
                               .Replace("{{address}}", data.Address ?? "")
                               .Replace("{{phone}}", data.Phone ?? "")
                               .Replace("{{email}}", data.Email ?? "")
                               .Replace("{{about}}", data.About ?? "");

            // Ersätt Skills
            string skillsHtml = data.Skills != null && data.Skills.Count > 0
                ? string.Join("", data.Skills.Select(skill => $"<li>{skill}</li>"))
                : "<li>No skills listed</li>";
            template = template.Replace("{{skills}}", skillsHtml);

            // Ersätt Projects
            if (data.Projects != null && data.Projects.Count > 0)
            {
                string projectsHtml = string.Join("", data.Projects.Select(proj =>
                    $"<div class='project'><b>{proj.Title}</b> ({proj.Company}, {proj.Dates})<p>{proj.Description}</p></div>"
                ));
                template = template.Replace("{{projects}}", projectsHtml);
            }
            else
            {
                template = template.Replace("{{projects}}", "<div>No projects listed</div>");
            }

            return template;
        }

        // Save() - tar emot JSON-data från klienten och sparar till data.json
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ResumeData resumeData)
        {
            if (resumeData != null)
            {
                // Konvertera ResumeData till JSON
                string jsonData = JsonConvert.SerializeObject(resumeData, Formatting.Indented);
                // Spara i data.json
                await System.IO.File.WriteAllTextAsync(_dataPath, jsonData);
            }
            return Ok();
        }
    }
}
