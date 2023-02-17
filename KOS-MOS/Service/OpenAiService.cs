using System.Net;
using Newtonsoft.Json;
using Discord.WebSocket;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace KOS_MOS.Service
{
    public class OpenAiService
    {
        private static readonly string? ChatGptApiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");

        /// <summary>
        /// Url to the OpenAI API.
        /// </summary>
        private const string ChatGptApiUrl = "https://api.openai.com/v1/completions";

        internal static async Task<bool> ChatGpt(SocketMessage message)
        {
            var client = new RestClient(ChatGptApiUrl);
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {ChatGptApiKey}");

            var data = new
            {
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
    }
}
