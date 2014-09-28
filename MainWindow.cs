using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using NITE;
using OpenNI;
using System.Diagnostics;
using System.Collections.Generic;

namespace SimpleViewer.net
{
    public partial class MainWindow : Form
    {
        private Context context;
        private ScriptNode scriptNode;
        private DepthGenerator depth;
        private Thread readerThread;
        private bool shouldRun;
        private Bitmap bitmap;
        private int[] histogram;
        private SessionManager sessionManager;

        enum ActionId
        {
            push,
            stable,
            down,
            up,
            right,
            left,
            steady,
        }

        private Dictionary<ActionId, DateTime> timeStamp = new Dictionary<ActionId, DateTime>();

        static readonly int interval = 5;

        public MainWindow()
        {
            InitializeComponent();
            this.context = Context.CreateFromXmlFile(@"../../MotionCaptureAudio/Setting/Config.xml", out scriptNode);
            this.sessionManager = new SessionManager(context, "Wave");
            sessionManager.SessionStart += new EventHandler<PositionEventArgs>(sessionManager_SessionStart);
            var now = DateTime.Now.AddSeconds(-interval);

            timeStamp[ActionId.push] = now;
            timeStamp[ActionId.stable] = now;
            timeStamp[ActionId.down] = now;
            timeStamp[ActionId.up] = now;
            timeStamp[ActionId.right] = now;
            timeStamp[ActionId.left] = now;
            timeStamp[ActionId.steady] = now;

            PushDetector push = new PushDetector();
            push.Push += new EventHandler<VelocityAngleEventArgs>(push_Push);
            push.Stable += new EventHandler<VelocityEventArgs>(push_Stable);
            sessionManager.AddListener(push);

            SwipeDetector swipe = new SwipeDetector();
            swipe.SwipeDown += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeDown);
            swipe.SwipeUp += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeUp);
            swipe.SwipeRight += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeRight);
            swipe.SwipeLeft += new EventHandler<VelocityAngleEventArgs>(swipe_SwipeLeft);
            sessionManager.AddListener(swipe);

            SteadyDetector steady = new SteadyDetector();
            steady.Steady += new EventHandler<SteadyEventArgs>(steady_Steady);
            sessionManager.AddListener(steady);

            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (this.depth == null)
            {
                throw new Exception("Viewer must have a depth node!");
            }

            this.histogram = new int[this.depth.DeviceMaxDepth];

            MapOutputMode mapMode = this.depth.MapOutputMode;

            Console.WriteLine("手を振ってNITEを初期化してください");

            this.bitmap = new Bitmap((int)mapMode.XRes, (int)mapMode.YRes, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.shouldRun = true;
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
        }

        void sessionManager_SessionStart(object sender, PositionEventArgs e)
        {
            Console.WriteLine("上下左右前後と停止のジェスチャーを認識します");
        }

        private void push_Push(object sender, VelocityAngleEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.push]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("(/・ω・)/彡☆マエ");
                timeStamp[ActionId.push] = DateTime.Now;
            }
        }

        private void push_Stable(object sender, VelocityEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.stable]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("|дﾟ)バックオーライ");
                timeStamp[ActionId.stable] = DateTime.Now;
            }
        }

        private void swipe_SwipeDown(object sender, VelocityAngleEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.down]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("<m(__)m>シタムキ");
                timeStamp[ActionId.down] = DateTime.Now;
            }
        }

        private void swipe_SwipeUp(object sender, VelocityAngleEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.up]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("Σ(･ω･ﾉ)ﾉウエ！");
                timeStamp[ActionId.up] = DateTime.Now;
            }
        }

        private void swipe_SwipeRight(object sender, VelocityAngleEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.right]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("(￣▽￣)(￣▽￣)ミギニオナジ");
                timeStamp[ActionId.right] = DateTime.Now;
            }
        }

        private void swipe_SwipeLeft(object sender, VelocityAngleEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.left]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("(-_-メ)ヒダリホホヲフショウ");
                timeStamp[ActionId.left] = DateTime.Now;
            }
        }

        private void steady_Steady(object sender, SteadyEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.steady]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("くコ:彡イカ！イカ！ストップ！");
                timeStamp[ActionId.steady] = DateTime.Now;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            lock (this)
            {
                e.Graphics.DrawImage(this.bitmap,
                    this.panelView.Location.X,
                    this.panelView.Location.Y,
                    this.panelView.Size.Width,
                    this.panelView.Size.Height);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //Don't allow the background to paint
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.shouldRun = false;
            this.readerThread.Join();
            base.OnClosing(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                Close();
            }
            base.OnKeyPress(e);
        }

        private unsafe void CalcHist(DepthMetaData depthMD)
        {
            // reset
            for (int i = 0; i < this.histogram.Length; ++i)
                this.histogram[i] = 0;

            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    ushort depthVal = *pDepth;
                    if (depthVal != 0)
                    {
                        this.histogram[depthVal]++;
                        points++;
                    }
                }
            }

            for (int i = 1; i < this.histogram.Length; i++)
            {
                this.histogram[i] += this.histogram[i - 1];
            }

            if (points > 0)
            {
                for (int i = 1; i < this.histogram.Length; i++)
                {
                    this.histogram[i] = (int)(256 * (1.0f - (this.histogram[i] / (float)points)));
                }
            }
        }

        private unsafe void ReaderThread()
        {
            DepthMetaData depthMD = new DepthMetaData();

            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitOneUpdateAll(this.depth);
                    this.sessionManager.Update(context);
                }
                catch (Exception)
                {
                }

                this.depth.GetMetaData(depthMD);

                CalcHist(depthMD);

                lock (this)
                {
                    Rectangle rect = new Rectangle(0, 0, this.bitmap.Width, this.bitmap.Height);
                    BitmapData data = this.bitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    ushort* pDepth = (ushort*)this.depth.DepthMapPtr.ToPointer();

                    // set pixels
                    for (int y = 0; y < depthMD.YRes; ++y)
                    {
                        byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                        for (int x = 0; x < depthMD.XRes; ++x, ++pDepth, pDest += 3)
                        {
                            byte pixel = (byte)this.histogram[*pDepth];
                            pDest[0] = 0;
                            pDest[1] = pixel;
                            pDest[2] = pixel;
                        }
                    }

                    this.bitmap.UnlockBits(data);
                }

                this.Invalidate();
            }
        }
    }
}
