using TurtleShell;
using TurtleShell.Config;
using TurtleShell.Engines.GoogleGemini;
using TurtleShell.Engines.AnthropicClaude;
using TurtleShell.Engines.AzureOpenAI;
using TurtleShell.Engines.OpenAI;

int ITERATIONS = 10;

EngineConfigOptions options = new EngineConfigOptions();

//Options example
//options[EngineConfigSections.SystemPrompt] = new SystemPromptConfigSection { Prompt = "This is an example" };

options.GetSection<JsonEngineConfigSection>()!.ParseJson = false;
options.GetSection<JsonEngineConfigSection>()!.JsonFormat = false;

//options[EngineConfigSections.Json] = JsonEngineConfigSection.UseJsonFormat;

//var engineModelId = new EngineModelId(EngineType.OpenAI, OpenAIModelIds.GPT4o);
//var engineModelId = new EngineModelId(EngineType.Ollama, OllamaModelIds.Phi3);
//var engineModelId = new EngineModelId(EngineType.Anthropic, AnthropicModelIds.Claude3_Haiku_20240307);
var engineModelId = new EngineModelId(EngineType.GoogleGemini, GoogleGeminiModelIds.Flash1_5_8B);
//options is optional

//Will initialize IConfiguration from appsettings.json, but IConfiguration can be passed directly as a named parameter
IEngine engine = EngineFactory.Start(engineModelId, options);
engine.SetSystemPrompt("Be sure to ALWAYS WRITE IN lyrical prose");
//engine.SetSystemPrompt("Be precise, creative and technically correct. Do not be obvious but ensure to write compilable code. ALWAYS Scratchpad first to consider what to do, then ALWAYS reflect on that before coding");
Console.WriteLine("LLM TEST");
//Console.WriteLine(response);

Console.WriteLine("STREAM TEST");
//StreamAsync example
await foreach (var streamresponse in engine.StreamAsync("Write a highly detailed and verbose essay on belly button fluff"))
{
    Console.Write(streamresponse);
}

//string input = " { using System; }";
//List<string> output = new List<string>();
//for (int i = 0; i < ITERATIONS; i++)
//{
//    var response = await engine.CallAsync($"Start or Complete this C# code as you wish: {input}");
//    Console.WriteLine($"{i+1}: {response}");
//    output.Add(response);
//    input = response;
//}


//Test conversation history
//var response = await engine.CallAsync("Start counting from 10 in 10s");
//Console.WriteLine(response);
//for(int i = 0; i < 10; i++)
//{
//    response = await engine.CallAsync("What is the next number in the sequence you just gave?");
//    Console.WriteLine(response);
//}

//Console.ReadLine();
