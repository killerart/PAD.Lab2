using Lab2.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab2.Warehouse.Filters {
    public class RequestExceptionFilter : IExceptionFilter {
        public void OnException(ExceptionContext context) {
            context.Result = context.Exception switch {
                RequestException exception => new ObjectResult(new ProblemDetails {
                    Detail = exception.Message
                }) {
                    StatusCode = (int)exception.StatusCode
                },
                _ => context.Result
            };
        }
    }
}
