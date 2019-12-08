using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple2D.Core
{
    public sealed class Engine
    {
        public event EventHandler<Form> Setup;
        public event EventHandler<UpdateEventArgs> Update;
        public event EventHandler<DrawEventArgs> Draw;

        public Engine()
        {
            FPSLimit = 30;
            stopThreads = false;
        }

        public int FPSLimit { get; set; }
        public TimeSpan LastDrawTime { get; private set; }
        public TimeSpan LastUpdateTime { get; private set; }
        public int FPSFrameTime
        {
            get
            {
                return 1000 / FPSLimit;
            }
        }

        Thread thdUpdate;
        Thread thdDraw;

        myForm form = null;
        Stopwatch swTotal = new Stopwatch();
        private class myForm : Form
        {
            public myForm()
                : base()
            {
                
            }
        }
        public void Show(WindowInfo Info)
        {
            //Info.FullScreen = true;
            //Info.Size = new Size(10, 10);

            Info.Validate();

            form = new myForm();
            form.BackColor = Color.Azure;
            form.AutoValidate = AutoValidate.Disable;
            form.KeyPreview = true;
            //form.Init(Info);
            //form.Show();

            if (Info.FullScreen)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
                form.MaximizeBox = false;
                form.Size = Info.Size;
            }

            stopThreads = false;

            thdDraw = new Thread(threadDraw);
            thdDraw.Name = "ThreadDraw";

            thdUpdate = new Thread(threadUpdate);
            thdUpdate.Name = "ThreadUpdate";

            form.Shown += (object sender, EventArgs e) =>
            {
                if (Setup != null) Setup(this, form);

                thdDraw.Start();
                thdUpdate.Start();
            };
            form.Paint += (object sender, PaintEventArgs e) =>
            {
                DrawEventArgs dea = new DrawEventArgs()
                {
                    Form = form,
                    Graphics = e.Graphics
                };
                Draw(this, dea);
            };

            swTotal.Start();
            Application.Run(form);
            stopThreads = true;

        }
        public Rectangle GetTileSize(int XPos, int YPos, int TotalXTiles, int TotalYTiles)
        {
            int tileSizeX = form.Width / TotalXTiles;
            int tileSizeY = form.Height / TotalYTiles;

            return new Rectangle(XPos * tileSizeX, YPos * tileSizeY, tileSizeX, tileSizeY);
        }

        public double GetFPS(TimeSpan ts) { return 1000 / ts.TotalMilliseconds; }

        private void threadUpdate(object obj)
        {
            Stopwatch swUpdateTimer = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            TimeSpan last = new TimeSpan();
            sw.Start();
            swUpdateTimer.Start();
            while (!stopThreads)
            {
                sw.Restart();
                swUpdateTimer.Restart();
                DoSleep(0);

                if (Update != null) Update(this, new UpdateEventArgs()
                {
                    LastUpdateTimer = last,
                    TotalTime = swTotal.Elapsed
                });

                sw.Stop();
                LastUpdateTime = sw.Elapsed;
                if (LastUpdateTime.TotalMilliseconds < FPSFrameTime)
                {
                    DoSleep(FPSFrameTime - (int)LastUpdateTime.TotalMilliseconds);
                }

                swUpdateTimer.Stop();
                last = swUpdateTimer.Elapsed;
            }
        }

        Bitmap bmp = null;
        private void threadDraw(object obj)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!stopThreads)
            {
                sw.Restart();
                DoSleep(0);

                if (Draw != null)
                {
                    if (form.Disposing) return;
                    if (form.IsDisposed) return;

                    try
                    {
                        form.Invoke(new MethodInvoker(() =>
                        {
                            form.Invalidate();
                        }));
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception ex) { throw; }
                }

                var elapsed = sw.Elapsed;
                DoSleep(FPSFrameTime - (int)elapsed.TotalMilliseconds - 1);

                LastDrawTime = sw.Elapsed;
            }
        }
        private void DoSleep(int p)
        {
            if (p >= 0) Thread.Sleep(p);
        }

        bool stopThreads;
        //public void Close()
        //{
        //    stopThreads = true;
        //    form.Close();
        //}
    }
}
