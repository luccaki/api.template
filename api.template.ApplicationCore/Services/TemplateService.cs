using Api.Template.ApplicationCore.Dto;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.Template.ApplicationCore.Interfaces.Services;
using Api.Template.ApplicationCore.Interfaces.Repositories;
using Api.Template.ApplicationCore.Exceptions;

namespace Api.Template.ApplicationCore.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;

        public TemplateService(ITemplateRepository TemplateRepository)
        {
            _templateRepository = TemplateRepository;
        }

        public async Task<TemplateResultDto> GetTemplateAsync(int? pageNumber, string name)
        {
            if (pageNumber.HasValue && pageNumber.Value <= 0)
                throw new BusinessException("Paginação a partir da página 1", HttpStatusCode.BadRequest);

            if (!pageNumber.HasValue)
                pageNumber = 0;
            else
                pageNumber--;

            var result = await _templateRepository.GetTemplateFullDetailsByFilterAsync(new TemplateSearchFilterDto()
            {
                PageNumber = pageNumber.Value,
                KeyWord = name,
            });

            return new TemplateResultDto()
            {
                Count = result?.Count() ?? 0,
                Data = result?.Select(x => new TemplateDto()
                {
                    TemplateId = x.TemplateId,
                    Name = x.Name,
                    Content = x.Content,
                    SomeOtherInfo = x.SomeOtherInfo
                })
            };
        }

        public async Task<TemplateDto> GetTemplateByIdAsync(int TemplateId)
        {
            var result = await _templateRepository.GetTemplateByIdAsync(TemplateId);
            if (result == null)
                return null;

            return new TemplateDto()
            {
                TemplateId = result.TemplateId,
                Name = result.Name,
                Content = result.Content,
                SomeOtherInfo = result.SomeOtherInfo
            };
        }

        public async Task<int> InsertTemplateAsync(TemplatePostDto body)
        {
            if (body.Content.Length > 255)
                throw new BusinessException("Template Content need to be 255 or less character!", HttpStatusCode.BadRequest);

            var template = await _templateRepository.GetTemplateFullDetailsByFilterAsync(new TemplateSearchFilterDto()
            {
                KeyWord = body.Name
            });

            if (template != null && template.Any(x => x.Name.Equals(body.Name)))
                throw new BusinessException("We already have a template with the same name!", HttpStatusCode.BadRequest);

            var templateId = await _templateRepository.InsertTemplateAsync(new Entities.Template()
            {
                Name = body.Name,
                Content = body.Content,
                SomeOtherInfo = body.SomeOtherInfo
            });

            return templateId;
        }

        public async Task UpdateTemplateAsync(int id, TemplateDto template)
        {
            var verifyTemplate = await GetTemplateByIdAsync(id);
            if (verifyTemplate is null)
                throw new BusinessException("Template not found!", HttpStatusCode.NotFound);

            await _templateRepository.UpdateTemplateAsync(new Entities.Template()
            {
                TemplateId = id,
                Name = template.Name,
                Content = template.Content,
                SomeOtherInfo = template.SomeOtherInfo
            });
        }
    }
}