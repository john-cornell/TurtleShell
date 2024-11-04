using OllamaSharp.Models.Chat;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Config;

namespace TurtleShell.Engines.Ollama
{
    public class OllamaEngine : BaseEngine
    {
        private List<Message> _conversationHistory;
        private readonly Uri _uri = new Uri("http://localhost:11434");
        private OllamaApiClient _ollama;

        private OllamaEngine(EngineModelId engineModelId, EngineConfigOptions options)
            : base(engineModelId, options)
        {
        }

        protected override void Initialize()
        {
            _ollama = new OllamaApiClient(_uri)
            {
                SelectedModel = EngineModelId.ModelId
            };
            base.Initialize();
        }

        public static OllamaEngine Start(EngineModelId engineModelId, EngineConfigOptions options = null)
        {
            return new OllamaEngine(engineModelId, options);
        }

        protected override void OnSystemPromptChanged(string systemPrompt)
        {
            _conversationHistory[0] = new Message { Role = ChatRole.System, Content = systemPrompt };
        }

        protected override void ResetHistory()
        {
            var systemPrompt = _options.GetSection<SystemPromptConfigSection>();
            _conversationHistory = new List<Message>
                {
                    new Message { Role = ChatRole.System, Content = systemPrompt!.Prompt }
                };
        }

        protected override async Task<string> ExecuteCallAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            _conversationHistory.Add(new Message { Role = ChatRole.User, Content = prompt });

            var request = new ChatRequest
            {
                Model = _ollama.SelectedModel,
                Messages = _conversationHistory,
                Format = _options.GetSection<JsonEngineConfigSection>()?.JsonFormat == true ? "json" : null
            };

            StringBuilder builder = new StringBuilder();
            await foreach (var answerToken in _ollama.Chat(request))
            {
                builder.Append(answerToken.Message.Content);
            }

            var response = builder.ToString();
            _conversationHistory.Add(new Message { Role = ChatRole.Assistant, Content = response });
            return response;
        }

        protected override async IAsyncEnumerable<string> ExecuteStreamAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            var request = new ChatRequest
            {
                Model = _ollama.SelectedModel,
                Messages = _conversationHistory,
                Format = _options.GetSection<JsonEngineConfigSection>()?.JsonFormat == true ? "json" : null
            };

            var fullResponse = new StringBuilder();
            await foreach (var answerToken in _ollama.Chat(request))
            {
                fullResponse.Append(answerToken.Message.Content);
                yield return answerToken.Message.Content;
            }
        }

        protected override void AddUserMessageToHistory(string prompt) =>
            _conversationHistory.Add(new Message { Role = ChatRole.User, Content = prompt });

        protected override void AddAssistantMessageToHistory(string response) => 
            _conversationHistory.Add(new Message { Role = ChatRole.Assistant, Content = response });
        
        protected override void OnResponseParsed(string processedResponse) =>
            _conversationHistory[^1].Content = processedResponse;
    }
}
