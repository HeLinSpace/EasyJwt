﻿using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Easy.Jwt.Core;

public class JwtResponse
{
    /// <summary>
    /// token
    /// </summary>
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    /// <summary>
    /// 有效期
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// token类型
    /// </summary>
    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; }

    /// <summary>
    /// error
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object> Custom { get; set; }

    private static JsonSerializerOptions _default = new()
    {
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task ExecuteAsync(HttpContext context)
    {
        var json = JsonSerializer.Serialize(this, _default);
        await context.Response.WriteJsonAsync(json);
    }
}
