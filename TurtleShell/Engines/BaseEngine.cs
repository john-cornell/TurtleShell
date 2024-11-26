using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Config;
using TurtleShell.TextProcessing;

namespace TurtleShell.Engines
{
    public abstract class BaseEngine : IEngine
    {
        protected EngineConfigOptions _options;
        public EngineModelId EngineModelId { get; protected set; }

        protected BaseEngine(EngineModelId engineModelId, EngineConfigOptions options)
        {
            EngineModelId = engineModelId;
            _options = options ?? new EngineConfigOptions();
            Initialize();
        }

        protected BaseEngine(EngineModelId engineModelId, params EngineConfigSection[] options)
        {
            EngineModelId = engineModelId;
            InitializeOptions(options);
            Initialize();
        }

        protected virtual void Initialize()
        {
            ResetHistory();
        }

        protected void InitializeOptions(params EngineConfigSection[] options)
        {
            _options = new EngineConfigOptions();
            foreach (var option in options)
            {
                _options.AddSection(option);
            }
        }

        public void SetSystemPrompt(string systemPrompt)
        {
            CustomSetSystemPrompt(systemPrompt);
            OnSystemPromptChanged(systemPrompt);
        }

        public virtual void CustomSetSystemPrompt(string systemPrompt)
        {
            _options[EngineConfigSections.SystemPrompt] = new SystemPromptConfigSection { Prompt = systemPrompt };
        }

        protected abstract void OnSystemPromptChanged(string systemPrompt);
        public abstract void ResetHistory();
        protected abstract Task<string> ExecuteCallAsync(string prompt, params EngineConfigSection[] engineConfigSections);
        protected abstract IAsyncEnumerable<string> ExecuteStreamAsync(string prompt, params EngineConfigSection[] engineConfigSections);

        public async Task<string> CallAsync(string prompt, bool resetHistory = false, params EngineConfigSection[] engineConfigSections)
        {
            var originalSystemPrompt = _options.GetSection<SystemPromptConfigSection>();
            var tempSystemPrompt = engineConfigSections.OfType<SystemPromptConfigSection>().FirstOrDefault();

            var jsonFormatOption =
                engineConfigSections.OfType<JsonEngineConfigSection>().FirstOrDefault()
                ?? _options.GetSection<JsonEngineConfigSection>();

            try
            {
                if (tempSystemPrompt != null)
                {
                    SetSystemPrompt(tempSystemPrompt.Prompt);
                }

                if (resetHistory)
                {
                    ResetHistory();
                }

                AddUserMessageToHistory(prompt);

                var response = await ExecuteCallAsync(prompt, engineConfigSections);

                AddAssistantMessageToHistory(response);

                if (jsonFormatOption?.ParseJson == true)
                {
                    response = new JsonExtractor().ExtractJSON(response);
                    OnResponseParsed(response);
                }

                return response;
            }
            finally
            {
                if (tempSystemPrompt != null)
                {
                    SetSystemPrompt(originalSystemPrompt.Prompt);
                }
            }
        }

        public async IAsyncEnumerable<string> StreamAsync(string prompt, bool resetHistory = false, params EngineConfigSection[] engineConfigSections)
        {
            var originalSystemPrompt = _options.GetSection<SystemPromptConfigSection>();
            var tempSystemPrompt = engineConfigSections.OfType<SystemPromptConfigSection>().FirstOrDefault();
            var jsonFormatOption =
                engineConfigSections.OfType<JsonEngineConfigSection>().FirstOrDefault()
                ?? _options.GetSection<JsonEngineConfigSection>();

            StringBuilder complete = new StringBuilder();

            try
            {
                if (tempSystemPrompt != null)
                {
                    SetSystemPrompt(tempSystemPrompt.Prompt);
                }

                if (resetHistory)
                {
                    ResetHistory();
                }
                AddUserMessageToHistory(prompt);
                await foreach (var chunk in ExecuteStreamAsync(prompt, engineConfigSections))
                {
                    complete.Append(chunk);
                    yield return chunk;
                }

                string assistantResponse = complete.ToString();

                if (jsonFormatOption?.ParseJson == true)
                {
                    assistantResponse = new JsonExtractor().ExtractJSON(assistantResponse);
                    OnResponseParsed(assistantResponse);
                }

                AddAssistantMessageToHistory(complete.ToString());
            }
            finally
            {
                if (tempSystemPrompt != null)
                {
                    SetSystemPrompt(originalSystemPrompt.Prompt);
                }
            }
        }

        protected abstract void AddUserMessageToHistory(string prompt);
        protected abstract void AddAssistantMessageToHistory(string response);

        protected abstract void OnResponseParsed(string processedResponse);
    }
}
