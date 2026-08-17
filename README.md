# Message.Encoder

**Message.Encoder** is a lightweight C# library for encoding message objects into a compact binary format. It is a proof-of-concept for custom binary serialization of messages with headers and payloads. The project focuses on efficient packing for transport or storage.

> **Note**  
> The encoder is conceptually similar to MessagePack in some areas, but it is an independent proof-of-concept without code generation.

## Project Structure

- **src/Message.Encoder** – Core library with encoder logic.
- **src/Message.Encoder.PoC** – Proof-of-concept console app showcasing basic usage.
- **src/Benchmarks/Message.Encoder.Benchmarks** – Benchmark project using BenchmarkDotNet.
- **src/Tests/Message.Encoder.MessageBuilder.Tests** – Unit tests for the message builder and binary format.
- **src/Tests/Message.Encoder.CustomMessages.Tests** – Unit tests for custom message classes and serializers.

## Features

- **Message Schema with Attributes** – Define message classes with `[SerializationOrder]` and `[MessageType]`.
- **Headers & Payload Model** – Separate header and payload serialization.
- **Fluent Builder API** – Staged builder that enforces the required basic message fields.
- **Custom Serialization Support** – Select serializers per type with `[UseSerializer]`.
- **Deterministic Binary Format** – Integer primitives are encoded explicitly as little-endian values.
- **Round-trip Support** – Registered message types can be serialized and deserialized.
- **Minimal Dependencies** – The core library uses only .NET runtime libraries.

## Getting Started

### Prerequisites

- [.NET SDK 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) or newer.

### Build the Solution

```bash
dotnet build Message.Encoder.sln
```

Or open `Message.Encoder.sln` in Visual Studio 2022+ and build.

### Run the PoC

```bash
dotnet run --project src/Message.Encoder.PoC/Message.Encoder.PoC.csproj
```

Or set **Message.Encoder.PoC** as your startup project in Visual Studio.

### Run the Tests

```bash
dotnet test Message.Encoder.sln
```

Or run them through Visual Studio's Test Explorer.

### Run Benchmarks

```bash
cd src/Benchmarks/Message.Encoder.Benchmarks
dotnet run -c Release
```

Always run benchmarks in **Release** mode for representative results.

## Usage Example

```csharp
using Message.Encoder.Builders;
using Message.Encoder.Messages.Default.Text;

var builder = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>.CreateBuilder();

var messageBytes = builder
    .From(1)
    .To(2)
    .Timestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
    .MsgType(1)
    .AddHeader("recipient-name", "Bob")
    .AddHeader("sender-name", "Alice")
    .AddHeader("is-message-unread", true)
    .EndHeaders()
    .AddPayloadProperty(nameof(DefaultTextMessagePayload.TextMessageBody), "Hello, world!")
    .GetBinary();
```

`messageBytes` contains the serialized message ready for transport or storage.

## Contributing

Contributions, ideas, and improvements are welcome. Feel free to fork the project, create a branch, and open a pull request.
