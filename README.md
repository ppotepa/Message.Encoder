# Message.Encoder

**Message.Encoder** is a lightweight C# library for encoding message objects into a compact binary format. It is a proof-of-concept for custom binary serialization of messages with headers and payloads. The project focuses on efficient packing for transport or storage.

> **Note**  
> The encoder is conceptually similar to MessagePack in some areas, but it is an independent proof-of-concept without code generation.

## Status

Proof of concept. The library, console app, tests, and benchmarks currently target `net7.0`, which [reached end of support in May 2024](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core). The repository includes tests but no automated CI workflow. Treat the wire format and API as experimental.

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

- A .NET SDK with `net7.0` targeting support. Running the existing applications and tests without retargeting requires the .NET 7 runtime.

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

The PoC console app contains serialization/round-trip timing experiments, not a messaging service.

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

Run benchmarks in **Release** mode. The BenchmarkDotNet project compares integer and floating-point conversion routines; it does **not** measure end-to-end message serialization throughput.

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
