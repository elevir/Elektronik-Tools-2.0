﻿using Elektronik.Data;
using Elektronik.Data.Converters;
using JetBrains.Annotations;

namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for plugins implements new data sources. </summary>
    public interface IDataSourcePlugin : IElektronikPlugin
    {
        /// <summary> Converter for raw Vector3 and Quaternions </summary>
        [CanBeNull] ICSConverter Converter { get; set; }

        /// <summary> Containers with cloud data. </summary>
        [NotNull] ISourceTree Data { get; }
    }
}