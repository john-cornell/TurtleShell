using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell
{
    public class EngineModelId
    {
        public EngineType EngineType { get; }
        public string ModelId { get; }

        public EngineModelId(EngineType engineType, string modelId)
        {
            EngineType = engineType;
            ModelId = modelId;
        }
    }
}
