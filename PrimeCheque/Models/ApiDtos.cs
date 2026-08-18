using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PrimeCheque.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }

    public class LicenceStatusDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("edition")]
        public string Edition { get; set; } = string.Empty;

        [JsonPropertyName("expires_at")]
        public DateTime? ExpiresAt { get; set; }
    }

    public class BankTemplateDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("bank_name")]
        public string BankName { get; set; } = string.Empty;
        
        [JsonPropertyName("series_name")]
        public string SeriesName { get; set; } = string.Empty;

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("dimensions")]
        public DimensionsDto? Dimensions { get; set; }
        
        [JsonPropertyName("template_config")]
        public string TemplateConfig { get; set; } = string.Empty;
        
        [JsonPropertyName("template_image_path")]
        public string? TemplateImagePath { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class DimensionsDto
    {
        [JsonPropertyName("width")]
        public decimal Width { get; set; }

        [JsonPropertyName("height")]
        public decimal Height { get; set; }
    }

    public class BackupResponseDto
    {
        [JsonPropertyName("backup_id")]
        public string BackupId { get; set; } = string.Empty;

        [JsonPropertyName("size_bytes")]
        public long SizeBytes { get; set; }

        [JsonPropertyName("uploaded_at")]
        public DateTime UploadedAt { get; set; }
    }
}
