﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple2D.Core
{
    public class UpdateEventArgs : EventArgs
    {
        public TimeSpan LastUpdateTimer { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int FrameCount { get; set; }
    }
}