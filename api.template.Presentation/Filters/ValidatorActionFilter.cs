using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Template.ApplicationCore.Exceptions;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api.Template.Presentation.Filters
{
    public class ValidatorActionFilter : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.ModelState.IsValid)
			{
				var error = context.ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage);

				var response = new ExceptionResponse()
				{
					StatusCode = HttpStatusCode.BadRequest.GetHashCode(),
					Message = error?.FirstOrDefault()
				};

				context.Result = new BadRequestObjectResult(response);
			}
			else
			{
				if (next != null)
				{
					var resultContext = await next();
				}
			}
		}

		public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			var result = context.Result;

			if (result is OkObjectResult okObj)
			{
				var jsonData = JsonConvert.SerializeObject(okObj.Value ?? new object(),
					new JsonSerializerSettings
					{
						ContractResolver = new CamelCasePropertyNamesContractResolver()
					});

				context.Result = new ContentResult
				{
					Content = jsonData,
					ContentType = "application/json; charset=utf-8"
				};
			}

			await base.OnResultExecutionAsync(context, next);
		}
	}
}