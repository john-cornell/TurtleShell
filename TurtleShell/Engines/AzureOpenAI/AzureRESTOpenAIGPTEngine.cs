using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TurtleShell.Config;
using TurtleShell.Config.Settings;

namespace TurtleShell.Engines.AzureOpenAI
{
    internal class AzureRESTOpenAIGPTEngine : BaseEngine
    {
        private readonly AzureOpenAIService _azureOpenAIService;
        private List<ChatMessage> _messages = new();

        private AzureRESTOpenAIGPTEngine(IConfiguration configuration, EngineModelId engineModelId,
            EngineConfigOptions options, string configurationSectionName)
            : base(engineModelId, options)
        {
            var appSettings = configuration.GetSection(configurationSectionName).Get<AzureGptSettings>();

            if (appSettings == null)
            {
                throw new Exception("AppSettings section is missing from the configuration file.");
            }

            string endpoint = appSettings.Endpoint;
            string apiKey = appSettings.ApiKey;
            string apiVersion = appSettings.SpecifyApiVersion ? appSettings.ApiVersion : "2024-02-01";

            _azureOpenAIService = new AzureOpenAIService(
                endpoint,
                apiKey,
                engineModelId.ModelId,
                apiVersion);
        }

        public static AzureRESTOpenAIGPTEngine Start(IConfiguration configuration, EngineModelId engineModelId,
            EngineConfigOptions options = null, string configurationSectionName = "AzureOpenAI")
        {
            return new AzureRESTOpenAIGPTEngine(configuration, engineModelId, options, configurationSectionName);
        }

        protected override void OnSystemPromptChanged(string systemPrompt)
        {
            ResetHistory();
        }

        public override void ResetHistory()
        {
            var systemPrompt = _options.GetSection<SystemPromptConfigSection>();
            _messages.Clear();

            if (!string.IsNullOrEmpty(systemPrompt?.Prompt))
            {
                _messages.Add(new ChatMessage(Role.System, systemPrompt.Prompt));
            }
        }

        protected override async Task<string> ExecuteCallAsync(string prompt,
            params EngineConfigSection[] engineConfigSections)
        {
            try
            {
                var response = await _azureOpenAIService.GetCompletionAsync(_messages);
                _messages.Add(new ChatMessage(Role.Assistant, response));
                return response;
            }
            catch (HttpRequestException ex)
            {
                // Handle API errors
                Console.WriteLine($"API Error: {ex.Message}");
                throw;
            }
        }

        protected override IAsyncEnumerable<string> ExecuteStreamAsync(string prompt,
            params EngineConfigSection[] engineConfigSections)
        {
            // Streaming implementation would require different API handling
            throw new NotImplementedException("Streaming not implemented for REST API version");
        }

        protected override void AddAssistantMessageToHistory(string response) =>
            _messages.Add(new ChatMessage(Role.Assistant, response));

        protected override void AddUserMessageToHistory(string prompt) =>
            _messages.Add(new ChatMessage(Role.User, prompt));

        protected override void OnResponseParsed(string processedResponse)
        {
            if (_messages.Count > 0 && _messages[^1].Role == Role.Assistant)
            {
                _messages[^1] = new ChatMessage(Role.Assistant, processedResponse);
            }
        }
    }

    internal class AzureOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string _deploymentName;
        private readonly string _apiVersion;

        public AzureOpenAIService(string endpoint, string apiKey, string deploymentName,
            string apiVersion = "2024-02-01")
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _endpoint = endpoint;
            _deploymentName = deploymentName;
            _apiVersion = apiVersion;
        }

        public async Task<string> GetCompletionAsync(List<ChatMessage> messages)
        {
            var requestBody = new
            {
                messages = messages.Select(m => new
                {
                    role = m.Role.ToString().ToLower(),
                    content = m.Content
                })
            };

            var uri = $"{_endpoint}openai/deployments/{_deploymentName}/chat/completions?api-version={_apiVersion}";

            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Add("api-key", _apiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

            return responseObject?.Choices.FirstOrDefault()?.Message?.Content ?? string.Empty;
        }
    }

    internal enum Role
    {
        System,
        User,
        Assistant
    }

    internal record ChatMessage(Role Role, string Content);
}