using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell.Engines.Ollama
{
    public static class OllamaModelIds
    {
        public const string Phi3 = "phi3:latest";
        public const string Phi3Medium = "phi3:medium-128k";
        public const string Llama2_U_7b = "llama2-uncensored:7b";
        public const string Llama3_8b = "llama3:8b";
        public const string Llama3_1_7b = "llama3.1:latest";
        public const string OrcaMini = "orca-mini:latest";
        public const string Mixstral = "mixtral:8x7b";
        public const string Llama3_2_7b = "llama3.2:latest";
        public const string HermesFinalEnd = "finalend/hermes-3-llama-3.1:latest";
        public const string NousHermes7b = "nous-hermes:latest";
        public const string NousHermes13b = "nous-hermes:13b";
        public const string NemotronMini = "nemotron-mini";
        public const string Dolphin = "dolphin-mistral:latest";
        public const string Summarizer = "ALIENTELLIGENCE/contentsummarizer";
        public const string Gemma2_2b = "gemma2:2b";
        public const string Gemma2_9b = "gemma2";
        public const string Default = Gemma2_9b;
    }
}
