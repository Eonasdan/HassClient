using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.Socket.Models.Color;

/// <summary>
/// Represents a color described by a known name.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Color names are self-documented")]
[PublicAPI]
public class NameColor : Color
{
    public static NameColor AliceBlue => new("aliceblue");

    public static NameColor AntiqueWhite => new("antiquewhite");

    public static NameColor Aqua => new("aqua");

    public static NameColor Aquamarine => new("aquamarine");

    public static NameColor Azure => new("azure");

    public static NameColor Beige => new("beige");

    public static NameColor Bisque => new("bisque");

    public static NameColor Black => new("black");

    public static NameColor BlanchedAlmond => new("blanchedalmond");

    public static NameColor Blue => new("blue");

    public static NameColor BlueViolet => new("blueviolet");

    public static NameColor Brown => new("brown");

    public static NameColor BurlyWood => new("burlywood");

    public static NameColor CadetBlue => new("cadetblue");

    public static NameColor Chartreuse => new("chartreuse");

    public static NameColor Chocolate => new("chocolate");

    public static NameColor Coral => new("coral");

    public static NameColor CornflowerBlue => new("cornflowerblue");

    public static NameColor Cornsilk => new("cornsilk");

    public static NameColor Crimson => new("crimson");

    public static NameColor Cyan => new("cyan");

    public static NameColor DarkBlue => new("darkblue");

    public static NameColor DarkCyan => new("darkcyan");

    public static NameColor DarkGoldenrod => new("darkgoldenrod");

    public static NameColor DarkGray => new("darkgray");

    public static NameColor DarkGreen => new("darkgreen");

    public static NameColor DarkGrey => new("darkgrey");

    public static NameColor DarkKhaki => new("darkkhaki");

    public static NameColor DarkMagenta => new("darkmagenta");

    public static NameColor DarkOliveGreen => new("darkolivegreen");

    public static NameColor DarkOrange => new("darkorange");

    public static NameColor DarkOrchid => new("darkorchid");

    public static NameColor DarkRed => new("darkred");

    public static NameColor DarkSalmon => new("darksalmon");

    public static NameColor DarkSeaGreen => new("darkseagreen");

    public static NameColor DarkSlateBlue => new("darkslateblue");

    public static NameColor DarkSlateGray => new("darkslategray");

    public static NameColor DarkSlateGrey => new("darkslategrey");

    public static NameColor DarkTurquoise => new("darkturquoise");

    public static NameColor DarkViolet => new("darkviolet");

    public static NameColor DeepPink => new("deeppink");

    public static NameColor DeepSkyBlue => new("deepskyblue");

    public static NameColor DimGray => new("dimgray");

    public static NameColor DimGrey => new("dimgrey");

    public static NameColor DodgerBlue => new("dodgerblue");

    public static NameColor Firebrick => new("firebrick");

    public static NameColor FloralWhite => new("floralwhite");

    public static NameColor ForestGreen => new("forestgreen");

    public static NameColor Fuchsia => new("fuchsia");

    public static NameColor Gainsboro => new("gainsboro");

    public static NameColor GhostWhite => new("ghostwhite");

    public static NameColor Gold => new("gold");

    public static NameColor Goldenrod => new("goldenrod");

    public static NameColor Gray => new("gray");

    public static NameColor Green => new("green");

    public static NameColor GreenYellow => new("greenyellow");

    public static NameColor Grey => new("grey");

    public static NameColor Honeydew => new("honeydew");

    public static NameColor HotPink => new("hotpink");

    public static NameColor IndianRed => new("indianred");

    public static NameColor Indigo => new("indigo");

    public static NameColor Ivory => new("ivory");

    public static NameColor Khaki => new("khaki");

    public static NameColor Lavender => new("lavender");

    public static NameColor LavenderBlush => new("lavenderblush");

    public static NameColor LawnGreen => new("lawngreen");

    public static NameColor LemonChiffon => new("lemonchiffon");

    public static NameColor LightBlue => new("lightblue");

    public static NameColor LightCoral => new("lightcoral");

    public static NameColor LightCyan => new("lightcyan");

    public static NameColor LightGoldenrodYellow => new("lightgoldenrodyellow");

    public static NameColor LightGray => new("lightgray");

    public static NameColor LightGreen => new("lightgreen");

    public static NameColor LightGrey => new("lightgrey");

    public static NameColor LightPink => new("lightpink");

    public static NameColor LightSalmon => new("lightsalmon");

    public static NameColor LightSeaGreen => new("lightseagreen");

    public static NameColor LightSkyBlue => new("lightskyblue");

    public static NameColor LightSlateGray => new("lightslategray");

    public static NameColor LightSlateGrey => new("lightslategrey");

    public static NameColor LightSteelBlue => new("lightsteelblue");

    public static NameColor LightYellow => new("lightyellow");

    public static NameColor Lime => new("lime");

    public static NameColor LimeGreen => new("limegreen");

    public static NameColor Linen => new("linen");

    public static NameColor Magenta => new("magenta");

    public static NameColor Maroon => new("maroon");

    public static NameColor MediumAquamarine => new("mediumaquamarine");

    public static NameColor MediumBlue => new("mediumblue");

    public static NameColor MediumOrchid => new("mediumorchid");

    public static NameColor MediumPurple => new("mediumpurple");

    public static NameColor MediumSeaGreen => new("mediumseagreen");

    public static NameColor MediumSlateBlue => new("mediumslateblue");

    public static NameColor MediumSpringGreen => new("mediumspringgreen");

    public static NameColor MediumTurquoise => new("mediumturquoise");

    public static NameColor MediumVioletRed => new("mediumvioletred");

    public static NameColor MidnightBlue => new("midnightblue");

    public static NameColor MintCream => new("mintcream");

    public static NameColor MistyRose => new("mistyrose");

    public static NameColor Moccasin => new("moccasin");

    public static NameColor NavajoWhite => new("navajowhite");

    public static NameColor Navy => new("navy");

    public static NameColor OldLace => new("oldlace");

    public static NameColor Olive => new("olive");

    public static NameColor OliveDrab => new("olivedrab");

    public static NameColor Orange => new("orange");

    public static NameColor OrangeRed => new("orangered");

    public static NameColor Orchid => new("orchid");

    public static NameColor PaleGoldenrod => new("palegoldenrod");

    public static NameColor PaleGreen => new("palegreen");

    public static NameColor PaleTurquoise => new("paleturquoise");

    public static NameColor PaleVioletRed => new("palevioletred");

    public static NameColor PapayaWhip => new("papayawhip");

    public static NameColor PeachPuff => new("peachpuff");

    public static NameColor Peru => new("peru");

    public static NameColor Pink => new("pink");

    public static NameColor Plum => new("plum");

    public static NameColor PowderBlue => new("powderblue");

    public static NameColor Purple => new("purple");

    public static NameColor Red => new("red");

    public static NameColor RosyBrown => new("rosybrown");

    public static NameColor RoyalBlue => new("royalblue");

    public static NameColor SaddleBrown => new("saddlebrown");

    public static NameColor Salmon => new("salmon");

    public static NameColor SandyBrown => new("sandybrown");

    public static NameColor SeaGreen => new("seagreen");

    public static NameColor SeaShell => new("seashell");

    public static NameColor Sienna => new("sienna");

    public static NameColor Silver => new("silver");

    public static NameColor SkyBlue => new("skyblue");

    public static NameColor SlateBlue => new("slateblue");

    public static NameColor SlateGray => new("slategray");

    public static NameColor SlateGrey => new("slategrey");

    public static NameColor Snow => new("snow");

    public static NameColor SpringGreen => new("springgreen");

    public static NameColor SteelBlue => new("steelblue");

    public static NameColor Tan => new("tan");

    public static NameColor Teal => new("teal");

    public static NameColor Thistle => new("thistle");

    public static NameColor Tomato => new("tomato");

    public static NameColor Turquoise => new("turquoise");

    public static NameColor Violet => new("violet");

    public static NameColor Wheat => new("wheat");

    public static NameColor White => new("white");

    public static NameColor WhiteSmoke => new("whitesmoke");

    public static NameColor Yellow => new("yellow");

    public static NameColor YellowGreen => new("yellowgreen");

    public static NameColor HomeAssistant => new("homeassistant");

    /// <summary>
    /// Gets a the color name.
    /// </summary>
    [JsonPropertyName("color_name")]
    public string? Name { get; internal set; }

    internal NameColor(string name)
    {
        Name = name;
    }

    /// <inheritdoc />
    public override string ToString() => Name;
}