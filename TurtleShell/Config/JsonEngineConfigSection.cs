using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell.Config
{
    public class JsonEngineConfigSection : EngineConfigSection
    {
        public override EngineConfigSections Section => EngineConfigSections.Json;
    
        public bool ParseJson { get; set; }
        public bool JsonFormat { get; set; }

        /// <summary>
        /// Formats output as JSON and parses JSON response, removing any gumph not needed and adds final } if missing
        /// </summary>
        public static JsonEngineConfigSection UseJsonParse => new JsonEngineConfigSection
        {
            ParseJson = true,
            JsonFormat = true
        };

        /// <summary>
        /// Does not format output as JSON, does not parse JSON response
        /// </summary>
        public static JsonEngineConfigSection UsePlainText => new JsonEngineConfigSection
        {
            ParseJson = false,
            JsonFormat = false
        };

        /// <summary>
        /// Only formats output as JSON, does not parse JSON response
        /// </summary>
        public static JsonEngineConfigSection UseJsonFormat => new JsonEngineConfigSection
        {
            ParseJson = false,
            JsonFormat = true
        };
    }
}
