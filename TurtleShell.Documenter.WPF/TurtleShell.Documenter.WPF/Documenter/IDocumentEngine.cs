using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell.Documenter.WPF.Documenter
{
    public interface IDocumentEngine
    {
        Task<string> GenerateHighLevelOverview(string projectPath);
        string GenerateDetailedDocumentation(string filePath);
    }
}
