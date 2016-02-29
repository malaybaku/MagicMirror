using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

using Baku.MagicMirror.Plugin;
using Baku.LibqiDotNet;
using System.Linq;
using System.Collections.Generic;

namespace Baku.MagicMirror.Kinect.Models
{
    [Export(typeof(IMagicMirrorPlugin))]
    public class KinectController : IMagicMirrorPlugin
    {
        #region IMagicMirrorPlugin

        public string Name { get; } = "Kinect";
        public string JapaneseDescription { get; } = "Microsoft Kinectセンサーでロボットを操作します。";
        public string EnglishDescription { get; } = "Control robot by Microsoft Kinect Sensor";

        public UserControl GuiContent { get; private set; }

        public Guid Guid { get; } = new Guid("6d3c8744-4fdf-4c77-8869-fe209f4bcacc");

        public bool EnableAfterInitialize { get; } = false;

        public void Initialize(IQiConnectionServiceProxy connectionService)
        {
            _connectionService = connectionService;
        }

        public void Enable()
        {

        }

        public void Disable()
        {

        }

        public void Dispose()
        {

        }

        public void Update()
        {
            //意外にも、このプログラムですらUpdateは必要ないのだ！(ポーリングっぽい処理の出どころはKinect)
        }

        #endregion

        private IQiConnectionServiceProxy _connectionService;

        public RobotJointAngles RobotAngles { get; } = new RobotJointAngles();

        public bool SendToAllSelectedSession { get; set; }

        public bool IsEnabled { get; set; }

        /// <summary>対象となるロボットに現在のポーズ情報を送信します。</summary>
        public void SendPose()
        {
            if (!IsEnabled) return;

            //とりあえずカレントセッションに限定
            var m = _connectionService.CurrentSession?.GetService("ALMotion");
            if (m?.Name != "ALMotion") return;

            var angles = RobotAngles.Angles;
            for (int i = 0; i < angles.Length; i++)
            {
                angles[i] = Math.Max(_limits[i].Min,
                            Math.Min(_limits[i].Max, angles[i]));
            }
            m.Call("setAngles",
                _angleNames,
                QiList.Create(angles.Select(a => new QiFloat(a))),
                _fractions
                );
        }

        private static QiList<QiString> _angleNames
            = QiList.Create(RobotJointAngles.AngleNames.Select(n => new QiString(n)));

        private static QiList<QiFloat> _fractions = GetFractions();
        private static IReadOnlyList<AngleLimit> _limits = GetLimits();


        private static QiList<QiFloat> GetFractions()
        {
            var fracs = GetFractionDict();
            return RobotJointAngles
                .AngleNames
                .Select(n => new QiFloat(fracs[n]))
                .ToQiList();
        }
        private static IReadOnlyDictionary<string, float> GetFractionDict()
        {
            return new Dictionary<string, float>()
            {
                {"HeadYaw"        , 0.3f},
                {"HeadPitch"      , 0.2f},
                {"LShoulderPitch" , 0.2f},
                {"LShoulderRoll"  , 0.2f},
                {"LElbowYaw"      , 0.4f},
                {"LElbowRoll"     , 0.6f},
                {"LWristYaw"      , 1.0f},
                {"RShoulderPitch" , 0.2f},
                {"RShoulderRoll"  , 0.2f},
                {"RElbowYaw"      , 0.4f},
                {"RElbowRoll"     , 0.6f},
                {"RWristYaw"      , 1.0f},
                {"HipPitch"       , 0.3f},
                {"HipRoll"        , 0.3f},
                {"LHand"          , 1.0f},
                {"RHand"          , 1.0f}
            };
        }

        private static IReadOnlyList<AngleLimit> GetLimits()
        {
            var limits = GetLimitsDict();
            return RobotJointAngles
                .AngleNames
                .Select(n => limits[n])
                .ToArray();
        }
        //公式ドキュメンテーションに記載された、モーターの角度限界を弧度法で取得
        //(全モーターについて載せてるわけではない点に注意)
        private static IReadOnlyDictionary<string, AngleLimit> GetLimitsDict()
        {
            return new Dictionary<string, AngleLimit>()
            {
                { "HeadYaw", new AngleLimit(-2.0857f, 2.0857f) },
                { "HeadPitch", new AngleLimit(-0.7068f, 0.6371f) },
                { "LShoulderPitch", new AngleLimit(-2.0857f, 2.0857f) },
                { "LShoulderRoll", new AngleLimit(0.0087f, 1.5620f) },
                { "LElbowYaw", new AngleLimit(-2.0857f, 2.0857f) },
                { "LElbowRoll", new AngleLimit(-1.5620f, -0.0087f) },
                { "LWristYaw", new AngleLimit(-1.8239f, 1.8239f) },
                { "RShoulderPitch", new AngleLimit(-2.0857f, 2.0857f) },
                { "RShoulderRoll", new AngleLimit(-1.5620f, -0.0087f) },
                { "RElbowYaw", new AngleLimit(-2.0857f, 2.0857f) },
                { "RElbowRoll", new AngleLimit(0.0087f, 1.5620f) },
                { "RWristYaw", new AngleLimit(-1.8239f, 1.8239f) },
                { "HipPitch", new AngleLimit(-1.0385f, 1.0385f) },
                { "HipRoll", new AngleLimit(-0.5149f, 0.5149f) },
                { "LHand", new AngleLimit(0.0f, 1.0f) },
                { "RHand", new AngleLimit(0.0f, 1.0f) }
            };
        }


    }


    internal struct AngleLimit
    {
        public AngleLimit(float min, float max) : this()
        {
            _min = min;
            _max = max;
        }

        private float _min;
        private float _max;

        public float Min => _min;
        public float Max => _max;
    }
}
