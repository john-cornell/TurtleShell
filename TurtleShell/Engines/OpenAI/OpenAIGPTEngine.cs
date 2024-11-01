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

        protected override void ResetHistory()
        {
            var systemPrompt = _options.GetSection<SystemPromptConfigSection>();
            _chatHistory = new ChatHistory(systemPrompt?.Prompt ?? string.Empty);
        }

        protected override async Task<string> ExecuteCallAsync(string prompt)
        {
            _chatHistory.AddUserMessage(prompt);
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            var response = await chatCompletionService.GetChatMessageContentsAsync(_chatHistory);
            var content = response.First().Content;
            _chatHistory.AddAssistantMessage(content);
            return content;
        }

        public override async IAsyncEnumerable<string> StreamAsync(string prompt, bool resetHistory = false, params EngineConfigSection[] engineConfigSections)
        {
            if (resetHistory) ResetHistory();
            _chatHistory.AddUserMessage(prompt);

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            var fullResponse = new StringBuilder();

            await foreach (var chunk in chatCompletionService.GetStreamingChatMessageContentsAsync(_chatHistory))
            {
                fullResponse.Append(chunk.Content);
                yield return chunk.Content;
            }

            _chatHistory.AddAssistantMessage(fullResponse.ToString());
        }

        protected override void OnJsonResponseProcessed(string processedResponse)
        {
            _chatHistory[^1].Content = processedResponse;
        }
    }
}
