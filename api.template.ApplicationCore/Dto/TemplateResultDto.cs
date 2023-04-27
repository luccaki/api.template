using System.Collections.Generic;

namespace Api.Template.ApplicationCore.Dto
{
	public class TemplateResultDto
	{
		public int? Count { get; set; }
		public IEnumerable<TemplateDto> Data { get; set; }
	}
}