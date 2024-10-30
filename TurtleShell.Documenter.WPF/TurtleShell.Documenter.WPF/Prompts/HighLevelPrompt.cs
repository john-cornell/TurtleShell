using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurtleShell.Prompts;

namespace TurtleShell.Documenter.WPF.Prompts
{
    internal class HighLevelPrompt : Prompt
    {
        public override string PromptTemplate =>
            @"<TASK>
    <TITLE>High-Level Overview</TITLE>
    <OUTPUT_FORMAT>
        <RULE>You MUST answer in Git Flavored Markdown, parsable by MarkdownSharp</RULE>
        <RULE>You MUST answer as Application Documentation</RULE>
        
</OUTPUT_FORMAT>
    <TASK>Look at the project in the CONTEXT BLOCK and provide comprehensive, high-level documentation for the project.
    You must
    * Document how to use the project in an easy-to-understand manner
        * You MUST include code snippets
    * Describe the purpose of the project
    * Describe the project's architecture
    * Describe the project's components
    * DO NOT provide any irrelevant or unnecessary information
    * BE PRECISE and USEFUL
    * Explain options where available, such as configuration options and OPTIONAL features
        
    </TASK>
PLEASE respond in a friendly, technical, comprehensive and expository manner. Remember, you are writing documentation for a project.
DO NOT be overly cheesy or overly formal. Be professional, but also be friendly and approachable.
</OUTPUT_FORMAT>
    <CONTEXT>
        <BLOCK>
            <TITLE>Project</TITLE>
            <CONTENT>
                {files}
            </CONTENT>
        </BLOCK>
</TASK>";
    }
}
