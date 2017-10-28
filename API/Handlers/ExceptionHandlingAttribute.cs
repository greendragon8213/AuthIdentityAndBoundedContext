using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Core;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using API.Models;
using AutoMapper;
using BLL.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Handlers
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var responseBody = new ExceptionResponse<string>()
            {
                Description = "An error occured",
                ExceptionData = context.Exception.Message
            };

            if (context.Exception is ObjectNotFoundException || context.Exception is NullReferenceException)
            {
                responseBody.Description = "Object not found";
                context.Response = GetExceptionResponse(HttpStatusCode.NotFound, responseBody);
                return;
            }
            if (context.Exception is ValidationFailedException || context.Exception is ValidationException || context.Exception is ArgumentException || context.Exception is FormatException)
            {
                responseBody.Description = context.Exception.Message;
                context.Response = GetExceptionResponse((HttpStatusCode)422, responseBody);
                return;
            }
            if (context.Exception is UserDoesntHavePermissionException)
            {
                responseBody.Description = "No permission to complete the action";
                context.Response = GetExceptionResponse(HttpStatusCode.Forbidden, responseBody);
                return;
            }
            if (context.Exception is DuplicateNameException)
            {
                responseBody.Description = "Object already exists";
                context.Response = GetExceptionResponse(HttpStatusCode.Conflict, responseBody);
                return;
            }
            //if (context.Exception is InactiveException)
            //{
            //    responseBody.Status = ExceptionMessages.NotActive;
            //    context.Response = GetExceptionResponse(HttpStatusCode.NotFound, responseBody);
            //    return;
            //}
            //if (context.Exception is WrongFileExtensionException)
            //{
            //    responseBody.Status = ExceptionMessages.WrongMediaType;
            //    context.Response = GetExceptionResponse(HttpStatusCode.UnsupportedMediaType, responseBody);
            //    return;
            //}
            if (context.Exception.InnerException is ValidationException || context.Exception.InnerException is ValidationFailedException)
            {
                responseBody.Description = context.Exception.InnerException.Message;
                responseBody.ExceptionData = context.Exception.InnerException.Message;
                context.Response = GetExceptionResponse((HttpStatusCode)422, responseBody);
                return;
            }
            if (context.Exception is AutoMapperMappingException && context.Exception.InnerException?.InnerException is ValidationFailedException)
            {
                responseBody.Description = context.Exception.InnerException.InnerException.Message;
                responseBody.ExceptionData = context.Exception.InnerException.InnerException.Message;
                context.Response = GetExceptionResponse((HttpStatusCode)422, responseBody);
                return;
            }
            if ((context.Exception is AutoMapperMappingException && context.Exception.InnerException?.InnerException is NullReferenceException))
            {
                responseBody.Description = "Object not found";
                responseBody.ExceptionData = context.Exception.InnerException.InnerException.Message;
                context.Response = GetExceptionResponse(HttpStatusCode.NotFound, responseBody);
                return;
            }

            context.Response = GetExceptionResponse(HttpStatusCode.InternalServerError, responseBody);
        }

        private HttpResponseMessage GetExceptionResponse<T>(HttpStatusCode statusCode, T responseBody)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(responseBody, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }), Encoding.UTF8, "application/json")
            };
        }
    }
}