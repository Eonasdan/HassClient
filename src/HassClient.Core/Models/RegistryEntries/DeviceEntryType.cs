﻿using JetBrains.Annotations;

namespace HassClient.Core.Models.RegistryEntries
{
    /// <summary>
    /// Defines the device entry type possible values.
    /// </summary>
    [PublicAPI]
    public enum DeviceEntryTypes
    {
        /// <summary>
        /// Device has not defined type.
        /// </summary>
        None,

        /// <summary>
        /// Device is a service entry type.
        /// </summary>
        Service,
    }
}
