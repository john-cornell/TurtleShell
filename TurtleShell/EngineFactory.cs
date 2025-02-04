using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Config;
using TurtleShell.Engines.AnthropicClaude;
using TurtleShell.Engines.GoogleGemini;
using TurtleShell.Engines.AzureOpenAI;
using TurtleShell.Engines.Ollama;
using TurtleShell.Engines.OpenAI;

namespace TurtleShell
{
    public class EngineFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="engineModelId"></param>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        /// <param name="customConfigurationSectionName">Custom section name in configuration file, currently only implemented in Azure</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEngine Start(EngineModelId engineModelId, EngineConfigOptions? options = null, IConfiguration? configuration = null, string customConfigurationSectionName = null)
        {
            if (configuration == null)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                configuration = builder.Build();
            }

            options = options ?? new EngineConfigOptions();

            switch (engineModelId.EngineType)
            {
                case EngineType.Anthropic:
                    return AnthropicEngine.Start(configuration, engineModelId, options);
                case EngineType.Ollama:
                    return OllamaEngine.Start(engineModelId, options);
                case EngineType.OpenAI:
                    return OpenAIGPTEngine.Start(configuration, engineModelId, options);
                case EngineType.GoogleGemini:
                    return GoogleGeminiAIEngine.Start(configuration, engineModelId, options);
                case EngineType.AzureOpenAI:
                    return AzureOpenAIGPTEngine.Start(configuration, engineModelId, options, customConfigurationSectionName ?? "AzureOpenAI");
                case EngineType.AzureRESTOpenAI:
                    return AzureRESTOpenAIGPTEngine.Start(configuration, engineModelId, options, customConfigurationSectionName ?? "AzureOpenAI");
                default:
                    throw new ArgumentException("Invalid engine type");
            }
        }
    }
}
