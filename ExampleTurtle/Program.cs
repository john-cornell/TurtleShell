using Microsoft.Extensions.Configuration;
using TurtleShell;
using TurtleShell.Config;
using TurtleShell.Engines.Anthropic;
using TurtleShell.Engines.Ollama;
using TurtleShell.Engines.OpenAI;
using static OllamaSharp.OllamaApiClient;

EngineConfigOptions options = new EngineConfigOptions();
int INTERATIONS = 10;
//Options example
//options[EngineConfigSections.SystemPrompt] = new SystemPromptConfigSection { Prompt = "This is an example" };

options.GetSection<JsonEngineConfigSection>()!.ParseJson = false;
options.GetSection<JsonEngineConfigSection>()!.JsonFormat = false;

//options[EngineConfigSections.Json] = JsonEngineConfigSection.UseJsonFormat;

//var engineModelId = new EngineModelId(EngineType.OpenAI, OpenAIModelIds.GPT4o);
var engineModelId = new EngineModelId(EngineType.Ollama, OllamaModelIds.Phi3);
//var engineModelId = new EngineModelId(EngineType.Anthropic, AnthropicModelIds.Claude3_Haiku_20240307);

//options is optional

//Will initialize IConfiguration from appsettings.json, but IConfiguration can be passed directly as a named parameter
IEngine engine = EngineFactory.Start(engineModelId, options);
engine.SetSystemPrompt("Drill down deeply, be very verbose, ALWAYS extrapolate as deeply as you can, don't be scared of creativity");

//var response = await engine.CallAsync("What is the capital of France?");
//Console.WriteLine(response);

//StreamAsync example
//await foreach (var streamresponse in engine.StreamAsync("Write a highly detailed and verbose essay on belly button fluff"))
//{
//    Console.Write(streamresponse);
//}

string input = " { }";
List<string> output = new List<string>();
for (int i = 0; i < INTERATIONS; i++)
{
    var response = await engine.CallAsync($"Start or Complete this code as you wish: {input}");
    Console.WriteLine($"{i+1}: {response}");
    output.Add(response);
    input = response;
}

Console.WriteLine("Press any key to exit");
Console.ReadKey();