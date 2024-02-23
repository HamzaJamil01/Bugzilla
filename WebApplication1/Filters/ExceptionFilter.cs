using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using WebApplication1.Models;

public class ExceptionFilterClass : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var errorViewModel = new ErrorViewModel
        {
            ExceptionMessage = context.Exception.Message
        };
        context.Result = new ViewResult
        {
            ViewName = "Error",
            StatusCode = 500,
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = errorViewModel
            }
        };

        context.ExceptionHandled = true;
    }
}