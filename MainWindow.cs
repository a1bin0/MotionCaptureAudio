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
using MotionCaptureAudio;
using MotionCaptureAudio.Controller;

namespace MotionCaptureAudio
{
    public partial class MainWindow : Form
    {
        private Context context;
        private ScriptNode scriptNode;
        private Thread readerThread;
        private bool shouldRun;
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

        public EventHandler PlayRequest = null;
        public EventHandler PauseRequest = null;
        public EventHandler VolumeUpRequest = null;
        public EventHandler VolumeDownRequest = null;

        private Dictionary<ActionId, DateTime> timeStamp = new Dictionary<ActionId, DateTime>();

        static readonly int interval = 3;

        private MotionCaptureAudio.Player player;

        public MotionCaptureAudio.Controller.AudioPlayer audioPlayer;

        private void notifyPlayEvent()
        {
            if (this.PlayRequest != null)
            {
                this.PlayRequest(this, EventArgs.Empty);
            }
        }

        private void notifyPauseEvent()
        {
            if (this.PauseRequest != null)
            {
                this.PauseRequest(this, EventArgs.Empty);
            }
        }

        private void notifyVolumeUp()
        {
            if (this.VolumeUpRequest != null)
            {
                this.VolumeUpRequest(this, EventArgs.Empty);
            }
        }

        private void notifyVolumeDown()
        {
            if (this.VolumeDownRequest != null)
            {
                this.VolumeDownRequest(this, EventArgs.Empty);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.context = Context.CreateFromXmlFile(@"./Config.xml", out scriptNode);
            this.sessionManager = new SessionManager(context, "RaiseHand");
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

            Console.WriteLine("手を翳してNITEを初期化してください");

            this.shouldRun = true;
        }

        public void StartThread()
        {
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

                this.notifyPlayEvent();
            }
        }

        private void push_Stable(object sender, VelocityEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.stable]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("|дﾟ)バックオーライ");
                timeStamp[ActionId.stable] = DateTime.Now;

                this.notifyPlayEvent();
            }
        }

        private void swipe_SwipeDown(object sender, VelocityAngleEventArgs e)
        {
            int timeSpan = DateTime.Now.Subtract(timeStamp[ActionId.down]).Seconds;

            if (timeSpan > interval)
            {
                Console.WriteLine("<m(__)m>シタムキ");
                timeStamp[ActionId.down] = DateTime.Now;

                //this.
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

                this.notifyPauseEvent();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.shouldRun = false;
            this.readerThread.Join();
            base.OnClosing(e);
        }

        private unsafe void ReaderThread()
        {
            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitAnyUpdateAll();
                    this.sessionManager.Update(context);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("エラー" + e.StackTrace);
                }
            }
        }
    }
}
