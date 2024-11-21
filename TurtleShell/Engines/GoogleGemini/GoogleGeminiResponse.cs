using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TurtleShell.Engines.GoogleGemini
{
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }

        [JsonPropertyName("usageMetadata")]
        public UsageMetadata UsageMetadata { get; set; }

        [JsonPropertyName("modelVersion")]
        public string ModelVersion { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("safetyRatings")]
        public List<SafetyRating> SafetyRatings { get; set; }

        [JsonPropertyName("citationMetadata")]
        public CitationMetadata CitationMetadata { get; set; }

        [JsonPropertyName("avgLogprobs")]
        public double AvgLogprobs { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class SafetyRating
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("probability")]
        public string Probability { get; set; }
    }

    public class CitationMetadata
    {
        [JsonPropertyName("citationSources")]
        public List<CitationSource> CitationSources { get; set; }
    }

    public class CitationSource
    {
        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; }

        [JsonPropertyName("endIndex")]
        public int EndIndex { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }

    public class UsageMetadata
    {
        [JsonPropertyName("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        [JsonPropertyName("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        [JsonPropertyName("totalTokenCount")]
        public int TotalTokenCount { get; set; }
    }
}