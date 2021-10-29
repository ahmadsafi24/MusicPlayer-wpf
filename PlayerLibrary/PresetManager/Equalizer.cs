using System.Net.Http;
using System.IO;

namespace PlayerLibrary.PresetManager
{
    public static class Equalizer
    {
        public static void LoadPreset(Player player,string presetFilePath)
        {
            try
            {
                if(!System.IO.File.Exists(presetFilePath))return;
                var json = File.ReadAllBytes(presetFilePath);
                Model.EqPreset eqPreset = System.Text.Json.JsonSerializer.Deserialize<Model.EqPreset>(json);
                if (eqPreset != null)
                {
                    ApplyPresetBands(player, eqPreset.BandsGain);
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public static void ExportPreset(Player player,string destinationFilePath)
        {
            Model.EqPreset EqPreset = new() { EqualizerMode = player.EqualizerMode, BandsGain = player.EqBandsGain };

            using var ms = new MemoryStream();
            using var writer = new System.Text.Json.Utf8JsonWriter(ms,new System.Text.Json.JsonWriterOptions(){Indented=true});

            writer.WriteStartObject();
            writer.WriteStartArray(nameof(EqPreset.BandsGain));
            foreach (var item in EqPreset.BandsGain)
            {
                writer.WriteNumberValue(item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
            File.WriteAllBytes(destinationFilePath, ms.ToArray());
        }
        public static void ApplyPresetBands(Player player, int[] presetBands)
        {
            if (presetBands.Length < 10)
            {
                player.EqualizerMode = PlayerLibrary.EqualizerMode.Normal;
            }
            else if (presetBands.Length > 10)
            {
                player.EqualizerMode = PlayerLibrary.EqualizerMode.Super;
            }
            player.ReIntialEq();
            int i = 0;
            foreach (var item in presetBands)
            {
                player.ChangeEq(i, (float)item,true);
                i++;
            }
        }
    }
}