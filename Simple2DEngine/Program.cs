using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple2D
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Core.Engine engine = new Core.Engine();
            engine.FPSLimit = 35;
            engine.Setup += (object sender, Form e) =>
            {

            };

            int i = 0;
            string updateText = "";
            engine.Draw += (object sender, Core.DrawEventArgs e) =>
            {
                i = (i + 1) % 256;
                //e.Form.BackColor = ;
                e.FillColor(Color.FromArgb(i, i, i));

                var rect = engine.GetTileSize(i % 30, Math.Abs(30 - (i % 60)), 30, 30);
                e.Graphics.FillRectangle(Brushes.Blue, rect);

                e.Graphics.DrawString("Draw " + engine.LastDrawTime.ToString() + " FPS " + engine.GetFPS(engine.LastDrawTime),
                                      e.Form.Font,
                                      Brushes.Red, 0, 0);

                e.Graphics.DrawString(updateText,
                                      e.Form.Font,
                                      Brushes.Blue, 0, 15);
            };
            engine.Update += (object sender, Core.UpdateEventArgs e) =>
            {
                updateText = "Upd: " + e.LastUpdateTimer + "FPS " + engine.GetFPS(e.LastUpdateTimer);
            };

            engine.Show(new Core.WindowInfo()
            {
                FullScreen = true
            });
        }
    }
}
