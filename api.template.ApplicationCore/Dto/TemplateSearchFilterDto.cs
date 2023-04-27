using System.ComponentModel.DataAnnotations;

namespace Api.Template.ApplicationCore.Dto
{
    public class TemplateSearchFilterDto
    {
        public string KeyWord { get; set; }
        public int? TemplateId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber needs to be greather then 0!")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "RowsPerPage needs to be greather then 0!")]
        public int RowsPerPage { get; set; } = 10;
    }
}