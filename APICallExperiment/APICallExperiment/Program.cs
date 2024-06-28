using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace APICallExperiment
{
    internal class Program
    {
        static async Task Main()
        {
            // using HttpClient client = new();
            // client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");

            await MakeApiCallAsync("https://dog.ceo/api/breeds/image/random");
        }

        /// <summary>
        /// This method makes an api-call with given endpoint and optional http client.
        /// </summary>
        /// <param name="apiEndpoint">The tageted endpoint of the api.</param>
        /// <param name="client">A client with added headers if needed.</param>
        /// <returns>All properties from the response as key/value pairs.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private static async Task<Dictionary<string, string>> MakeApiCallAsync(string apiEndpoint, HttpClient? client = null)
        {
            client ??= new HttpClient();

            HttpResponseMessage responseMessage = await client.GetAsync(apiEndpoint);

            if (responseMessage.IsSuccessStatusCode)
            {
                string jsonContent = await responseMessage.Content.ReadAsStringAsync();

                Dictionary<string, string>? content = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

                return content is null ? throw new Exception("Unable to parse content.") : content;
            }
            else
            {
                HttpStatusCode statusCode = responseMessage.StatusCode;
                string message = $"HTTP {statusCode}: {responseMessage.ReasonPhrase}";

                throw statusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => new HttpRequestException($"Bad Request: {message}"),
                    System.Net.HttpStatusCode.Unauthorized => new HttpRequestException($"Unauthorized: {message}"),
                    System.Net.HttpStatusCode.Forbidden => new HttpRequestException($"Forbidden: {message}"),
                    System.Net.HttpStatusCode.NotFound => new HttpRequestException($"Not Found: {message}"),
                    System.Net.HttpStatusCode.InternalServerError => new HttpRequestException($"Internal Server Error: {message}"),
                    _ => new HttpRequestException($"HTTP Error: {message}"),
                };
            }
        }

        /*
        private static async Task TestMakeApiCallAsync()
        {
            string apiEndpoint = "https://dog.ceo/api/breeds/image/random";

            HttpClient client = new();

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    Dictionary<string, string>? jsonContent = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

                    if (jsonContent is not null)
                    {
                        Console.WriteLine($"Content of the response was:\n{jsonContent["message"]}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse content using the JsonSerializer.");
                    }
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            Console.WriteLine("Bad Request: The request was invalid.");
                            break;
                        case System.Net.HttpStatusCode.Unauthorized:
                            Console.WriteLine("Unauthorized: Access is denied due to invalid credentials.");
                            break;
                        case System.Net.HttpStatusCode.Forbidden:
                            Console.WriteLine("Forbidden: The server understood the request, but refuses to authorize it.");
                            break;
                        case System.Net.HttpStatusCode.NotFound:
                            Console.WriteLine("Not Found: The requested resource could not be found.");
                            break;
                        case System.Net.HttpStatusCode.InternalServerError:
                            Console.WriteLine("Internal Server Error: The server encountered an error.");
                            break;
                        default:
                            Console.WriteLine($"Error: {response.StatusCode}");
                            break;
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                Console.WriteLine("Request error: " + hre.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error: " + e.Message);
            }
        }
        */
    }
}
