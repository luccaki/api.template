using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Template.ApplicationCore.Dto;

namespace Api.Template.ApplicationCore.Interfaces.Repositories
{
    public interface ITemplateRepository
	{
        Task<Entities.Template> GetTemplateByIdAsync(int TemplateId);
        Task<IEnumerable<Entities.Template>> GetTemplateFullDetailsByFilterAsync(TemplateSearchFilterDto filter);
        Task<int> InsertTemplateAsync(Entities.Template Template);
        Task UpdateTemplateAsync(Entities.Template Template);
    }
}