﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public interface ISlamEvent
    {
        SlamEventType EventType { get; }
        int Timestamp { get; }
        bool IsKeyEvent { get; }
        SlamObservation[] Observations { get; }
        SlamPoint[] Points { get; }

    }
}
