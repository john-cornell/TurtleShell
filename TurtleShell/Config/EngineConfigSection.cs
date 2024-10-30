using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell.Config
{
    public abstract class EngineConfigSection
    {
        public abstract EngineConfigSections Section { get; }
    }
}
