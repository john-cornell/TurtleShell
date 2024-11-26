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

//options is optional
//options[EngineConfigSections.Json] = JsonEngineConfigSection.UseJsonFormat;

var engineModelId = new EngineModelId(EngineType.GoogleGemini, GoogleGeminiModelIds.Flash1_5_8B);

//Will initialize IConfiguration from appsettings.json, but IConfiguration can be passed directly as a named parameter
IEngine engine = EngineFactory.Start(engineModelId, options);
engine.SetSystemPrompt("Be sure to ALWAYS WRITE IN lyrical prose");

//Console.WriteLine("ENTER FOR LLM TEST");
//Console.ReadLine();
//var response = await engine.CallAsync("Talk about an obscure European town or village");

//Console.WriteLine(response);

Console.WriteLine("ENTER FOR STREAM TEST");
Console.ReadLine();
//StreamAsync example
await foreach (var streamresponse in engine.StreamAsync("Write a highly detailed and verbose essay on belly button fluff"))
{
    Console.Write(streamresponse);
}