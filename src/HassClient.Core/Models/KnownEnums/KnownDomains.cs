using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace HassClient.Core.Models.KnownEnums
{
    /// <summary>
    /// Represents a list of known domains. Useful to reduce use of strings.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1602:Enumeration items should be documented",
        Justification = "Due to the nature of the list, it is not necessary to document each field.")]
    [PublicAPI]
    public enum KnownDomains
    {
        /// <summary>
        /// Used to represent a domain not defined within this enum.
        /// </summary>
        Undefined = 0,

        Adguard,
        AirQuality,
        AlarmControlPanel,
        Automation,
        BinarySensor,
        Button,
        Calendar,
        Camera,
        Cast,
        Climate,
        Cloud,
        Counter,
        Cover,
        DeviceTracker,
        Esphome,
        Fan,
        Filesize,
        Frontend,
        Generic,
        GenericThermostat,
        Group,
        Hassio,
        Homeassistant,
        Html5,
        Humidifier,
        ImageProcessing,
        InputBoolean,
        InputButton,
        InputDatetime,
        InputNumber,
        InputSelect,
        InputText,
        Light,
        Lock,
        Logbook,
        Logger,
        Mailbox,
        MediaPlayer,
        Mqtt,
        Notify,
        Number,
        PersistentNotification,
        Person,
        PythonScript,
        Recorder,
        Remote,
        Scene,
        Script,
        Select,
        Sensor,
        Siren,
        Speedtestdotnet,
        Stream,
        Sun,
        Switch,
        SystemLog,
        Template,
        Timer,
        Tts,
        Vacuum,
        WakeOnLan,
        WaterHeater,
        Weather,
        WebosTv,
        XiaomiMiio,
        Zha,
        Zone,
    }
}
