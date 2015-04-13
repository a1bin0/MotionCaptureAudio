using OpenNI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MotionCaptureAudio
{
    public partial class MainWindow : Form
    {
        #region instance fields

        private const string VolumeUp = "Volume up ↑↑";
        private const string VolumeDown = "Volume down ↓↓";
        private const string PlayMusic = "Play music!!";
        private const string PauseMusic = "Pause music!!";
        private const string ChangePlayer = "Player changed!!";
        private const string Terminate = "Application terminate...";

        private int currentUserId = 1;
        private int countDownTime = 3;

        private Bitmap img;

        private Context context;
        private ScriptNode scriptNode;
        private DepthGenerator depth;
        private UserGenerator userGene;
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private MotionDetector motionDetector;
        private int playerId = 0;
        private System.Windows.Forms.Timer messageTimer = null;
        private System.Windows.Forms.Timer countDownTimer = null;
        private string message = string.Empty;
        private CommandState currentState = CommandState.none;

        private int detectionCount = 0;
        private int detectionMaxCount = 2;

        private Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> lastUserJoints = new Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>();

        /// <summary>
        /// ステートです
        /// </summary>
        enum CommandState
        {
            none,
            playPausecChange,
            volumeUp,
            volumeDown,
            playerChange,
            appEnd,
            jump,
        }

        /// <summary>
        /// ユーザの検出状態
        /// </summary>
        enum DetectionStatus
        {
            none,
            detected,
            calibrated,
        }

        #endregion instance fields

        #region constructors

        public MainWindow()
        {
            InitializeComponent();

            this.img = new Bitmap(this.pictBox.Width, this.pictBox.Height);
            this.context = Context.CreateFromXmlFile(@"Config.xml", out this.scriptNode);
            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            this.context.GlobalMirror = false;
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
            this.motionDetector.JumpDetected += this.jumpDetected;
            this.motionDetector.BothHandUpDetected += this.bothHandUpDetected;
            this.motionDetector.LeftHandDownDetected += this.leftHandDownDetected;
            this.motionDetector.LeftHandUpDetected += this.leftHandUpDetected;
            this.motionDetector.RightHandUpDetected += this.rightHandUpDetected;
            this.motionDetector.RightHandDownDetected += this.rightHandDownDetected;
            this.motionDetector.FinishDetected += this.finishDetected;
            this.motionDetector.IdleDetected += this.idleDetected;
        }

        private void startMessageTimer()
        {
            if (this.messageTimer != null && this.messageTimer.Enabled)
            {
                this.messageTimer.Stop();
                this.messageTimer.Dispose();
            }

            this.messageTimer = new System.Windows.Forms.Timer();
            this.messageTimer.Tick += this.clearMessage;
            this.messageTimer.Interval = 2000;

            this.messageTimer.Start();
        }

        private void startCountDownTimer()
        {
            if (this.countDownTimer != null && this.countDownTimer.Enabled)
            {
                this.countDownTimer.Stop();
                this.countDownTimer.Dispose();
            }

            this.message = Terminate;

            this.countDownTimer = new System.Windows.Forms.Timer();
            this.countDownTimer.Tick += this.countDown;
            this.countDownTimer.Interval = 900;

            this.countDownTimer.Start();
        }

        private void countDown(object sender, EventArgs e)
        {
            if (countDownTime == 0)
            {
                this.countDownTimer.Stop();
                this.countDownTimer.Dispose();

                this.Close();
            }

            this.countDownTime--;
        }

        private void clearMessage(object sender, EventArgs e)
        {
            this.messageTimer.Stop();
            this.message = string.Empty;

            this.messageTimer.Dispose();
        }

        private void jumpDetected(object sender, EventArgs e)
        {
            if ((this.currentState != CommandState.jump) &&
                (this.currentState != CommandState.appEnd))
            {
                this.message = Terminate;
                this.startMessageTimer();

                for (int i = 0; i > 3; i++)
                {
                    player.SetMaxVolume(i);
                }

                this.player.Play(0);
                this.player.Play(1);
                this.player.Play(2);

                this.currentState = CommandState.jump;
            }
        }

        private void bothHandUpDetected(object sender, EventArgs e)
        {
            if (this.detectionCount == 2)
            {
                this.currentState = CommandState.none;
                this.currentUserId = this.currentUserId == 1 ? 2 : 1;
            }
        }

        private void finishDetected(object sender, EventArgs e)
        {
            if (this.currentUserId == 1)
            {
                if (this.currentState != CommandState.appEnd)
                {
                    this.currentState = CommandState.appEnd;

                    this.player.Pause(0);
                    this.player.Pause(1);
                    this.player.Pause(2);

                    this.startCountDownTimer();
                }
            }
        }

        private void leftHandDownDetected(object sender, EventArgs e)
        {
            if (this.player.CanDown(this.playerId) &&
                (this.currentState != CommandState.volumeDown) &&
                (this.currentState != CommandState.appEnd))
            {
                this.message = VolumeDown;
                this.startMessageTimer();

                this.player.VolumeDown(this.playerId);
                this.currentState = CommandState.volumeDown;
            }
        }

        private void leftHandUpDetected(object sender, EventArgs e)
        {
            if (this.player.CanUp(this.playerId) &&
                (this.currentState != CommandState.volumeUp) &&
                (this.currentState != CommandState.appEnd))
            {
                this.message = VolumeUp;
                this.startMessageTimer();

                this.player.VolumeUp(this.playerId);
                this.currentState = CommandState.volumeUp;
            }
        }

        private void rightHandUpDetected(object sender, EventArgs e)
        {
            if (this.player.canPlay &&
                (this.currentState != CommandState.playPausecChange) &&
                (this.currentState != CommandState.appEnd))
            {
                this.message = (this.player.IsPlaying(this.playerId)) ? PauseMusic : PlayMusic;
                this.startMessageTimer();

                this.player.PlayPauseChange(this.playerId);
                this.currentState = CommandState.playPausecChange;
            }
        }

        private void rightHandDownDetected(object sender, EventArgs e)
        {
            if (this.player.canPlay &&
                (this.currentState != CommandState.playerChange) &&
                (this.currentState != CommandState.appEnd))
            {
                this.message = ChangePlayer;
                this.startMessageTimer();

                this.playerId = (this.playerId == 2) ? 0 : ++this.playerId;
                this.currentState = CommandState.playerChange;
                this.player.backColorChange(this.playerId);
            }
        }

        private void idleDetected(object sender, EventArgs e)
        {
            if ((this.currentState != CommandState.none) &&
                (this.currentState != CommandState.appEnd))
            {
                this.currentState = CommandState.none;
            }
        }

        void SkeletonCapability_CalibrationComplete(object sender, CalibrationProgressEventArgs e)
        {
            if (e.Status == CalibrationStatus.OK)
            {
                userGene.SkeletonCapability.StartTracking(e.ID);

                if (this.detectionCount == 1)
                {
                    this.player.CalibrationCompleted();
                }
            }
        }

        void user_NewUser(object sender, NewUserEventArgs e)
        {
            if (this.detectionCount < this.detectionMaxCount)
            {
                this.detectionCount++;
                Console.WriteLine(String.Format("ユーザ検出: {0}", e.ID) + "　人数は" + this.detectionCount);
                userGene.SkeletonCapability.RequestCalibration(e.ID, true);

                if (this.detectionCount == 1)
                {
                    this.player.DetectedUser();
                }
            }
        }

        private void user_Lost(object sender, UserLostEventArgs e)
        {
            this.detectionCount--;
            Console.WriteLine(String.Format("ユーザ消失: {0}", e.ID) + "　人数は" + this.detectionCount);

            if (this.currentUserId == e.ID)
            {
                this.currentUserId = (this.currentUserId == 1) ? 2 : 1;
            }

            if (this.detectionCount == 0)
            {
                this.player.LostUser();
            }
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
                        if (!userGene.SkeletonCapability.IsTracking(user))
                        {
                            Debug.WriteLine("User:" + user);
                            continue;
                        }
                        var pointDict = new Dictionary<SkeletonJoint, SkeletonJointPosition>();
                        foreach (SkeletonJoint skeletonJoint in Enum.GetValues(typeof(SkeletonJoint)))
                        {
                            Debug.WriteLine("OOOOUser:" + skeletonJoint);

                            if (!userGene.SkeletonCapability.IsJointAvailable(skeletonJoint)) continue;
                            Debug.WriteLine("1111User:{0:f}", userGene.SkeletonCapability.GetSkeletonJointPosition(user, skeletonJoint).Confidence);

                            pointDict.Add(skeletonJoint, userGene.SkeletonCapability.GetSkeletonJointPosition(user, skeletonJoint));
                        }
                        if (!lastUserJoints.ContainsKey(user))
                        {
                            lastUserJoints.Add(user, pointDict);
                        }
                        else
                        {
                            this.lastUserJoints[user] = pointDict;
                        }

                        if (user == this.currentUserId) this.motionDetector.DetectMotion(user, pointDict);

                        var pointDic = new List<Object>() { user, pointDict };
                        this.Invoke(new Action<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>(draw), pointDic.ToArray());
                        this.pictBox.Invalidate();
                    }
                }));
            }
        }

        private void draw(int user, Dictionary<SkeletonJoint, SkeletonJointPosition> pointDict)
        {
            Graphics g = Graphics.FromImage(this.img);

            foreach (SkeletonJoint item in Enum.GetValues(typeof(SkeletonJoint)))
            {
                if ((pointDict.ContainsKey(item) && (this.lastUserJoints[user].ContainsKey(item))))
                {
                    if (!pointDict[item].Position.Equals(this.lastUserJoints[user][item].Position))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
                g.FillRectangle(Brushes.Black, g.VisibleClipBounds);
                return;
            }

            if ((this.detectionCount < 2) ||
                ((this.detectionCount > 1) && (user == 1)))
            {
                g.FillRectangle(Brushes.Black, g.VisibleClipBounds);
            }

            Color color = (user == this.currentUserId)
                        ? ((user == 1) ? Color.OrangeRed : Color.LightGreen)
                        : Color.White;

            if (!string.IsNullOrEmpty(this.message))
            {
                var font = new Font("Segoe UI", 64, FontStyle.Bold | FontStyle.Italic);
                var brush = new SolidBrush(Color.Yellow);

                g.DrawString(this.message, font, brush, PointF.Empty);

                font.Dispose();
                font = null;

                brush.Dispose();
                brush = null;
            }

            if (this.currentState != CommandState.appEnd)
            {
                DrawLine(g, pointDict, SkeletonJoint.Head, SkeletonJoint.Neck, color);

                DrawLine(g, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.RightHip, color);
                DrawLine(g, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.LeftHip, color);

                DrawLine(g, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow, color);
                DrawLine(g, pointDict, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand, color);

                DrawLine(g, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.RightShoulder, color);
                DrawLine(g, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow, color);
                DrawLine(g, pointDict, SkeletonJoint.RightElbow, SkeletonJoint.RightHand, color);

                DrawLine(g, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.RightHip, color);
                DrawLine(g, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee, color);
                DrawLine(g, pointDict, SkeletonJoint.RightHip, SkeletonJoint.RightKnee, color);

                DrawLine(g, pointDict, SkeletonJoint.LeftKnee, SkeletonJoint.LeftFoot, color);
                DrawLine(g, pointDict, SkeletonJoint.RightKnee, SkeletonJoint.RightFoot, color);

                this.pictBox.BackgroundImage = this.img;
            }
            else
            {
                var font = new Font("Segoe UI", 400, FontStyle.Bold | FontStyle.Italic);
                var brush = new SolidBrush(Color.Yellow);

                g.DrawString(this.countDownTime.ToString(), font, brush, new PointF(220f, 50f));

                font.Dispose();
                font = null;

                brush.Dispose();
                brush = null;
            }

            g.Dispose();
        }

        private void DrawLine(
            Graphics g,
            Dictionary<SkeletonJoint, SkeletonJointPosition> dict,
            SkeletonJoint j1,
            SkeletonJoint j2,
            Color color)
        {
            Point3D pos1 = this.depth.ConvertRealWorldToProjective(dict[j1].Position);
            Point3D pos2 = this.depth.ConvertRealWorldToProjective(dict[j2].Position);

            if (dict[j1].Confidence == 0 || dict[j2].Confidence == 0) return;

            g.DrawLine(new Pen(color, 30),
                        new Point((int)(pos1.X * 3), (int)(pos1.Y * 3)),
                        new Point((int)(pos2.X * 3), (int)(pos2.Y * 3)));

            this.pictBox.Invalidate();
        }

        #endregion methods
    }
}
