using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionCaptureAudio.Controller
{
    /// <summary>
    /// 音楽再生を行う為のインターフェースです。
    /// </summary>
    interface IAudioPlayer : IDisposable
    {
        /// <summary>
        /// 再生を一時停止します。
        /// </summary>
        void Pause();

        /// <summary>
        /// 再生を開始します。
        /// </summary>
        void Play();

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        void Stop();

        /// <summary>
        /// 再生位置の変更を行える事を示す値を取得します。
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// 再生位置を時間単位で取得または設定します。
        /// </summary>
        TimeSpan CurrentTime { get; set; }

        /// <summary>
        /// 演奏時間を取得します。
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// 演奏残り時間を取得します。
        /// </summary>
        TimeSpan RemainedTime { get; }

        /// <summary>
        /// 音楽再生の状態を取得します。
        /// </summary>
        PlayState PlayState { get; }

        /// <summary>
        /// 音量を取得または設定します。
        /// </summary>
        float Volume { get; set; }
    }

    /// <summary>
    /// 音楽再生の状態を表します。
    /// </summary>
    public enum PlayState
    {
        /// <summary>
        /// 再生中。
        /// </summary>
        Playing,

        /// <summary>
        /// 一時停止。
        /// </summary>
        Paused,

        /// <summary>
        /// 停止。
        /// </summary>
        Stopped
    }
}
