using System;

namespace TurtleShell.Config
{
    public class SystemPromptConfigSection : EngineConfigSection
    {
        public override EngineConfigSections Section => EngineConfigSections.SystemPrompt;

        public string Prompt { get; set; }

        public static SystemPromptConfigSection Default => new SystemPromptConfigSection
        {
            Prompt = @"You are an intelligent, reasoning AI engine.
<RULES>
    <RULE>Be Helpful</RULE>
    <RULE>Answer as accurately as possible</RULE>
    <RULE>Be respectful</RULE>
    <RULE>Think through your responses before answering</RULE>
</RULES>"
        };
    }
}
