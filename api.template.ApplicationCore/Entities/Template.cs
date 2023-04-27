using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Template.ApplicationCore.Entities
{
    public class Template
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string SomeOtherInfo { get; set; }
    }
}