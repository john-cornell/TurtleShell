# TurtleShell Documentation

## Introduction

TurtleShell provides a versatile framework for engaging with different AI language processing engines. Designed with flexibility and scalability in mind, it allows users to seamlessly switch between different AI platforms, such as Ollama and OpenAI, leveraging unique features from each engine.

## Purpose

The primary purpose of TurtleShell is to offer a unified interface for running AI models from different platforms. This allows developers to integrate powerful AI functionalities into their applications with minimal setup and effort. The project is particularly useful for applications that require conversational AI capabilities, such as chatbots or virtual assistants.

## Architecture Overview

TurtleShell's architecture is centered around the `EngineFactory` class, which serves as the entry point for starting and configuring instances of AI engines. The architecture can be broken down into several key components:

- **Engine Factory**: This serves as a centralized access point to create and configure engine instances.
- **Engines**: Abstract implementations and specific implementations like Ollama and OpenAI engines are provided.
- **Configuration**: Built-in support for configuration using `.NET`'s configuration framework.
- **Text Processing**: Includes utilities like JSON extraction and prompt formatting for enhanced response processing.

## Key Components

### EngineFactory

- **Purpose**: `EngineFactory` initializes AI engines by determining the appropriate type and configuration settings based on the given input parameters.
- **Usage**:

```csharp
var engineModelId = new EngineModelId(EngineType.OpenAI, OpenAIModelIds.GPT3_5_Turbo);
IEngine engine = EngineFactory.Start(engineModelId, options: new EngineConfigOptions());
```

### EngineModelId

- **Purpose**: Represents the identity and type of a model being used in an engine.
- **Details**: Contains `EngineType` and `ModelId`, helping to specify which AI engine and model to use.

### Engine Types

- **Ollama**: Uses different models like `phi3`, `llama3`, etc., accessed via the `OllamaEngine`.
- **OpenAI**: Has support for multiple GPT models like GPT-3.5 Turbo and GPT-4, implemented in the `OpenAIGPTEngine`.

### IEngine Interface

- **Purpose**: Defines a common interface for all engine implementations with functionalities to set prompts, make asynchronous calls, and get engine details.
- **Example Usage**:
  
```csharp
engine.SetSystemPrompt("Hello, AI!");
string response = await engine.CallAsync("Tell me a joke.", resetHistory: true);
```

### Configuration Options

- **EngineConfigOptions**: Provides configurable sections for aspects such as JSON parsing or system prompts settings.
  - **JSON Parsing**:
    - `UseJsonParse`: Enables parsing
    - `UsePlainText`: Disables JSON formatting
    - `UseJsonFormat`: Enables JSON formatting without parsing
  - **System Prompt**:
    - Configures initial system prompts for the AI to follow.

### Text Processing

- **JsonExtractor**: This utility processes JSON responses from models, ensuring correct formatting and removal of unnecessary content.

## Advanced Usage

### Switching Models

To switch between different models, simply change the `EngineModelId` as follows:

```csharp
var engineModelId = new EngineModelId(EngineType.Ollama, OllamaModelIds.Llama3_8b);
// Switch to OpenAI
engineModelId = new EngineModelId(EngineType.OpenAI, OpenAIModelIds.GPT4);
```

### Adding Custom Configurations

You can customize engine behavior by modifying `EngineConfigOptions`:

```csharp
var customOptions = new EngineConfigOptions();
customOptions.AddSection(new JsonEngineConfigSection { ParseJson = true });
// Add additional custom configuration
```

## Conclusion

TurtleShell provides a robust solution for integrating various AI capabilities into your applications. By facilitating easy swaps between different AI engines and models, it standardizes how you can deploy conversational AI features across diverse use cases. With its straightforward design and powerful configuration options, TurtleShell will elevate the conversational experiences of your applications.
