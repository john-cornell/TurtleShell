using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TurtleShell.Config;
using TurtleShell.Config.Settings;


namespace TurtleShell.Engines.AzureOpenAI
{
    internal class AzureOpenAIGPTEngine : BaseEngine
    {
        private readonly Kernel _kernel;
        private ChatHistory _chatHistory;

        private AzureOpenAIGPTEngine(IConfiguration configuration, EngineModelId engineModelId,
            EngineConfigOptions options, string configurationSectionName)
            : base(engineModelId, options)
        {
            // Get AppSettings section
            var appSettings = configuration.GetSection(configurationSectionName).Get<AzureGptSettings>();

            if (appSettings == null)
            {
                throw new Exception("AppSettings section is missing from the configuration file.");
            }

            string endpoint = appSettings.Endpoint;

            string apiKey = appSettings.ApiKey;
            string apiVersion = appSettings.ApiVersion;

            if (appSettings.SpecifyApiVersion && !string.IsNullOrWhiteSpace(appSettings.ApiVersion))
            {

                _kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(
                        engineModelId.ModelId, endpoint, apiKey, apiVersion: apiVersion)
                    .Build();
            }
            else
            {
                _kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(
                        engineModelId.ModelId, endpoint, apiKey)
                    .Build();
            }
        }

        public static AzureOpenAIGPTEngine Start(IConfiguration configuration, EngineModelId engineModelId, EngineConfigOptions options = null, string configurationSectionName = "AzureOpenAI")
        {
            return new AzureOpenAIGPTEngine(configuration, engineModelId, options, configurationSectionName);
        }

        protected override void OnSystemPromptChanged(string systemPrompt)
        {
            ResetHistory();
        }

        public override void ResetHistory()
        {
            var systemPrompt = _options.GetSection<SystemPromptConfigSection>();
            _chatHistory = new ChatHistory(systemPrompt?.Prompt ?? string.Empty);
        }

        protected override async Task<string> ExecuteCallAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var response = await chatCompletionService.GetChatMessageContentsAsync(_chatHistory);
            return response.First().Content;
        }

        protected override async IAsyncEnumerable<string> ExecuteStreamAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            await foreach (var chunk in chatCompletionService.GetStreamingChatMessageContentsAsync(_chatHistory))
            {
                yield return chunk.Content;
            }
        }

        protected override void AddAssistantMessageToHistory(string response) => _chatHistory.AddAssistantMessage(response);
        protected override void AddUserMessageToHistory(string prompt) => _chatHistory.AddUserMessage(prompt);

        protected override void OnResponseParsed(string processedResponse)
        {
            _chatHistory[^1].Content = processedResponse;
        }
    }
}
