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

        [ResponseCache(CacheProfileName = "proxy")]
        public async Task<IActionResult> Index() {
            var attempts   = 0;
            var httpClient = _httpClientFactory.CreateClient("proxy");

            retry:
            var targetRequestMessage = CreateProxyRequestMessage();

            try {
                _responseMessage = await httpClient.SendAsync(targetRequestMessage, HttpContext.RequestAborted);

                Response.StatusCode = (int)_responseMessage.StatusCode;
                CopyResponseHeadersToResponse(_responseMessage);

                if (_responseMessage.StatusCode == HttpStatusCode.NotModified) {
                    return StatusCode(Response.StatusCode);
                }

                var stream = await _responseMessage.Content.ReadAsStreamAsync();
                return StatusCode(Response.StatusCode, stream);
            } catch (HttpRequestException) {
                attempts++;
                if (attempts >= _loadBalancer.Count * 2) {
                    return StatusCode(StatusCodes.Status502BadGateway);
                }

                goto retry;
            }
        }

        private HttpRequestMessage CreateProxyRequestMessage() {
            var targetUri      = GetTargetUri(Request);
            var requestMessage = new HttpRequestMessage();
            CopyContentAndHeadersFromRequest(requestMessage);
            requestMessage.RequestUri   = targetUri;
            requestMessage.Headers.Host = targetUri.Authority;
            requestMessage.Method       = GetMethod(Request.Method);
            return requestMessage;
        }

        private Uri GetTargetUri(HttpRequest request) {
            var host = _loadBalancer.GetNextWarehouse();
            return new Uri($"{host}{request.Path}");
        }

        private void CopyContentAndHeadersFromRequest(HttpRequestMessage requestMessage) {
            var requestMethod = Request.Method;

            HttpHeaders headers = requestMessage.Headers;

            if (MethodHasBody(requestMethod)) {
                var streamContent = new StreamContent(Request.Body);
                requestMessage.Content = streamContent;

                headers = requestMessage.Content.Headers;
            }

            foreach (var header in Request.Headers) {
                headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private void CopyResponseHeadersToResponse(HttpResponseMessage responseMessage) {
            foreach (var header in responseMessage.Headers) {
                Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers) {
                Response.Headers[header.Key] = header.Value.ToArray();
            }

            Response.Headers.Remove("transfer-encoding");
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

        public void Dispose() {
            _responseMessage?.Dispose();
        }
    }
}
