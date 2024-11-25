using Microsoft.Extensions.Configuration;
using TurtleShell;
using TurtleShell.Config;
using TurtleShell.Engines.AnthropicClaude;
using TurtleShell.Engines.AzureOpenAI;
using TurtleShell.Engines.Ollama;
using TurtleShell.Engines.OpenAI;
using static OllamaSharp.OllamaApiClient;

EngineConfigOptions options = new EngineConfigOptions();

//Options example
//options[EngineConfigSections.SystemPrompt] = new SystemPromptConfigSection { Prompt = "This is an example" };

options.GetSection<JsonEngineConfigSection>()!.ParseJson = false;
options.GetSection<JsonEngineConfigSection>()!.JsonFormat = false;

//options[EngineConfigSections.Json] = JsonEngineConfigSection.UseJsonFormat;

//var engineModelId = new EngineModelId(EngineType.OpenAI, OpenAIModelIds.GPT4o);
//var engineModelId = new EngineModelId(EngineType.Ollama, OllamaModelIds.Phi3);
//var engineModelId = new EngineModelId(EngineType.Anthropic, AnthropicModelIds.Claude3_Haiku_20240307);
var engineModelId = new EngineModelId(EngineType.AzureOpenAI, AzureOpenAIModelIds.GPT4o);
//options is optional

//Will initialize IConfiguration from appsettings.json, but IConfiguration can be passed directly as a named parameter
IEngine engine = EngineFactory.Start(engineModelId, options);
engine.SetSystemPrompt("Be sure to ALWAYS WRITE IN lyrical prose");

var response = await engine.CallAsync("What is the capital of France?");
Console.WriteLine(response);

//StreamAsync example
await foreach (var streamresponse in engine.StreamAsync("Write a highly detailed and verbose essay on belly button fluff"))
{
    Console.Write(streamresponse);
}
