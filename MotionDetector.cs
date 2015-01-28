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
        private readonly int positionMaxCount = 10;
        private readonly double confidenceBase = 0.95;

        public event EventHandler LeftHandUpDetected;
        public event EventHandler LeftHandDownDetected;
        public event EventHandler RightHandUpDetected;
        public event EventHandler RightHandDownDetected;
        public event EventHandler BothHandUpDetected;
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
                if (positions.All(item => this.isRightUp(positions)) && positions.All(item => this.isLeftUp(positions)))
                {
                    this.BothHandUpDetected(this, EventArgs.Empty);
                }
                if (positions.All(item => this.isRightUp(positions)))
                {
                    this.RightHandUpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isLeftUp(positions)))
                {
                    this.LeftHandUpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isLeftDown(positions)))
                {
                    this.LeftHandDownDetected(this, EventArgs.Empty);
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
        /// 左手が頭上にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isLeftUp(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                if (positions[i][SkeletonJoint.LeftHand].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.Head].Confidence < this.confidenceBase) return false;

                Point3D leftHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D head = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.Head].Position);

                if (leftHand.Y > head.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 左手が腰より下にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isLeftDown(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                if (positions[i][SkeletonJoint.LeftHand].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.LeftHip].Confidence < this.confidenceBase) return false;

                Point3D leftHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D leftHip = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHip].Position);

                  if (leftHand.Y < leftHip.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 右手が頭上にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isRightUp(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                if (positions[i][SkeletonJoint.RightHand].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.Head].Confidence < this.confidenceBase) return false;

                Point3D rightHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightHand].Position);
                Point3D head = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.Head].Position);

                if (rightHand.Y > head.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 右手が腰より下にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isRightDown(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                if (positions[i][SkeletonJoint.RightHand].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.RightHip].Confidence < this.confidenceBase) return false;

                Point3D rightHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightHand].Position);
                Point3D rightHip = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightHip].Position);

                if (rightHand.Y < rightHip.Y) return false;
            }

            return true;
        }
    }
}
