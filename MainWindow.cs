using MotionCaptureAudio.Controller;
using OpenNI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MotionCaptureAudio
{
    public partial class MainWindow : Form
    {
        #region instance fields

        private const string volumeUp = "Volume up ↑↑";
        private const string volumeDown = "Volume down ↓↓";
        private const string playMusic = "Play music";
        private const string pauseMusic = "Pause music";
        private const string changePlayer = "Player changed";

        private Bitmap img;

        private Context context;
        private ScriptNode scriptNode;
        private DepthGenerator depth;
        private UserGenerator userGene;
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private MotionDetector motionDetector;
        private int playerId = 0;
        private string message = string.Empty;
        private System.Windows.Forms.Timer messageTimer = null;
        private CommandState currentState = CommandState.None;
        private Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints
            = new Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>();

        private DetectionStatus detectionStatus = DetectionStatus.None;

        /// <summary>
        /// 操作ステート
        /// </summary>
        enum CommandState
        {
            None,
            PlayPauseChange,
            VolumeUp,
            VolumeDown,
            PlayerChange,
        }

        /// <summary>
        /// ユーザの検出状態
        /// </summary>
        enum DetectionStatus
        {
            None,
            Detected,
            Calibrated,
        }

        #endregion instance fields

        #region constructors

        public MainWindow()
        {
            InitializeComponent();

            this.img = new Bitmap(this.pictBox.Width, this.pictBox.Height);
            this.context = Context.CreateFromXmlFile(@"..\..\Audio\Config\Config.xml", out this.scriptNode);
            this.depth = this.context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            this.context.GlobalMirror = true;
            this.setupMotiondetector();

            this.userGene = new UserGenerator(context);
            this.userGene.NewUser += this.user_NewUser;
            this.userGene.LostUser += this.user_Lost;
            this.userGene.SkeletonCapability.CalibrationComplete += this.SkeletonCapability_CalibrationComplete;
            this.userGene.SkeletonCapability.SetSkeletonProfile(SkeletonProfile.All);

            this.context.StartGeneratingAll();
        }

        #endregion constructors

        #region methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            new Action(this.ReaderThread).BeginInvoke(null, null);
        }

        private void setupMotiondetector()
        {
            this.motionDetector = new MotionDetector(this.depth);
            this.motionDetector.BothHandUpDetected += this.bothHandUpDetected;
            this.motionDetector.LeftHandDownDetected += this.leftHandDownDetected;
            this.motionDetector.LeftHandUpDetected += this.leftHandUpDetected;
            this.motionDetector.RightHandUpDetected += this.rightHandUpDetected;
            this.motionDetector.RightHandDownDetected += this.rightHandDownDetected;
            this.motionDetector.IdleDetected += this.idleDetected;
        }

        private void startMessageTimer()
        {
            if(this.messageTimer != null && this.messageTimer.Enabled)
            {
                this.messageTimer.Stop();
                this.messageTimer.Dispose();
            }

            this.messageTimer = new System.Windows.Forms.Timer();
            this.messageTimer.Tick += this.clearMessage;
            this.messageTimer.Interval = 2000;

            this.messageTimer.Start();
        }

        private void clearMessage(object sender, EventArgs e)
        {
            this.messageTimer.Stop();
            this.message = string.Empty;

            this.messageTimer.Dispose();
        }

        private void leftHandDownDetected(object sender, EventArgs e)
        {
            if (this.currentState != CommandState.VolumeDown)
            {
                this.message = volumeDown;
                this.startMessageTimer();

                this.integratedPlayer.VolumeDown(this.playerId);
                this.currentState = CommandState.VolumeDown;
            }
        }

        private void bothHandUpDetected(object sender, EventArgs e)
        {
            this.integratedPlayer.Exit();

            this.Close();
        }

        private void leftHandUpDetected(object sender, EventArgs e)
        {
            if (this.currentState != CommandState.VolumeUp)
            {
                this.message = volumeUp;
                this.startMessageTimer();

                this.integratedPlayer.VolumeUp(this.playerId);
                this.currentState = CommandState.VolumeUp;
            }
        }

        private void rightHandUpDetected(object sender, EventArgs e)
        {
            if (this.currentState != CommandState.PlayPauseChange)
            {
                this.message
                    = (this.integratedPlayer.GetPlayState(this.playerId) == PlayState.Playing)
                    ? pauseMusic : playMusic;
                this.startMessageTimer();

                this.integratedPlayer.PlayPauseChange(this.playerId);
                this.currentState = CommandState.PlayPauseChange;
            }
        }

        private void rightHandDownDetected(object sender, EventArgs e)
        {
            if (this.currentState != CommandState.PlayerChange)
            {
                this.playerId = ((this.playerId + 1) % 3);

                this.message = changePlayer + " " + (this.playerId + 1).ToString();
                this.startMessageTimer();

                this.currentState = CommandState.PlayerChange;
                this.integratedPlayer.backColorChange(this.playerId);
            }
        }

        private void idleDetected(object sender, EventArgs e)
        {
            if (this.currentState != CommandState.None)
            {
                this.currentState = CommandState.None;
            }
        }

        void SkeletonCapability_CalibrationComplete(object sender, CalibrationProgressEventArgs e)
        {
            if (e.Status == CalibrationStatus.OK)
            {
                userGene.SkeletonCapability.StartTracking(e.ID);
                this.detectionStatus = DetectionStatus.Calibrated;
                this.integratedPlayer.CalibrationCompleted();
            }
        }

        void user_NewUser(object sender, NewUserEventArgs e)
        {
            Console.WriteLine(String.Format("ユーザ検出: {0}", e.ID));
            if (this.detectionStatus == DetectionStatus.None)
            {
                this.detectionStatus = DetectionStatus.Detected;
                userGene.SkeletonCapability.RequestCalibration(e.ID, true);

                this.integratedPlayer.DetectedUser();
            }
        }

        private void user_Lost(object sender, UserLostEventArgs e)
        {
            Console.WriteLine(String.Format("ユーザ消失: {0}", e.ID));
            this.detectionStatus = DetectionStatus.None;

            this.integratedPlayer.LostUser();
        }

        private void ReaderThread()
        {
            while (true)
            {
                this.context.WaitAndUpdateAll();

                this.dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    int[] users = userGene.GetUsers();
                    foreach (int user in users)
                    {
                        if (!userGene.SkeletonCapability.IsTracking(user)) continue;

                        var pointDict = new Dictionary<SkeletonJoint, SkeletonJointPosition>();
                        foreach (SkeletonJoint skeletonJoint in Enum.GetValues(typeof(SkeletonJoint)))
                        {
                            if (!userGene.SkeletonCapability.IsJointAvailable(skeletonJoint)) continue;
                            pointDict.Add(skeletonJoint, userGene.SkeletonCapability.GetSkeletonJointPosition(user, skeletonJoint));
                        }

                        if (this.detectionStatus == DetectionStatus.Calibrated)
                        {
                            this.motionDetector.DetectMotion(user, pointDict);
                        }

                        var pointDic = new List<Object>() { user, pointDict };

                        this.Invoke(new Action<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>(drawSkeleton), pointDic.ToArray());
                        this.pictBox.Invalidate();
                    }
                }));
            }
        }

        private void drawSkeleton(int user, Dictionary<SkeletonJoint, SkeletonJointPosition> pointDict)
        {
            Graphics g = Graphics.FromImage(this.img);

            g.FillRectangle(Brushes.Black, g.VisibleClipBounds);

            if (!string.IsNullOrEmpty(this.message))
            {
                var font = new Font("Segoe UI", 48, FontStyle.Bold | FontStyle.Italic);
                var brush = new SolidBrush(Color.Yellow);

                g.DrawString(this.message, font, brush, PointF.Empty);

                font.Dispose();
                font = null;

                brush.Dispose();
                brush = null;
            }

            DrawLine(g, pointDict, SkeletonJoint.Head, SkeletonJoint.Neck);

            DrawLine(g, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.RightHip);
            DrawLine(g, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.LeftHip);

            DrawLine(g, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow);
            DrawLine(g, pointDict, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand);

            DrawLine(g, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.RightShoulder);
            DrawLine(g, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow);
            DrawLine(g, pointDict, SkeletonJoint.RightElbow, SkeletonJoint.RightHand);

            DrawLine(g, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.RightHip);
            DrawLine(g, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee);
            DrawLine(g, pointDict, SkeletonJoint.RightHip, SkeletonJoint.RightKnee);

            DrawLine(g, pointDict, SkeletonJoint.LeftKnee, SkeletonJoint.LeftFoot);
            DrawLine(g, pointDict, SkeletonJoint.RightKnee, SkeletonJoint.RightFoot);

            this.pictBox.BackgroundImage = this.img;

            g.Dispose();
        }

        private void DrawLine(Graphics g, Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint j1, SkeletonJoint j2)
        {
            Point3D pos1 = this.depth.ConvertRealWorldToProjective(dict[j1].Position);
            Point3D pos2 = this.depth.ConvertRealWorldToProjective(dict[j2].Position);

            if (dict[j1].Confidence == 0 || dict[j2].Confidence == 0) return;

            g.DrawLine(new Pen(Color.OrangeRed, 30),
                        new Point((int)(pos1.X * 3), (int)(pos1.Y * 3)),
                        new Point((int)(pos2.X * 3), (int)(pos2.Y * 3)));

            this.pictBox.Invalidate();
        }

        #endregion methods
    }
}