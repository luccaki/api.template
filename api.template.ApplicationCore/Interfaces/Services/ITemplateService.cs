using Api.Template.ApplicationCore.Dto;

namespace Api.Template.ApplicationCore.Interfaces.Services
{
    public interface ITemplateService
	{
		Task<TemplateResultDto?> GetTemplateAsync(int? pageNumber, string? name);
        Task<TemplateDto?> GetTemplateByIdAsync(int TemplateId);
        Task<int> InsertTemplateAsync(TemplatePostDto body);
        Task UpdateTemplateAsync(int id, TemplateDto Template);
    }
}