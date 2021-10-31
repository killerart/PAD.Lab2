using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lab2.Proxy.LoadBalancer.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Lab2.Proxy.Middlewares {
    public class ProxyMiddleware {
        private readonly RequestDelegate _nextMiddleware;
        private readonly HttpClient      _httpClient;
        private readonly ILoadBalancer   _loadBalancer;

        public ProxyMiddleware(RequestDelegate nextMiddleware, IHttpClientFactory httpClientFactory, ILoadBalancer loadBalancer) {
            _nextMiddleware = nextMiddleware;
            _httpClient     = httpClientFactory.CreateClient("proxy");
            _loadBalancer   = loadBalancer;
        }

        public async Task Invoke(HttpContext context) {
            retry:
            var targetUri = GetTargetUri(context.Request);
            if (targetUri is null) {
                await _nextMiddleware(context);
                return;
            }

            var targetRequestMessage = CreateTargetMessage(context, targetUri);
            try {
                using var responseMessage =
                    await _httpClient.SendAsync(targetRequestMessage, context.RequestAborted);

                context.Response.StatusCode = (int)responseMessage.StatusCode;
                CopyFromTargetResponseHeaders(context, responseMessage);

                if (responseMessage.StatusCode != HttpStatusCode.NotModified)
                    await responseMessage.Content.CopyToAsync(context.Response.Body);
            } catch (HttpRequestException) {
                goto retry;
            }
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri) {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);

            requestMessage.RequestUri   = targetUri;
            requestMessage.Headers.Host = targetUri.Authority;
            requestMessage.Method       = GetMethod(context.Request.Method);

            return requestMessage;
        }

        private void CopyFromOriginalRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage) {
            var requestMethod = context.Request.Method;

            HttpHeaders headers = requestMessage.Headers;

            if (MethodHasBody(requestMethod)) {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;

                headers = requestMessage.Content.Headers;
            }

            foreach (var header in context.Request.Headers) {
                headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage) {
            foreach (var header in responseMessage.Headers) {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers) {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove("transfer-encoding");
        }

        private static HttpMethod GetMethod(string method) {
            if (HttpMethods.IsDelete(method))
                return HttpMethod.Delete;
            if (HttpMethods.IsGet(method))
                return HttpMethod.Get;
            if (HttpMethods.IsHead(method))
                return HttpMethod.Head;
            if (HttpMethods.IsOptions(method))
                return HttpMethod.Options;
            if (HttpMethods.IsPost(method))
                return HttpMethod.Post;
            if (HttpMethods.IsPut(method))
                return HttpMethod.Put;
            if (HttpMethods.IsTrace(method))
                return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private static bool MethodHasBody(string method) {
            return !HttpMethods.IsGet(method) &&
                   !HttpMethods.IsHead(method) &&
                   !HttpMethods.IsDelete(method) &&
                   !HttpMethods.IsTrace(method);
        }

        private Uri? GetTargetUri(HttpRequest request) {
            var host = _loadBalancer.GetNextWarehouse();
            return host is not null ? new Uri($"{host}{request.Path}") : null;
        }
    }

    public static class ProxyMiddlewareExtension {
        public static IApplicationBuilder UseProxy(this IApplicationBuilder app) {
            return app.UseMiddleware<ProxyMiddleware>();
        }
    }
}
