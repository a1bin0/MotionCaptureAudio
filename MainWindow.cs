using OpenNI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MotionCaptureAudio
{
    public partial class MainWindow : Form
    {
        #region instance fields

        private Context context;
        private ScriptNode scriptNode;
        private bool shouldRun = true;
        private DepthGenerator depth;
        private UserGenerator userGene;
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private MotionDetector motionDetector;
        private int playerId = 0;
        private Graphics graphics;
        private List<Color> userColors = new List<Color>() { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple, Color.Plum };

        private CommandState currentState = CommandState.none;
        private Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints = new Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>();

        private Dictionary<int, DetctionStatus> detectionStatusMap = new Dictionary<int, DetctionStatus>();

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
        }

        /// <summary>
        /// ユーザの検出状態
        /// </summary>
        enum DetctionStatus
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

            this.context = Context.CreateFromXmlFile(@"Config.xml", out this.scriptNode);
            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            this.context.GlobalMirror = true;
            this.setupMotiondetector();

            this.userGene = new UserGenerator(context);
            this.userGene.NewUser += this.user_NewUser;
            this.userGene.LostUser += this.user_Lost;
            this.userGene.SkeletonCapability.CalibrationComplete += this.SkeletonCapability_CalibrationComplete;
            this.userGene.SkeletonCapability.SetSkeletonProfile(SkeletonProfile.All);

            this.context.StartGeneratingAll();

            this.graphics = this.pictBox.CreateGraphics();
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

        private void leftHandDownDetected(object sender, EventArgs e)
        {
            if (this.playerController.CanPlay && this.currentState != CommandState.volumeDown)
            {
                this.playerController.VolumeDown(this.playerId);
                this.currentState = CommandState.volumeDown;
            }
        }

        private void bothHandUpDetected(object sender, EventArgs e)
        {
            this.shouldRun = false;

            this.playerController.Pause(this.playerId);
            this.playerController.Dispose();
            this.Close();
        }

        private void leftHandUpDetected(object sender, EventArgs e)
        {
            if (this.playerController.CanPlay && this.currentState != CommandState.volumeUp)
            {
                this.playerController.VolumeUp(this.playerId);
                this.currentState = CommandState.volumeUp;
            }
        }

        private void rightHandUpDetected(object sender, EventArgs e)
        {
            if (this.playerController.CanPlay && this.currentState != CommandState.playPausecChange)
            {
                this.playerController.PlayPauseChange(this.playerId);
                this.currentState = CommandState.playPausecChange;
            }
        }

        private void rightHandDownDetected(object sender, EventArgs e)
        {
            if (this.playerController.CanPlay && this.currentState != CommandState.playerChange)
            {
                this.playerId = ((this.playerId + 1) % 3);
                this.currentState = CommandState.playerChange;
                this.playerController.backColorChange(this.playerId);
            }
        }

        private void idleDetected(object sender, EventArgs e)
        {
            if (this.currentState != CommandState.none)
            {
                this.currentState = CommandState.none;
                Console.WriteLine("この姿勢ではコマンドが飛ばない");
            }
        }

        void SkeletonCapability_CalibrationComplete(object sender, CalibrationProgressEventArgs e)
        {
            if (e.Status == CalibrationStatus.OK)
            {
                userGene.SkeletonCapability.StartTracking(e.ID);
                Debug.Assert(this.detectionStatusMap.ContainsKey(e.ID), "e.Id not found.");
                this.detectionStatusMap[e.ID] = DetctionStatus.calibrated;
            }
        }

        void user_NewUser(object sender, NewUserEventArgs e)
        {
            Console.WriteLine(String.Format("ユーザ検出: {0}", e.ID));
            if (!this.detectionStatusMap.ContainsKey(e.ID))
            {
                this.detectionStatusMap.Add(e.ID, DetctionStatus.detected);
            }
            else
            {
                this.detectionStatusMap[e.ID] = DetctionStatus.detected;
            }

            userGene.SkeletonCapability.RequestCalibration(e.ID, true);
        }

        private void user_Lost(object sender, UserLostEventArgs e)
        {
            Console.WriteLine(String.Format("ユーザ消失: {0}", e.ID));
            this.detectionStatusMap[e.ID] = DetctionStatus.none;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void ReaderThread()
        {
            while (this.shouldRun)
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

                        if (this.detectionStatusMap[user] == DetctionStatus.calibrated)
                        {
                            this.motionDetector.DetectMotion(user, pointDict);
                        }

                        var pointDic = new List<Object>() { user, pointDict };

                        this.Invoke(new Action<int, Dictionary<SkeletonJoint, SkeletonJointPosition>>(drawSkeleton), pointDic.ToArray());
                    }

                    this.pictBox.Invalidate();
                }));
            }
        }

        private void drawSkeleton(int user, Dictionary<SkeletonJoint, SkeletonJointPosition> pointDict)
        {
            Color color = (this.detectionStatusMap[user] == DetctionStatus.calibrated) ? this.userColors[user] : Color.Gray;

            DrawLine(this.graphics, color, pointDict, SkeletonJoint.Head, SkeletonJoint.Neck);

            DrawLine(this.graphics, color, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.RightHip);
            DrawLine(this.graphics, color, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.LeftHip);

            DrawLine(this.graphics, color, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow);
            DrawLine(this.graphics, color, pointDict, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand);

            DrawLine(this.graphics, color, pointDict, SkeletonJoint.LeftShoulder, SkeletonJoint.RightShoulder);
            DrawLine(this.graphics, color, pointDict, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow);
            DrawLine(this.graphics, color, pointDict, SkeletonJoint.RightElbow, SkeletonJoint.RightHand);

            DrawLine(this.graphics, color, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.RightHip);
            DrawLine(this.graphics, color, pointDict, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee);
            DrawLine(this.graphics, color, pointDict, SkeletonJoint.RightHip, SkeletonJoint.RightKnee);
        }

        private void GetJoint(int user, SkeletonJoint joint)
        {
            SkeletonJointPosition pos = this.userGene.SkeletonCapability.GetSkeletonJointPosition(user, joint);
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

            if (dict[j1].Confidence == 0 || dict[j2].Confidence == 0) return;

            g.DrawLine(new Pen(color, 10),
                        new Point((int)(pos1.X * 2), (int)(pos1.Y * 2)),
                        new Point((int)(pos2.X * 2), (int)(pos2.Y * 2)));
        }

        #endregion methods
    }
}