using System.ComponentModel.DataAnnotations;

namespace Api.Template.ApplicationCore.Dto
{
    public class TemplatePostDto
    {
        [Required(ErrorMessage = "Necessary a template name! (Name)")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Necessary a template content! (Content)")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Necessary this other information! (SomeOtherInfo)")]
        public string SomeOtherInfo { get; set; }
    }
}