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

namespace SimpleViewer.net
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            this.context = Context.CreateFromXmlFile(SAMPLE_XML_FILE, out scriptNode);

            this.sessionManager = new SessionManager(context, "Wave", "RaiseHand");
            sessionManager.SessionStart += new EventHandler<PositionEventArgs>(sessionManager_SessionStart);

            PushDetector push = new PushDetector();
            push.Push += new EventHandler<VelocityAngleEventArgs>(push_Push);
            sessionManager.AddListener(push);

            WaveDetector wave = new WaveDetector();
            wave.Wave += new EventHandler(wave_Wave);
            sessionManager.AddListener(wave);

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

        static void sessionManager_SessionStart(object sender, PositionEventArgs e)
        {
            Console.WriteLine("ジェスチャーを認識する準備ができました");
        }

        static void wave_Wave(object sender, EventArgs e)
        {
            Console.WriteLine("(-_-)/~~~ピシー!ピシー!");
        }

        static void push_Push(object sender, VelocityAngleEventArgs e)
        {
            Console.WriteLine("(/・ω・)/彡☆");
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

        private readonly string SAMPLE_XML_FILE = @"../../../Data/SamplesConfig.xml";

        private Context context;
        private ScriptNode scriptNode;
        private DepthGenerator depth;
        private Thread readerThread;
        private bool shouldRun;
        private Bitmap bitmap;
        private int[] histogram;
        private SessionManager sessionManager;
    }
}
