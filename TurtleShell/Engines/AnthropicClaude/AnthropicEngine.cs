using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Microsoft.Extensions.Configuration;
using OllamaSharp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Config;
using TurtleShell.Engines.Ollama;
using Message = Anthropic.SDK.Messaging.Message;

namespace TurtleShell.Engines.AnthropicClaude
{
    public class AnthropicEngine : BaseEngine
    {
        AnthropicClient _client;
        List<Message> _messages;
        SystemMessage _systemMessage;
        MessageParameters _messageParameters;
        public AnthropicEngine(IConfiguration configuration, EngineModelId engineModelId, EngineConfigOptions options) : base(engineModelId, options)
        {
            string? apiKey = configuration["Anthropic:ApiKey"];
            _client = new AnthropicClient(new APIAuthentication(apiKey));

            ResetHistory();
        }

        public static AnthropicEngine Start(IConfiguration configuration, EngineModelId engineModelId, EngineConfigOptions options = null)
        {
            return new AnthropicEngine(configuration, engineModelId, options);
        }

        protected override void AddUserMessageToHistory(string prompt) => _messages.Add(new Message(RoleType.User, prompt));

        protected override void AddAssistantMessageToHistory(string response) => _messages.Add(new Message(RoleType.Assistant, response));

        protected override void OnResponseParsed(string processedResponse)
        {
            _messages[^1] = new Message(RoleType.Assistant, processedResponse);
        }

        protected override async Task<string> ExecuteCallAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            var parameters = new MessageParameters()
            {
                Messages = _messages,
                System = new List<SystemMessage> { _systemMessage },
                Model = EngineModelId.ModelId,
                Stream = false,
                Temperature = 1.0m,
            };

            var response = await _client.Messages.GetClaudeMessageAsync(parameters);

            return response.Message;
        }

        protected override async IAsyncEnumerable<string> ExecuteStreamAsync(string prompt, params EngineConfigSection[] engineConfigSections)
        {
            var parameters = new MessageParameters()
            {
                Messages = _messages,
                System = new List<SystemMessage> { _systemMessage },
                Model = EngineModelId.ModelId,
                Stream = true,
                Temperature = 1.0m,
            };
            var outputs = new List<MessageResponse>();
            await foreach (var response in _client.Messages.StreamClaudeMessageAsync(parameters))
            {
                if (response.Delta != null)
                {
                    yield return response.Delta.Text;
                }
            }
        }

        protected override void OnSystemPromptChanged(string systemPrompt)
        {
            _systemMessage = new SystemMessage(systemPrompt);
        }

        protected override void ResetHistory()
        {
            var systemPrompt = _options.GetSection<SystemPromptConfigSection>();
            _systemMessage = new SystemMessage(systemPrompt.Prompt);

            _messages = new List<Message>();
        }
    }


}
