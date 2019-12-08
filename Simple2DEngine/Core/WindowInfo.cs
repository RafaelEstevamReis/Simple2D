using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple2D.Core
{
    public class WindowInfo
    {
        public bool FullScreen { get; set; }

        public System.Drawing.Size Size { get; set; }

        public void Validate()
        {
            if (FullScreen)
            {
                if (Size.Height != 0 || Size.Width != 0)
                    throw new InvalidOperationException("Size must be ZERO in FullScreen monde");
            }
        }
    }
}
