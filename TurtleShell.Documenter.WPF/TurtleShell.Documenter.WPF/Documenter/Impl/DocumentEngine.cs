using System.IO;
using System.Text;
using MarkdownSharp;
using Microsoft.Extensions.Configuration;
using TurtleShell.Config;
using TurtleShell.Documenter.WPF.Prompts;
using TurtleShell.Engines.OpenAI;
using TurtleShell.Prompts;

namespace TurtleShell.Documenter.WPF.Documenter
{
    public class DocumentEngine : IDocumentEngine
    {
        private Dictionary<string, string>? _files;

        public string GenerateDetailedDocumentation(string projectPath)
        {
            if (_files == null)
            {
                EnsureFilesLoaded(projectPath);
            }

            // Logic to generate high-level overview
            return "High-Level Overview";
        }

        public async Task<string> GenerateHighLevelOverview(string filePath)
        {
            EnsureFilesLoaded(filePath);

            var engineModelId = new EngineModelId(EngineType.OpenAI, OpenAIModelIds.GPT4o);
            EngineConfigOptions options = new EngineConfigOptions();
            options[EngineConfigSections.Json] = JsonEngineConfigSection.UsePlainText;
            IEngine engine = EngineFactory.Start(engineModelId, options);
            engine.SetSystemPrompt("<TASK>You are a documentation writer for C# Applications. You always follow the tasks given you with utter skill and competence</TASK>");

            return await engine.CallAsync(GetHighLevelPrompt(), true);
        }

        private string GetHighLevelPrompt()
        {
            if (_files == null)
            {
                throw new InvalidOperationException("Files have not been loaded. EnsureFilesLoaded must be called before generating the prompt.");
            }

            StringBuilder builder = new StringBuilder();

            foreach (var file in _files)
            {
                builder.AppendLine($"## {file.Key} ##{file.Value}");
            }

            HighLevelPrompt prompt = new HighLevelPrompt();
            return prompt.Transform(("files", builder.ToString()));
        }

        private void EnsureFilesLoaded(string filePath)
        {
            if (filePath != null)
            {
                _files = Directory.GetFiles(filePath, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.EndsWith(".cs") || file.EndsWith(".csproj"))
                    .ToDictionary(file => Path.GetFileName(file), file => File.ReadAllText(file));
            }
        }
    }
}
