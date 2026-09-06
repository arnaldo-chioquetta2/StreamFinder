using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace StreamFinder.WinForms.Services
{
    public class HttpService
    {
        private static readonly HttpClient Client = CreateClient();

        public async Task<string> GetStringAsync(string url)
        {
            return await GetStringAsync(url, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken)
        {
            var requestUri = ValidateUrl(url);

            try
            {
                using (var response = await Client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpServiceException(
                            string.Format("A requisição HTTP falhou com status {0} ({1}).", (int)response.StatusCode, response.StatusCode),
                            response.StatusCode);
                    }

                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (HttpServiceException)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                throw new HttpServiceException("A requisição HTTP excedeu o tempo limite.", null, ex);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpServiceException("Não foi possível concluir a requisição HTTP.", null, ex);
            }
            catch (Exception ex)
            {
                throw new HttpServiceException("Ocorreu um erro ao realizar a requisição HTTP.", null, ex);
            }
        }

        public async Task<string> GetJsonAsync(string url)
        {
            return await GetJsonAsync(url, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<string> GetJsonAsync(string url, CancellationToken cancellationToken)
        {
            return await GetStringAsync(url, cancellationToken).ConfigureAwait(false);
        }

        public async Task<byte[]> GetBytesAsync(string url)
        {
            return await GetBytesAsync(url, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<byte[]> GetBytesAsync(string url, CancellationToken cancellationToken)
        {
            var requestUri = ValidateUrl(url);

            try
            {
                using (var response = await Client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpServiceException(
                            string.Format("A requisição HTTP falhou com status {0} ({1}).", (int)response.StatusCode, response.StatusCode),
                            response.StatusCode);
                    }

                    return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                }
            }
            catch (HttpServiceException)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                throw new HttpServiceException("A requisição HTTP excedeu o tempo limite.", null, ex);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpServiceException("Não foi possível concluir a requisição HTTP.", null, ex);
            }
            catch (Exception ex)
            {
                throw new HttpServiceException("Ocorreu um erro ao realizar a requisição HTTP.", null, ex);
            }
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd("StreamFinder/1.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static Uri ValidateUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new HttpServiceException("A URL deve ser absoluta e começar com http:// ou https://.");
            }

            return uri;
        }
    }

    public class HttpServiceException : Exception
    {
        public HttpServiceException(string message)
            : this(message, null, null)
        {
        }

        public HttpServiceException(string message, HttpStatusCode? statusCode)
            : this(message, statusCode, null)
        {
        }

        public HttpServiceException(string message, HttpStatusCode? statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode? StatusCode { get; private set; }
    }
}
