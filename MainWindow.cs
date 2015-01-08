using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using OpenNI;
using System.Collections.Generic;
using System.Drawing;

namespace MotionCaptureAudio
{
    public partial class MainWindow : Form
    {
        #region instance fields

        private Context context;
        private ScriptNode scriptNode;
        private Thread readerThread;
        private bool shouldRun = true;
        private DepthGenerator depth;
        private UserGenerator user;
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private MotionDetector motionDetector;
        private Dictionary<ActionId, DateTime> timeStamp = new Dictionary<ActionId, DateTime>();
        private MotionCaptureAudio.Player player;

        private Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints = new Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>();

        private List<Panel> statusPanels = new List<Panel>();

        #endregion instance fields

        /// <summary>
        /// モーションのパターンです
        /// </summary>
        enum ActionId
        {
            down,
            up,
            overHead,
            left,
        }

        /// <summary>
        /// 同モーションを無視する時間(sec)です
        /// /// </summary>
        static readonly int interval = 2;

        #region constructors

        public MainWindow()
        {
            InitializeComponent();

            this.statusPanels.Add(this.sign1);
            this.statusPanels.Add(this.sign2);
            this.statusPanels.Add(this.sign3);
            this.statusPanels.Add(this.sign4);
            this.statusPanels.Add(this.sign5);

            this.context = Context.CreateFromXmlFile(@"Config2.xml", out this.scriptNode);
            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            this.context.GlobalMirror = true;
            this.setupMotiondetector();

            this.user = new UserGenerator(context);
            this.user.NewUser += this.user_NewUser;
            this.user.LostUser += this.user_Lost;
            this.user.SkeletonCapability.CalibrationComplete += this.SkeletonCapability_CalibrationComplete;
            this.user.SkeletonCapability.SetSkeletonProfile(SkeletonProfile.All);

            DateTime now = DateTime.Now.AddSeconds(-interval);

            //全モーションのタイムスタンプに現在時刻を設定します
            foreach (ActionId id in Enum.GetValues(typeof(ActionId)))
            {
                timeStamp[id] = now;
            }

            this.context.StartGeneratingAll();
            this.player = new Player();
        }

        #endregion constructors

        #region methods
        #endregion methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            new Action(this.ReaderThread).BeginInvoke(null, null);
        }

        private void setupMotiondetector()
        {
            this.motionDetector = new MotionDetector(this.depth);
            this.motionDetector.LeftHandDownDetected += this.leftHandDownDetected;
            this.motionDetector.LeftHandUpDetected += this.leftHandUpDetected;
            this.motionDetector.LeftHandSwipeLeftDetected += this.leftHandSwipeLeftDetected;
            this.motionDetector.LeftHandOverHeadDetected += this.leftHandOverHeadDetected;
        }

        private void leftHandDownDetected(object sender, EventArgs e)
        {
            if (this.player.CanPlay)
            {
                if (DateTime.Now.Subtract(timeStamp[ActionId.up]).Seconds > interval)
                {
                    timeStamp[ActionId.down] = DateTime.Now;
                    this.player.VolumeDown();
                }
            }
        }

        private void leftHandUpDetected(object sender, EventArgs e)
        {
            if (this.player.CanPlay)
            {
                if (DateTime.Now.Subtract(timeStamp[ActionId.up]).Seconds > interval)
                {
                    timeStamp[ActionId.up] = DateTime.Now;
                    this.player.VolumeUp();
                }
            }
        }

        private void leftHandSwipeLeftDetected(object sender, EventArgs e)
        {
            if (this.player.CanPlay)
            {
                if (DateTime.Now.Subtract(timeStamp[ActionId.left]).Seconds > interval)
                {
                    timeStamp[ActionId.left] = DateTime.Now;
                    this.player.Pause();
                }
            }
        }

        private void leftHandOverHeadDetected(object sender, EventArgs e)
        {
            if (this.player.CanPlay)
            {
                if (DateTime.Now.Subtract(timeStamp[ActionId.overHead]).Seconds > interval)
                {
                    timeStamp[ActionId.overHead] = DateTime.Now;
                    this.player.Play();
                }
            }
        }

        void SkeletonCapability_CalibrationComplete(object sender, CalibrationProgressEventArgs e)
        {
            if (e.Status == CalibrationStatus.OK)
            {
                user.SkeletonCapability.StartTracking(e.ID);
                this.statusPanels[e.ID - 1].BackColor = Color.Green;
            }
        }

        void user_NewUser(object sender, NewUserEventArgs e)
        {
            Console.WriteLine(String.Format("ユーザ検出: {0}", e.ID));
            this.statusPanels[e.ID-1].BackColor = Color.Yellow;
            user.SkeletonCapability.RequestCalibration(e.ID, true);
        }

        private void user_Lost(object sender, UserLostEventArgs e)
        {
            Console.WriteLine(String.Format("ユーザ消失: {0}", e.ID));
            this.statusPanels[e.ID - 1].BackColor = Color.Gray;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.shouldRun = false;
            this.readerThread.Join();
            base.OnClosing(e);
        }

        private void ReaderThread()
        {
            while (this.shouldRun)
            {
                this.context.WaitAndUpdateAll();

                this.dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    int[] users = user.GetUsers();
                    foreach (int u in users)
                    {
                        if (!user.SkeletonCapability.IsTracking(u)) continue;

                        var pointDict = new Dictionary<SkeletonJoint, SkeletonJointPosition>();
                        foreach (SkeletonJoint s in Enum.GetValues(typeof(SkeletonJoint)))
                        {
                            if (!user.SkeletonCapability.IsJointAvailable(s)) continue;
                            pointDict.Add(s, user.SkeletonCapability.GetSkeletonJointPosition(u, s));
                        }

                        this.motionDetector.DetectMotion(u, pointDict);
                        var mae = new List<Object>() { u, pointDict };
                        this.Invoke(new Action<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>(drawSkeleton), mae.ToArray());
                        this.pictBox.Invalidate();
                    }
                }));
            }
        }

        private void drawSkeleton(int user, Dictionary<SkeletonJoint, SkeletonJointPosition> pointDict)
        {
            Graphics g = this.pictBox.CreateGraphics();
            List<Color> userColors = new List<Color>() { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple, Color.Plum };

            Color color = userColors[user];
            //Debug.WriteLine(this.joints[user][SkeletonJoint.Neck]);

            DrawLine(g, color, pointDict, SkeletonJoint.Head, SkeletonJoint.Neck);

            DrawLine(g, color, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.Torso);
            DrawLine(g, color, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.Torso);

            DrawLine(g, color, pointDict, SkeletonJoint.Neck, SkeletonJoint.LeftShoulder);
            DrawLine(g, color, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow);
            DrawLine(g, color, pointDict, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand);

            DrawLine(g, color, pointDict, SkeletonJoint.Neck, SkeletonJoint.RightShoulder);
            DrawLine(g, color, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow);
            DrawLine(g, color, pointDict, SkeletonJoint.RightElbow, SkeletonJoint.RightHand);

            DrawLine(g, color, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.Torso);
            DrawLine(g, color, pointDict, SkeletonJoint.RightHip, SkeletonJoint.Torso);
            DrawLine(g, color, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.RightHip);

            DrawLine(g, color, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee);
            DrawLine(g, color, pointDict, SkeletonJoint.LeftKnee, SkeletonJoint.LeftFoot);

            DrawLine(g, color, pointDict, SkeletonJoint.RightHip, SkeletonJoint.RightKnee);
            DrawLine(g, color, pointDict, SkeletonJoint.RightKnee, SkeletonJoint.RightFoot);
        }

        private void GetJoint(int user, SkeletonJoint joint)
        {
            SkeletonJointPosition pos = this.user.SkeletonCapability.GetSkeletonJointPosition(user, joint);
            if (pos.Position.Z == 0)
            {
                pos.Confidence = 0;
            }
            else
            {
                pos.Position = this.depth.ConvertRealWorldToProjective(pos.Position);
            }
            this.joints[user][joint] = pos;
        }

        private void DrawLine(Graphics g, Color color, Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint j1, SkeletonJoint j2)
        {
            
            Point3D pos1 = this.depth.ConvertRealWorldToProjective(dict[j1].Position);
            Point3D pos2 = this.depth.ConvertRealWorldToProjective(dict[j2].Position);

            if (dict[j1].Confidence == 0 || dict[j2].Confidence == 0)
                return;

//            Debug.WriteLine("["+dict[j1].Position.ToString() +"]"+pos1.X.ToString() + "," + pos1.Y.ToString());
            g.DrawLine( new Pen(color, 3),
                        new Point((int)(pos1.X), (int)(pos1.Y)),
                        new Point((int)(pos2.X), (int)(pos2.Y)));

            this.pictBox.Invalidate();
        }
    }
}