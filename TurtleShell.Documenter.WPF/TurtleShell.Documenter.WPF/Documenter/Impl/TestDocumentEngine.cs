using TurtleShell.Documenter.WPF.Documenter;

namespace TurtleShell.Documenter.WPF.Tests
{
    public class TestDocumentEngine : IDocumentEngine
    {
        public Task<string> GenerateHighLevelOverview(string projectPath)
        {
            return Task.FromResult("Test High-Level Overview");
        }

        public string GenerateDetailedDocumentation(string filePath)
        {
            return Resources.HelloWorldDocumentation;
        }
    }
}
