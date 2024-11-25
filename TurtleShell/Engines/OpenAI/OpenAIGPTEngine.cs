using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Config;

namespace TurtleShell.Engines.OpenAI
{
    public class OpenAIGPTEngine : BaseEngine
    {
        private readonly Kernel _kernel;
        private ChatHistory _chatHistory;

        private OpenAIGPTEngine(IConfiguration configuration, EngineModelId engineModelId, EngineConfigOptions options)
            : base(engineModelId, options)
        {
            string? apiKey = configuration["OpenAI:ApiKey"];
            _kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(engineModelId.ModelId, apiKey ?? "")
                .Build();
        }

        public static OpenAIGPTEngine Start(IConfiguration configuration, EngineModelId engineModelId, EngineConfigOptions options = null)
        {
            return new OpenAIGPTEngine(configuration, engineModelId, options);
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
