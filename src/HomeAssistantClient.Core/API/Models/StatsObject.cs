using System.Text.Json.Serialization;

namespace HomeAssistantClient.Core.API.Models;

/// <summary>
/// Represents the stats object.
/// </summary>
public class StatsObject
{
    /// <summary>
    /// Gets or sets the number of bulk reads.
    /// </summary>
    [JsonPropertyName("blk_read")]
    public long BlkRead { get; set; }

    /// <summary>
    /// Gets or sets the number of bulk writes.
    /// </summary>
    [JsonPropertyName("blk_write")]
    public long BlkWrite { get; set; }

    /// <summary>
    /// Gets or sets the CPU usage percent.
    /// </summary>
    [JsonPropertyName("cpu_percent")]
    public double CpuPercent { get; set; }

    /// <summary>
    /// Gets or sets the memory limit.
    /// </summary>
    [JsonPropertyName("memory_limit")]
    public long MemoryLimit { get; set; }

    /// <summary>
    /// Gets or sets the memory usage percent.
    /// </summary>
    [JsonPropertyName("memory_percent")]
    public double MemoryPercent { get; set; }

    /// <summary>
    /// Gets or sets the memory usage.
    /// </summary>
    [JsonPropertyName("memory_usage")]
    public long MemoryUsage { get; set; }

    /// <summary>
    /// Gets or sets the network rx.
    /// </summary>
    [JsonPropertyName("network_rx")]
    public long NetworkRx { get; set; }

    /// <summary>
    /// Gets or sets the network tx.
    /// </summary>
    [JsonPropertyName("network_tx")]
    public long NetworkTx { get; set; }
}