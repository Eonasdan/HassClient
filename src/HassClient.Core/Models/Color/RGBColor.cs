namespace HassClient.Core.Models.Color;

/// <summary>
/// Represents an RGB (red, green, blue) color.
/// </summary>
public class RGBColor : Color
{
    /// <summary>
    /// Gets the red color component value.
    /// </summary>
    public byte R { get; internal set; }

    /// <summary>
    /// Gets the green color component value.
    /// </summary>
    public byte G { get; internal set; }

    /// <summary>
    /// Gets the blue color component value.
    /// </summary>
    public byte B { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RGBColor"/> class.
    /// </summary>
    /// <param name="red">The red color component value.</param>
    /// <param name="green">The green color component value.</param>
    /// <param name="blue">The blue color component value.</param>
    public RGBColor(byte red, byte green, byte blue)
    {
            R = red;
            G = green;
            B = blue;
        }

    /// <summary>
    /// Initializes a new instance of the <see cref="RGBColor"/> class.
    /// </summary>
    /// <param name="color">A <see cref="System.Drawing.Color"/> color.</param>
    public RGBColor(System.Drawing.Color color)
        : this(color.R, color.G, color.B)
    {
        }

    public static implicit operator RGBColor(System.Drawing.Color x) => new(x);

    /// <inheritdoc/>
    public override string ToString() => $"[{R}, {G}, {B}]";
}