using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleShell.Engines.AzureOpenAI
{
    using System.Text.Json.Serialization;

    public class OpenAIResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("object")]
        public string ObjectType { get; set; }

        [JsonPropertyName("prompt_filter_results")]
        public List<PromptFilterResult> PromptFilterResults { get; set; }

        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("content_filter_results")]
        public ContentFilterResults ContentFilterResults { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("logprobs")]
        public object Logprobs { get; set; } // Can be null

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    public class ContentFilterResults
    {
        [JsonPropertyName("hate")]
        public FilterResult Hate { get; set; }

        [JsonPropertyName("protected_material_code")]
        public ProtectedMaterial ProtectedMaterialCode { get; set; }

        [JsonPropertyName("protected_material_text")]
        public ProtectedMaterial ProtectedMaterialText { get; set; }

        [JsonPropertyName("self_harm")]
        public FilterResult SelfHarm { get; set; }

        [JsonPropertyName("sexual")]
        public FilterResult Sexual { get; set; }

        [JsonPropertyName("violence")]
        public FilterResult Violence { get; set; }
    }

    public class FilterResult
    {
        [JsonPropertyName("filtered")]
        public bool Filtered { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }
    }

    public class ProtectedMaterial
    {
        [JsonPropertyName("filtered")]
        public bool Filtered { get; set; }

        [JsonPropertyName("detected")]
        public bool Detected { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("refusal")]
        public object Refusal { get; set; } // Can be null

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class PromptFilterResult
    {
        [JsonPropertyName("prompt_index")]
        public int PromptIndex { get; set; }

        [JsonPropertyName("content_filter_results")]
        public PromptContentFilterResults ContentFilterResults { get; set; }
    }

    public class PromptContentFilterResults
    {
        [JsonPropertyName("hate")]
        public FilterResult Hate { get; set; }

        [JsonPropertyName("jailbreak")]
        public ProtectedMaterial Jailbreak { get; set; }

        [JsonPropertyName("self_harm")]
        public FilterResult SelfHarm { get; set; }

        [JsonPropertyName("sexual")]
        public FilterResult Sexual { get; set; }

        [JsonPropertyName("violence")]
        public FilterResult Violence { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("completion_tokens_details")]
        public CompletionTokensDetails CompletionTokensDetails { get; set; }

        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("prompt_tokens_details")]
        public PromptTokensDetails PromptTokensDetails { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class CompletionTokensDetails
    {
        [JsonPropertyName("accepted_prediction_tokens")]
        public int AcceptedPredictionTokens { get; set; }

        [JsonPropertyName("audio_tokens")]
        public int AudioTokens { get; set; }

        [JsonPropertyName("reasoning_tokens")]
        public int ReasoningTokens { get; set; }

        [JsonPropertyName("rejected_prediction_tokens")]
        public int RejectedPredictionTokens { get; set; }
    }

    public class PromptTokensDetails
    {
        [JsonPropertyName("audio_tokens")]
        public int AudioTokens { get; set; }

        [JsonPropertyName("cached_tokens")]
        public int CachedTokens { get; set; }
    }
}
