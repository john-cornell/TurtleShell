﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Config;
using TurtleShell.Engines.Ollama;
using TurtleShell.Engines.OpenAI;

namespace TurtleShell
{
    public class EngineFactory
    { 
        public static IEngine Start(EngineModelId engineModelId, EngineConfigOptions? options = null, IConfiguration? configuration = null)
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
                case EngineType.Ollama:
                    return OllamaEngine.Start(engineModelId, options);
                case EngineType.OpenAI:
                    return OpenAIGPTEngine.Start(configuration, engineModelId, options);
                default:
                    throw new ArgumentException("Invalid engine type");
            }
        }
    }
}
