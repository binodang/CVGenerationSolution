using System.Collections.Generic;

namespace CVGenerationSolution.Models
{
    public class ResumeData
    {
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string About { get; set; } = "";
        public List<string> Skills { get; set; } = new List<string>();
        public List<Project> Projects { get; set; } = new List<Project>();
    }

    public class Project
    {
        public string Title { get; set; } = "";
        public string Company { get; set; } = "";
        public string Dates { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
