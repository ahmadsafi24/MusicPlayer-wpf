using PlayerLibrary.Model;
using System.IO;

namespace PlayerLibrary.Preset
{
    public static class Equalizer
    {
        public static void PresetToFile(EqPreset preset, string filePath)
        {
            using var ms = new MemoryStream();
            using var writer = new System.Text.Json.Utf8JsonWriter(ms, new System.Text.Json.JsonWriterOptions() { Indented = true });

            writer.WriteStartObject();
            writer.WriteStartArray(nameof(EqPreset.BandsGain));
            foreach (var item in preset.BandsGain)
            {
                writer.WriteNumberValue(item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
            File.WriteAllBytes(filePath, ms.ToArray());
        }

        public static EqPreset PresetFromFile(string filePath)
        {
            EqPreset preset = null;
            if (File.Exists(filePath))
            {
                var json = File.ReadAllBytes(filePath);
                preset = System.Text.Json.JsonSerializer.Deserialize<EqPreset>(json);
            }
            return preset;
        }
    }
}