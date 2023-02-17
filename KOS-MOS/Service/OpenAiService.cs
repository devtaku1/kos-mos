using System.Net;
using Newtonsoft.Json;
using Discord.WebSocket;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace KOS_MOS.Service
{
    public class OpenAiService
    {
        /// <summary>
        ///     Api Key to access OpenAI Apis like ChatGPT - (Replace this with your OpenAi API key in your environment)
        /// </summary>
        private static readonly string? OpenAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        /// <summary>
        /// Url to the OpenAI API.
        /// </summary>
        private const string ChatGptApiUrl = "https://api.openai.com/v1/completions";

        /// <summary>
        ///     Url to the Dalle API.
        /// </summary>
        private const string DalleApiUrl = "https://api.openai.com/v1/images/generations";

        /// <summary>
        ///     The method uses the RestClient class to send a request to the ChatGPT API, passing the user's message as the
        ///     prompt and sends the response into the Chat
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Boolean indicating whether the request was successful</returns>
        internal static async Task<bool> ChatGpt(SocketMessage message)
        {
            var client = new RestClient(ChatGptApiUrl);
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {OpenAiApiKey}");

            var data = new
            {
                // The prompt is everything after the !chat command
                model = "text-davinci-003",
                prompt = message.Content[4..],
                max_tokens = 256,
            };

            var jsonData = JsonSerializer.Serialize(data);

            request.AddJsonBody(jsonData);

            var response = await client.ExecuteAsync(request);
            string? responseText;
            var success = false;
            if (response.Content != null && response.StatusCode == HttpStatusCode.OK)
            {
                responseText = JsonConvert.DeserializeObject<dynamic>(response.Content)?["choices"][0]["text"];
                success = true;
            }
            else
            {
                responseText = response.ErrorMessage;
            }

            await message.Channel.SendMessageAsync(responseText);
            return success;
        }


        /// <summary>
        ///     The method uses the RestClient class to send a request to the Dall-E API, passing the user's message as the
        ///     prompt and sends an image to the Chat
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Boolean indicating whether the request was successful</returns>
        internal static async Task<bool> DallE(SocketMessage message)
        {
            var client = new RestClient(DalleApiUrl);
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {OpenAiApiKey}");

            var data = new
            {
                // The prompt is everything after the !image command
                //model = "image-alpha-001",
                prompt = message.Content[5..],
                n = 1,
                size = "1024x1024"
            };

            var jsonData = JsonSerializer.Serialize(data);

            request.AddJsonBody(jsonData);

            var response = await client.ExecuteAsync(request);
            string? responseText;
            var success = false;
            if (response.Content != null && response.StatusCode == HttpStatusCode.OK)
            {
                var imageUrl = JsonConvert.DeserializeObject<dynamic>(response.Content)?["data"][0]["url"];
                responseText = $"Here is the generated image: {imageUrl}";
                success = true;
            }
            else
            {
                responseText = response.ErrorMessage;
            }

            await message.Channel.SendMessageAsync(responseText);
            return success;
        }
    }
}
