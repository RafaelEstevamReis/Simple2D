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

        int realDrawFpsSecond = 0;
        int realDrawFpsCounter = 0;
        public int RealDrawFPS { get; private set; }

        int realUpdateFpsSecond = 0;
        int realUpdateFpsCounter = 0;
        public int RealUpdateFPS { get; private set; }

        Dictionary<Keys, bool> dicKeysStatus = new Dictionary<Keys, bool>();
        public bool this[Keys IsKeyPressed]
        {
            get
            {
                if (dicKeysStatus.ContainsKey(IsKeyPressed))
                    return dicKeysStatus[IsKeyPressed];
                return false;
            }
            set
            {
                if (dicKeysStatus.ContainsKey(IsKeyPressed)) dicKeysStatus[IsKeyPressed] = value;
                else dicKeysStatus.Add(IsKeyPressed, value);
            }
        }

        Thread thdUpdate;
        Thread thdDraw;

        myForm form = null;
        Stopwatch swTotal = new Stopwatch();

        bool stopThreads;

        private class myForm : Form
        {
            public myForm()
                : base()
            {
                DoubleBuffered = true;
            }
            public override void Refresh()
            {
                // can this.InvokePaint be usefull ?
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)Refresh);
                    return;
                }

                base.Refresh();
            }
        }
        public void Show(WindowInfo Info)
        {
            Info.Validate();

            form = new myForm();
            form.BackColor = Color.Azure;
            form.AutoValidate = AutoValidate.Disable;
            form.KeyPreview = true;

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
                executeUpdate(e);
            };
            form.KeyDown += form_KeyDown;
            form.KeyUp += form_KeyUp;
            resetKeyStatus();

            swTotal.Start();
            Application.Run(form);
            stopThreads = true;

        }
        private void resetKeyStatus()
        {
            dicKeysStatus = new Dictionary<Keys, bool>();
        }
        void form_KeyUp(object sender, KeyEventArgs e)
        {
            this[e.KeyCode] = false;
        }
        void form_KeyDown(object sender, KeyEventArgs e)
        {
            this[e.KeyCode] = true;
        }
        public void Close()
        {
            stopThreads = true;
            form.Close();
        }

        private void threadUpdate(object obj)
        {
            Stopwatch swUpdateTimer = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            TimeSpan last = new TimeSpan();
            sw.Start();
            swUpdateTimer.Start();
            int frameCount = 0;
            while (!stopThreads)
            {
                sw.Restart();
                swUpdateTimer.Restart();
                DoSleep(0);

                realUpdateFpsCounter++;
                if (realUpdateFpsSecond != DateTime.Now.Second)
                {
                    realUpdateFpsSecond = DateTime.Now.Second;
                    RealUpdateFPS = realUpdateFpsCounter;
                    realUpdateFpsCounter = 0;
                }

                if (Update != null) Update(this, new UpdateEventArgs()
                {
                    LastUpdateTimer = last,
                    TotalTime = swTotal.Elapsed,
                    FrameCount = ++frameCount
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
                        // Refresh call Invalidate to "mark as dirty"
                        // then calls Update for send a WM_PAINT
                        form.Refresh();
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception) { throw; }
                }

                var elapsed = sw.Elapsed;
                DoSleep(FPSFrameTime - (int)elapsed.TotalMilliseconds - 1);
            }
        }
        Stopwatch swDraw = new Stopwatch();
        private void executeUpdate(PaintEventArgs e)
        {
            swDraw.Restart();
            // Comes from Form DoPaint, not from ThreadUpdate
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear; // is not shrinking
            DrawEventArgs dea = new DrawEventArgs()
            {
                Form = form,
                Graphics = e.Graphics,
                LastElapsed = LastDrawTime,
            };
            Draw(this, dea);
            var elapsed = swDraw.Elapsed;
            LastDrawTime = elapsed;
            // do not wait here, wait is done in thread

            realDrawFpsCounter++;
            if (realDrawFpsSecond != DateTime.Now.Second)
            {
                realDrawFpsSecond = DateTime.Now.Second;
                RealDrawFPS = realDrawFpsCounter;
                realDrawFpsCounter = 0;
            }
        }

        public Rectangle GetTileSize(int XPos, int YPos, int TotalXTiles, int TotalYTiles)
        {
            int tileSizeX = form.Width / TotalXTiles;
            int tileSizeY = form.Height / TotalYTiles;

            return new Rectangle(XPos * tileSizeX, YPos * tileSizeY, tileSizeX, tileSizeY);
        }

        private void DoSleep(int p)
        {
            if (p >= 0) Thread.Sleep(p);
        }

    }
}
