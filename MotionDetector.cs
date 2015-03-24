using System;
using System.Collections.Generic;
using System.Linq;
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
                if (userID == 1 && positions.All(item => this.isRightOverSholder(positions)) && positions.All(item => this.isLeftOverSholder(positions)))
                {
                    this.BothHandUpDetected(this, EventArgs.Empty);
                }
                if (positions.All(item => this.isLeftUp(positions)))
                {
                    this.RightHandUpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isRightUp(positions)))
                {
                    this.LeftHandUpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isRightDown(positions)))
                {
                    this.LeftHandDownDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isLeftDown(positions)))
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

        /// <summary>
        /// 左手が左肩より上にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isLeftOverSholder(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                if (positions[i][SkeletonJoint.LeftHand].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.LeftShoulder].Confidence < this.confidenceBase) return false;

                Point3D leftHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHand].Position);
                Point3D leftSholder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftShoulder].Position);

                if (leftHand.Y > leftSholder.Y) return false;
            }

            return true;
        }

        /// <summary>
        /// 右手が右肩より上にあるか
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isRightOverSholder(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount; i++)
            {
                if (positions[i][SkeletonJoint.RightHand].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.RightShoulder].Confidence < this.confidenceBase) return false;

                Point3D rightHand = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightHand].Position);
                Point3D rightSholder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightShoulder].Position);

                if (rightHand.Y > rightSholder.Y) return false;
            }

            return true;
        }
    }
}
