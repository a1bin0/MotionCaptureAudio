using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNI;

namespace MotionCaptureAudio
{
    class MotionDetector
    {
        private DepthGenerator depth;
        private Dictionary<int, List<Dictionary<SkeletonJoint, SkeletonJointPosition>>> histryData = new Dictionary<int, List<Dictionary<SkeletonJoint, SkeletonJointPosition>>>();
        private readonly int positionMaxCount = 5;
        private readonly double margin = 20.0;

        public event EventHandler LeftHandUpDetected;
        public event EventHandler LeftHandDownDetected;
        public event EventHandler LeftHandSwipeLeftDetected;
        public event EventHandler LeftHandOverHeadDetected;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MotionDetector(DepthGenerator depth)
        {
            this.depth = depth;
        }

        public void DetectMotion(int userID, Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton)
        {
            saveHistory(userID, skeleton);
            List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions = this.histryData[userID];

            if (positions.Count == this.positionMaxCount)
            {
                if (this.detectLeftHandSwipeUp(positions) && (positions.All(item => this.isLeftPosition(item))))
                {
                    this.LeftHandUpDetected(this, EventArgs.Empty);
                }
                else if (this.detectLeftHandDown(positions) && (positions.All(item => this.isLeftPosition(item))))
                {
                    this.LeftHandDownDetected(this, EventArgs.Empty);
                }
                else if (this.detectLeftHandSwipeLeft(positions) && (positions.All(item => this.isLeftPosition(item))))
                {
                    this.LeftHandSwipeLeftDetected(this, EventArgs.Empty);
                }
                else if (this.detectLeftHandOverHead(positions) && (positions.All(item => this.isBetweenPosition(item))))
                {
                    this.LeftHandOverHeadDetected(this, EventArgs.Empty);
                }
            }
        }

        private void saveHistory(int userID, Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton)
        {
            if (!this.histryData.ContainsKey(userID))
            {
                this.histryData.Add(userID, new List<Dictionary<SkeletonJoint, SkeletonJointPosition>>());
            }

            List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions = this.histryData[userID];

            if (positions.Count > this.positionMaxCount - 1) positions.RemoveAt(0);

            positions.Add(skeleton);
        }

        /// <summary>
        /// 左手が頭上にあるか
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool detectLeftHandOverHead(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                Point3D handData = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D headData = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.Head].Position);
                if (handData.Y > headData.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 左手が左に移動したか
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool detectLeftHandSwipeLeft(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount - 1; i++)
            {
                Point3D oldData = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D newData = depth.ConvertRealWorldToProjective(positions[i + 1][SkeletonJoint.LeftHand].Position);
                if (oldData.X < newData.X) return false;
            }

            return (depth.ConvertRealWorldToProjective(positions[0][SkeletonJoint.LeftHand].Position).X - margin <
                    depth.ConvertRealWorldToProjective(positions[this.positionMaxCount - 1][SkeletonJoint.LeftHand].Position).X);
        }

        /// <summary>
        /// 左手が下がっているか
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool detectLeftHandDown(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount - 1; i++)
            {
                Point3D oldData = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D newData = depth.ConvertRealWorldToProjective(positions[i + 1][SkeletonJoint.LeftHand].Position);
                if (oldData.Y > newData.Y) return false;
            }

            return (depth.ConvertRealWorldToProjective(positions[0][SkeletonJoint.LeftHand].Position).Y + margin >
                    depth.ConvertRealWorldToProjective(positions[this.positionMaxCount - 1][SkeletonJoint.LeftHand].Position).Y);
        }

        /// <summary>
        /// 左手が上がっているか
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool detectLeftHandSwipeUp(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount - 1; i++)
            {
                Point3D oldData = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D newData = depth.ConvertRealWorldToProjective(positions[i + 1][SkeletonJoint.LeftHand].Position);
                if (oldData.Y < newData.Y) return false;
            }

            return (depth.ConvertRealWorldToProjective(positions[0][SkeletonJoint.LeftHand].Position).Y - margin <
                    depth.ConvertRealWorldToProjective(positions[this.positionMaxCount - 1][SkeletonJoint.LeftHand].Position).Y);
        }

        /// <summary>
        /// 左手が左肩より左にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isLeftPosition(Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton)
        {
            return (depth.ConvertRealWorldToProjective(skeleton[SkeletonJoint.LeftHand].Position).X <
                    depth.ConvertRealWorldToProjective(skeleton[SkeletonJoint.LeftShoulder].Position).X);
        }

        /// <summary>
        /// 左手が両肩の間にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isBetweenPosition(Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton)
        {
            return (!this.isLeftPosition(skeleton) &&
                    depth.ConvertRealWorldToProjective(skeleton[SkeletonJoint.LeftHand].Position).X <
                    depth.ConvertRealWorldToProjective(skeleton[SkeletonJoint.RightShoulder].Position).X);
        }
    }
}
