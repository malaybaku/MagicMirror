
using System;
using System.Collections.Generic;
using System.Linq;

namespace Baku.MagicMirror.Kinect
{
    /// <summary>ロボット(Pepper)の関節角度情報</summary>
    public class RobotJointAngles
    {

        #region 間接名の列挙

        public float HeadYaw { get; set; }
        public float HeadPitch { get; set; }

        public float LShoulderPitch { get; set; }
        public float LShoulderRoll { get; set; }
        public float LElbowYaw { get; set; }
        public float LElbowRoll { get; set; }
        public float LWristYaw { get; set; }
        public float LHand { get; set; }

        public float HipRoll { get; set; }
        public float HipPitch { get; set; }
        public float KneePitch { get; set; }

        public float RShoulderPitch { get; set; }
        public float RShoulderRoll { get; set; }
        public float RElbowYaw { get; set; }
        public float RElbowRoll { get; set; }
        public float RWristYaw { get; set; }
        public float RHand { get; set; }

        public float WheelFL { get; set; }
        public float WheelFR { get; set; }
        public float WheelB { get; set; }

        #endregion

        public const int JointNumberToUse = 13;

        //NOTE: 首の方向はうまく取れない。

        /// <summary>リモート指示で用いる角度一覧を取得します。</summary>
        public float[] Angles
        {
            get
            {
                return new float[]
                {
                    LShoulderPitch,
                    LShoulderRoll,
                    LElbowYaw,
                    LElbowRoll,
                    LWristYaw,
                    RShoulderPitch,
                    RShoulderRoll,
                    RElbowYaw,
                    RElbowRoll,
                    RWristYaw,
                    HipPitch,
                    LHand,
                    RHand
                };
            }
        }

        /// <summary>
        /// 名前と対応取った角度を取得
        /// (<see cref="AngleNames"/>を一度だけ取得して<see cref="Angles"/>と併用した方が速度的には有利)
        /// </summary>
        public Tuple<string, float>[] NamedAngles
        {
            get
            {
                return new []
                {
                    new Tuple<string, float>(nameof(LShoulderPitch), LShoulderPitch),
                    new Tuple<string, float>(nameof(LShoulderRoll), LShoulderRoll),
                    new Tuple<string, float>(nameof(LElbowYaw), LElbowYaw),
                    new Tuple<string, float>(nameof(LElbowRoll), LElbowRoll),
                    new Tuple<string, float>(nameof(LWristYaw), LWristYaw),
                    new Tuple<string, float>(nameof(RShoulderPitch), RShoulderPitch),
                    new Tuple<string, float>(nameof(RShoulderRoll), RShoulderRoll),
                    new Tuple<string, float>(nameof(RElbowYaw), RElbowYaw),
                    new Tuple<string, float>(nameof(RElbowRoll), RElbowRoll),
                    new Tuple<string, float>(nameof(RWristYaw), RWristYaw),
                    new Tuple<string, float>(nameof(HipPitch), HipPitch),
                    new Tuple<string, float>(nameof(LHand), LHand),
                    new Tuple<string, float>(nameof(RHand), RHand)
                };
            }
        }

        /// <summary>角度名を取得</summary>
        public static IReadOnlyList<string> AngleNames { get; } 
            = new RobotJointAngles().NamedAngles.Select(i => i.Item1).ToArray();

    }
}
