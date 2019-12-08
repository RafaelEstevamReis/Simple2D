using Simple2D.Core;
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
        static Core.Engine engine;
        static Tileset player;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            player = new Tileset(@"Assets\Character.png", 4, 4);

            engine = new Core.Engine();
            engine.FPSLimit = 30;
            engine.Setup += (object sender, Form e) =>
            {
                e.KeyPress += e_KeyPress;
                e.KeyDown += e_KeyDown;
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

                player.DrawCurrentTile(e.Graphics, new Point(100, 100));
            };
            engine.Update += (object sender, Core.UpdateEventArgs e) =>
            {
                updateText = "Upd: " + e.LastUpdateTimer + "FPS " + engine.GetFPS(e.LastUpdateTimer);
                // Should step based on time, not in FrameCount
                if (e.FrameCount % 5 == 0)
                {
                    player.StepCurrTileY();
                }
            };

            engine.Show(new Core.WindowInfo()
            {
                FullScreen = true,
                //Size = new Size(480, 320)
            });
        }

        static void e_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    player.SetCurrTileColumn(1);
                    break;
                case Keys.Down:
                    player.SetCurrTileColumn(0);
                    break;
                case Keys.Left:
                    player.SetCurrTileColumn(3);
                    break;
                case Keys.Right:
                    player.SetCurrTileColumn(2);
                    break;
            }
        }
        static void e_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
    }
}
