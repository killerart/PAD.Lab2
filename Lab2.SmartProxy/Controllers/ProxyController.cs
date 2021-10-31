using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lab2.SmartProxy.Proxy.LoadBalancer.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.SmartProxy.Controllers {
    public class ProxyController : ControllerBase, IDisposable {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoadBalancer      _loadBalancer;

        private HttpResponseMessage? _responseMessage;

        public ProxyController(IHttpClientFactory httpClientFactory, ILoadBalancer loadBalancer) {
            _httpClientFactory = httpClientFactory;
            _loadBalancer      = loadBalancer;
        }
        
        public async Task<IActionResult> Index() {
            int attempts = 0;

            var httpClient           = _httpClientFactory.CreateClient("proxy");
            var targetRequestMessage = CreateTargetMessage(HttpContext);

            retry:
            var targetUri = GetTargetUri(Request);
            if (attempts >= _loadBalancer.Count * 2) {
                return StatusCode(StatusCodes.Status502BadGateway);
            }

            SetTargetUri(targetRequestMessage, targetUri);

            try {
                _responseMessage = await httpClient.SendAsync(targetRequestMessage, HttpContext.RequestAborted);

                Response.StatusCode = (int)_responseMessage.StatusCode;
                CopyFromTargetResponseHeaders(HttpContext, _responseMessage);

                if (_responseMessage.StatusCode == HttpStatusCode.NotModified) {
                    return StatusCode(Response.StatusCode);
                }

                return StatusCode(Response.StatusCode, await _responseMessage.Content.ReadAsStreamAsync(HttpContext.RequestAborted));
            } catch (HttpRequestException) {
                attempts++;
                goto retry;
            }
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context) {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);
            requestMessage.Method = GetMethod(context.Request.Method);

            return requestMessage;
        }

        private void SetTargetUri(HttpRequestMessage requestMessage, Uri targetUri) {
            requestMessage.RequestUri   = targetUri;
            requestMessage.Headers.Host = targetUri.Authority;
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

        private Uri GetTargetUri(HttpRequest request) {
            var host = _loadBalancer.GetNextWarehouse();
            return new Uri($"{host}{request.Path}");
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                _responseMessage?.Dispose();
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ProxyController() {
            Dispose(false);
        }
    }
}
