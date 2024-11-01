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

        public virtual void SetSystemPrompt(string systemPrompt)
        {
            _options[EngineConfigSections.SystemPrompt] = new SystemPromptConfigSection { Prompt = systemPrompt };
            OnSystemPromptChanged(systemPrompt);
        }

        protected abstract void OnSystemPromptChanged(string systemPrompt);
        protected abstract void ResetHistory();
        protected abstract Task<string> ExecuteCallAsync(string prompt);

        public async Task<string> CallAsync(string prompt, bool resetHistory = false, params EngineConfigSection[] engineConfigSections)
        {
            var originalSystemPrompt = _options.GetSection<SystemPromptConfigSection>();
            var tempSystemPrompt = engineConfigSections.OfType<SystemPromptConfigSection>().FirstOrDefault();

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

                var response = await ExecuteCallAsync(prompt);

                var jsonConfig = _options.GetSection<JsonEngineConfigSection>();
                if (jsonConfig?.ParseJson == true)
                {
                    response = new JsonExtractor().ExtractJSON(response);
                    OnJsonResponseProcessed(response);
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

        public abstract IAsyncEnumerable<string> StreamAsync(string prompt, bool resetHistory = false, params EngineConfigSection[] engineConfigSections);

        protected abstract void OnJsonResponseProcessed(string processedResponse);
    }
}
