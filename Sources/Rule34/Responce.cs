namespace rever
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Rule34Responce
    {
        [JsonProperty("preview_url")]
        public Uri PreviewUrl { get; set; }

        [JsonProperty("sample_url")]
        public Uri SampleUrl { get; set; }

        [JsonProperty("file_url")]
        public Uri FileUrl { get; set; }

        [JsonProperty("directory")]
        public long Directory { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("change")]
        public long Change { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("parent_id")]
        public long ParentId { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("sample")]
        public bool Sample { get; set; }

        [JsonProperty("sample_height")]
        public long SampleHeight { get; set; }

        [JsonProperty("sample_width")]
        public long SampleWidth { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("source")]
        public Uri Source { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("has_notes")]
        public bool HasNotes { get; set; }

        [JsonProperty("comment_count")]
        public long CommentCount { get; set; }
    }

    public partial class Responce
    {
        public static Rule34Responce[] FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Rule34Responce[]>(json, rever.Converter.Settings);
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
