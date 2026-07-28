using System.Text.Json.Serialization;

namespace PrimeCheque.Models
{
    public class FieldConfig
    {
        [JsonPropertyName("x")]
        public float x { get; set; }

        [JsonPropertyName("y")]
        public float y { get; set; }

        [JsonPropertyName("width")]
        public float width { get; set; }

        [JsonPropertyName("height")]
        public float height { get; set; }

        [JsonPropertyName("fontSize")]
        public float fontSize { get; set; } = 11;

        [JsonPropertyName("angle")]
        public float angle { get; set; } = 0; // Degrees

        [JsonPropertyName("fontWeight")]
        public string fontWeight { get; set; } = "Bold"; // Bold, Normal

        [JsonPropertyName("letterSpacing")]
        public float letterSpacing { get; set; } = 0;
    }

    public class TemplateConfigDto
    {
        [JsonPropertyName("dateDay")]
        public FieldConfig? dateDay { get; set; }

        [JsonPropertyName("dateMonth")]
        public FieldConfig? dateMonth { get; set; }

        [JsonPropertyName("dateYear")]
        public FieldConfig? dateYear { get; set; }

        [JsonPropertyName("payeeLine1")]
        public FieldConfig? payeeLine1 { get; set; }

        [JsonPropertyName("payeeLine2")]
        public FieldConfig? payeeLine2 { get; set; }

        [JsonPropertyName("amountWordsLine1")]
        public FieldConfig? amountWordsLine1 { get; set; }

        [JsonPropertyName("amountWordsLine2")]
        public FieldConfig? amountWordsLine2 { get; set; }

        [JsonPropertyName("amountFigures")]
        public FieldConfig? amountFigures { get; set; }

        [JsonPropertyName("crossingZone")]
        public FieldConfig? crossingZone { get; set; }

        [JsonPropertyName("memoLine")]
        public FieldConfig? memoLine { get; set; }
    }
}
