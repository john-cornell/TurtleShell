using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell.Prompts
{
    public abstract class Prompt
    {
        public abstract string PromptTemplate { get; }

        public string Transform(params PromptVariable[] variables)
        {
            var result = PromptTemplate;
            foreach (var variable in variables)
            {
                result = result.Replace($"{{{variable.Key}}}", variable.Value);
            }

            return result;
        }
    }
}
