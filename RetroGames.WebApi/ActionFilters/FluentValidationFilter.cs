using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor controller)
        {
            foreach (var parametro in controller.Parameters)
            {
                if (!parametro.BindingInfo?.BindingSource?.CanAcceptDataFrom(BindingSource.Body) ?? true)
                    continue;

                if (!context.ActionArguments.TryGetValue(parametro.Name, out var corpo) || corpo == null)
                {
                    context.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel
                    {
                        ResponseCode = ResponseCodes.MODEL_NULL.Code,
                        ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage
                    });
                    return;
                }
            }
        }

        var erros = new Dictionary<string, List<string>>();

        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento == null)
                continue;

            var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (context.HttpContext.RequestServices.GetService(tipoValidator) is not IValidator validator)
                continue;

            var resultado = await validator.ValidateAsync(new ValidationContext<object>(argumento),
                context.HttpContext.RequestAborted);

            foreach (var erro in resultado.Errors)
            {
                var campo = string.Join(".", erro.PropertyName.Split('.')
                    .Select(JsonNamingPolicy.CamelCase.ConvertName));
                if (!erros.TryGetValue(campo, out var mensagens))
                {
                    mensagens = new List<string>();
                    erros[campo] = mensagens;
                }
                mensagens.Add(erro.ErrorMessage);
            }
        }

        if (erros.Count > 0)
        {
            context.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel<Dictionary<string, List<string>>>
                {
                    ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                    ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                    Data = erros
                });
            return;
        }

        await next();
    }
}
