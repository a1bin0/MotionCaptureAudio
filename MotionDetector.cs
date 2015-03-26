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
        private float thresholdJump = 60f;

        public event EventHandler LeftHandUpDetected;
        public event EventHandler LeftHandDownDetected;
        public event EventHandler RightHandUpDetected;
        public event EventHandler RightHandDownDetected;
        public event EventHandler BothHandUpDetected;
        public event EventHandler JumpDetected;
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
                if (positions.All(item => this.isJump(positions)))
                {
                    this.JumpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isRightOverSholder(positions)) && positions.All(item => this.isLeftOverSholder(positions)))
                {
                    this.BothHandUpDetected(this, EventArgs.Empty);
                }
                else if (positions.All(item => this.isLeftUp(positions)))
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

        /// <summary>
        /// ジャンプしたか（消すかも？）
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool isJump(List<Dictionary<SkeletonJoint, SkeletonJointPosition>> positions)
        {
            for (int i = 0; i < this.positionMaxCount - 1; i++)
            {
                if (positions[i][SkeletonJoint.RightShoulder].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.LeftShoulder].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.RightHip].Confidence < this.confidenceBase) return false;
                if (positions[i][SkeletonJoint.LeftHip].Confidence < this.confidenceBase) return false;

                Point3D oldRightSholder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightShoulder].Position);
                Point3D newRightSholder = depth.ConvertRealWorldToProjective(positions[i + 1][SkeletonJoint.RightShoulder].Position);

                Point3D oldLeftSholder = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftShoulder].Position);
                Point3D newLeftSholder = depth.ConvertRealWorldToProjective(positions[i + 1][SkeletonJoint.LeftShoulder].Position);

                Point3D oldRightHip = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.RightHip].Position);
                Point3D newRightHip = depth.ConvertRealWorldToProjective(positions[i + 1][SkeletonJoint.RightHip].Position);

                Point3D oldLeftHip = depth.ConvertRealWorldToProjective(positions[i][SkeletonJoint.LeftHip].Position);
                Point3D newLeftHip = depth.ConvertRealWorldToProjective(positions[i + 1][SkeletonJoint.LeftHip].Position);

                if (oldRightSholder.Y  < newRightSholder.Y) return false;
                if (oldLeftSholder.Y  < newLeftSholder.Y) return false;
                if (oldRightHip.Y  < newRightHip.Y) return false;
                if (oldLeftHip.Y  < newLeftHip.Y) return false;
            }

             this.thresholdJump = (Math.Abs (depth.ConvertRealWorldToProjective(positions[0][SkeletonJoint.RightShoulder].Position).X -
                 depth.ConvertRealWorldToProjective(positions[0][SkeletonJoint.LeftShoulder].Position).X)) * 0.9f;

             if (Math.Abs(depth.ConvertRealWorldToProjective(positions[9][SkeletonJoint.RightShoulder].Position).Y -
             depth.ConvertRealWorldToProjective(positions[0][SkeletonJoint.RightShoulder].Position).Y) < this.thresholdJump) return false;
                
            return true;
        }
    }
}
