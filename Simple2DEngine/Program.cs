﻿using Simple2D.Core;
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
        static PointF PlayerPos;

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
                PlayerPos = new Point(100, 100);
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

                e.Graphics.DrawString("Draw " + engine.LastDrawTime.ToString() + " FPS " + engine.RealDrawFPS,
                                      e.Form.Font,
                                      Brushes.Red, 0, 0);

                e.Graphics.DrawString(updateText,
                                      e.Form.Font,
                                      Brushes.Blue, 0, 15);

                player.DrawCurrentTile(e.Graphics, PlayerPos);
            };
            engine.Update += (object sender, Core.UpdateEventArgs e) =>
            {
                updateText = "Upd: " + e.LastUpdateTimer + "FPS " + engine.RealUpdateFPS;
                // Should step based on time, not in FrameCount
                if (e.FrameCount % 5 == 0)
                {
                    player.StepCurrTileY();
                }

                float speed = e.LastUpdateTimer.Milliseconds / 30f;

                if (engine[Keys.Up])
                {
                    player.SetCurrTileColumn(1);
                    PlayerPos.Y -= speed;
                }
                if (engine[Keys.Down])
                {
                    player.SetCurrTileColumn(0);
                    PlayerPos.Y += speed;
                }
                if (engine[Keys.Right])
                {
                    player.SetCurrTileColumn(2);
                    PlayerPos.X += speed;
                }
                if (engine[Keys.Left])
                {
                    player.SetCurrTileColumn(3);
                    PlayerPos.X -= speed;
                }

            };

            engine.Show(new Core.WindowInfo()
            {
                FullScreen = false,
                Size = new Size(480, 320)
            });
        }

        static void e_KeyDown(object sender, KeyEventArgs e)
        {
        }
        static void e_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
    }
}
