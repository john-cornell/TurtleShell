using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Config;

namespace TurtleShell
{
    public interface IEngine
    {
        void SetSystemPrompt(string systemPrompt);
        Task<string> CallAsync(string prompt, bool resetHistory = false, params EngineConfigSection[] engineConfigSections);
        EngineModelId EngineModelId { get; }

        IAsyncEnumerable<string> StreamAsync(string prompt, bool resetHistory = false, params EngineConfigSection[] engineConfigSections);
    }
}
