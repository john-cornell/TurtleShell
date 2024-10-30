using Microsoft.Extensions.Configuration;
using TurtleShell;
using TurtleShell.Config;
using TurtleShell.Engines.Ollama;
using TurtleShell.Engines.OpenAI;
using static OllamaSharp.OllamaApiClient;

EngineConfigOptions options = new EngineConfigOptions();

//Options example
//options[EngineConfigSections.SystemPrompt] = new SystemPromptConfigSection { Prompt = "This is an example" };

//options.GetSection<JsonEngineConfigSection>()!.ParseJson = false; 
//options.GetSection<JsonEngineConfigSection>()!.JsonFormat = false;

options[EngineConfigSections.Json] = JsonEngineConfigSection.UseJsonFormat;

var engineModelId = new EngineModelId(EngineType.OpenAI, OpenAIModelIds.GPT4o);
//var engineModelId = new EngineModelId(EngineType.Ollama, OllamaModelIds.Gemma2_9b);
//options is optional

//Will initialize IConfiguration from appsettings.json, but IConfiguration can be passed directly as a named parameter
IEngine engine = EngineFactory.Start(engineModelId, options);

var response = await engine.CallAsync("What is the capital of France?");
Console.WriteLine(response);