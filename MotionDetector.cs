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

        public event EventHandler LeftHandUpDetected;
        public event EventHandler LeftHandDownDetected;
        public event EventHandler RightHandUpDetected;
        public event EventHandler RightHandDownDetected;
        public event EventHandler IdleDetected;

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

            if (positions.Count >= this.positionMaxCount)
            {
                if (positions.All(item => this.isLeftUp(positions)))
                {
                    this.LeftHandUpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isLeftDown(positions)))
                {
                    this.LeftHandDownDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isRightUp(positions)))
                {
                    this.RightHandUpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isRightDown(positions)))
                {
                    this.RightHandDownDetected(this, EventArgs.Empty);
                }
                else
                {
                    this.IdleDetected(this, EventArgs.Empty);
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
        /// 左手が左肩より左上にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isLeftUp(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                Point3D leftHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D leftShoulder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftShoulder].Position);

                if (leftHand.X > leftShoulder.X || leftHand.Y > leftShoulder.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 左手が腰より左下にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isLeftDown(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                Point3D leftHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D leftShoulder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftShoulder].Position);
                Point3D waist = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.Waist].Position);

                  if (leftHand.X > leftShoulder.X || leftHand.Y < waist.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 右手が右肩より右上にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isRightUp(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                Point3D rightHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightHand].Position);
                Point3D rightShoulder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightShoulder].Position);

                if (rightHand.X < rightShoulder.X || rightHand.Y > rightShoulder.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 右手が腰より右下にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isRightDown(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                Point3D rightHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightHand].Position);
                Point3D rightShoulder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightShoulder].Position);
                Point3D waist = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.Waist].Position);

                if (rightHand.X < rightShoulder.X || rightHand.Y < waist.Y) return false;
            }

            return true;
        }
    }
}
