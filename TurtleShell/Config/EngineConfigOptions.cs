using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TurtleShell.Config
{
    public enum EngineConfigSections
    {
        Json,
        SystemPrompt
    }

    public class EngineConfigOptions
    {
        private Dictionary<EngineConfigSections, EngineConfigSection> _sections = new Dictionary<EngineConfigSections, EngineConfigSection>();

        public EngineConfigOptions()
        {
            AddSection(JsonEngineConfigSection.UsePlainText);
            AddSection(SystemPromptConfigSection.Default);
            // Add other sections as needed
        }

        public void AddSection(EngineConfigSection section)
        {
            _sections[section.Section] = section;
        }

        public EngineConfigSection this[EngineConfigSections section]
        {
            get
            {
                return _sections[section];
            }
            set
            {
                AddSection(value);
            }
        }

        // Get section based on the type of TConfigSection
        public TConfigSection? GetSection<TConfigSection>() where TConfigSection : EngineConfigSection
        {
            return _sections.Values.OfType<TConfigSection>().FirstOrDefault();
        }
    }
}
