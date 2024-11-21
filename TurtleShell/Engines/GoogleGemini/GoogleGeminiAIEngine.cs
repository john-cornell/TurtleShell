using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TurtleShell.Config;

namespace TurtleShell.Engines.GoogleGemini
{
    public class GoogleGeminiAIEngine : BaseEngine
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private  List<ChatMessage> _chatHistory;

        private GoogleGeminiAIEngine(IConfiguration configuration, EngineModelId engineModelId, EngineConfigOptions options)
            : base(engineModelId, options)
        {
            _apiKey = configuration["Google:Gemini:ApiKey"];
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new ArgumentException("Google API Key is missing in the configuration.");
            }

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/")
            };

            _chatHistory = new List<ChatMessage>();
        }

        public static GoogleGeminiAIEngine Start(IConfiguration configuration, EngineModelId engineModelId, EngineConfigOptions options = null)
        {
            return new GoogleGeminiAIEngine(configuration, engineModelId, options);
        }

        
        protected override void OnSystemPromptChanged(string systemPrompt)
        {
            if (_chatHistory.Count == 0)
            {
                _chatHistory.Add(new ChatMessage
                {
                    Role = ChatRole.Model,
                    Parts = new List<ChatPart> { new ChatPart { Text = systemPrompt } }
                });
            }
            else
            {
                _chatHistory[0] = new ChatMessage
                {
                    Role = ChatRole.Model,
                    Parts = new List<ChatPart> { new ChatPart { Text = systemPrompt } }
                };
            }
        }

        protected override void ResetHistory()
        {
            var systemPrompt = _options.GetSection<SystemPromptConfigSection>();
            _chatHistory = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = ChatRole.Model,
                    Parts = new List<ChatPart> { new ChatPart { Text = systemPrompt!.Prompt } }
                }
            };

        }

        protected override async Task<string> ExecuteCallAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            var requestBody = new
            {
                contents = _chatHistory.Select(message => new
                {
                    role = message.Role,
                    parts = message.Parts.Select(part => new { text = part.Text }).ToArray()
                }).ToArray()
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, $"models/{EngineModelId.ModelId}:generateContent?key={_apiKey}")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Google Gemini API call failed with status code {response.StatusCode} and message: {await response.Content.ReadAsStringAsync()}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);

            // Extract the text from the first candidate's content parts
            var assistantResponse = parsedResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? string.Empty;

            return assistantResponse;
        }

        protected override async IAsyncEnumerable<string> ExecuteStreamAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            // Add the user message to the chat history
            var requestBody = new
            {
                contents = _chatHistory.Select(message => new
                {
                    role = message.Role,
                    parts = message.Parts.Select(part => new { text = part.Text }).ToArray()
                }).ToArray()
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, $"models/{EngineModelId.ModelId}:streamGenerateContent?alt=sse&key={_apiKey}")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Google Gemini API streaming call failed with status code {response.StatusCode} and message: {await response.Content.ReadAsStringAsync()}");
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);

            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();

                // Filter for lines containing "text"
                if (line != null && line.Contains("\"text\""))
                {
                    var textStartIndex = line.IndexOf("\"text\":") + 8;
                    var textEndIndex = line.IndexOf("\"", textStartIndex);
                    var text = line.Substring(textStartIndex, textEndIndex - textStartIndex);

                    yield return text;
                }
            }
        }

        protected override void AddAssistantMessageToHistory(string response)
        {
            _chatHistory.Add(new ChatMessage
            {
                Role = ChatRole.Model,
                Parts = new List<ChatPart> { new ChatPart { Text = response } }
            });
        }

        protected override void AddUserMessageToHistory(string prompt)
        {
            _chatHistory.Add(new ChatMessage
            {
                Role = ChatRole.User,
                Parts = new List<ChatPart> { new ChatPart { Text = prompt } }
            });
        }

        protected override void OnResponseParsed(string processedResponse)
        {
            // Handle any post-processing of the response if needed
        }

        private class ChatRole
        {
            public const string User = "user";
            public const string Model = "model";
            public const string SystemInstruction = "system_instruction";
        }

        private class ChatMessage
        {
            public string Role { get; set; }
            public List<ChatPart> Parts { get; set; }
        }

        private class ChatPart
        {
            public string Text { get; set; }
        }

        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate> Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content Content { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public List<Part> Parts { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }
    }
}