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

        protected override async IAsyncEnumerable<string> ExecuteStreamAsync(string prompt)
        {
            throw new NotImplementedException();
        }

        protected override async Task<string> ExecuteCallAsync(string prompt)
        {
            Message message = new Message(RoleType.User, prompt);

            var parameters = new MessageParameters()
            {
                Messages = _messages,
                //MaxTokens = 1024,
                Model = AnthropicModels.Claude35Sonnet,
                Stream = false,
                Temperature = 1.0m,
            };

            var response = await _client.Messages.GetClaudeMessageAsync(parameters);

            return response.Message;
        }

        protected override void OnJsonResponseProcessed(string processedResponse)
        {
            throw new NotImplementedException();
        }

        protected override void OnSystemPromptChanged(string systemPrompt)
        {
            throw new NotImplementedException();
        }

        protected override void ResetHistory()
        {            
            var systemPrompt = _options.GetSection<SystemPromptConfigSection>();
            _systemMessage = new SystemMessage(systemPrompt.Prompt);

            _messages = new List<Message>();
        }
    }

    
}
