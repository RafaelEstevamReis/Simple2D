using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Simple2D.Core
{
    public class DrawEventArgs : EventArgs
    {
        public System.Windows.Forms.Form Form { get; set; }

        public System.Drawing.Graphics Graphics { get; set; }

        public void FillColor(System.Drawing.Color color)
        {
            //Form.BackColor = color;
            //Graphics.FillRectangle(new SolidBrush(color), 0, 0, Form.Width, Form.Height);
            Graphics.Clear(color);
        }
    }
}
