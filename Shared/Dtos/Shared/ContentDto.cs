using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedModels.Dtos.Shared
{
 namespace SharedModels.Dtos.Shared
{
    public class ContentPage
    {
        [JsonPropertyName("page")]
        public string Page { get; set; }

        [JsonPropertyName("content")]
        public List<ContentItem> Content { get; set; } = new List<ContentItem>();
    }

    public class ContentItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
}
