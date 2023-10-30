using System.Text.Json.Serialization;
using Zaphyros.Core.Users;

namespace Zaphyros.Core.Configuration
{
    [JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyFields = true, GenerationMode = JsonSourceGenerationMode.Serialization)]
    [JsonSerializable(typeof(User))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
