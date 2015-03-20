using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace MotionCaptureAudio.Controller
{
    /// <summary>
    /// Bass.Net を利用して音楽再生を行います。
    /// </summary>
    public class AudioPlayer : IAudioPlayer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AudioPlayer()
        {
            this.PlayState = PlayState.Stopped;
        }

        public BASS_DEVICEINFO[] GetDevice()
        {
            try
            {
                return Bass.BASS_GetDeviceInfos();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="deviceIndex">出力デバイスのインデックス。</param>
        /// <param name="fileName">音楽ファイルのパス。</param>
        /// <exception cref="FileNotFoundException">音楽ファイルが存在しない。</exception>
        /// <exception cref="Exception">ストリームの生成に失敗した。</exception>
        public Result InitializeInstance(int deviceIndex, string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            try
            {
                this.initializeDevice(deviceIndex);
                this.createStream(fileName);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }

            if (this._handle == 0)
            {
                var error = Bass.BASS_ErrorGetCode();
                throw new Exception("ストリームの生成に失敗しました。\nError : " + error.ToString());
            }

            // 演奏時間の算出
            long length = Bass.BASS_ChannelGetLength(this._handle);
            double seconds = Bass.BASS_ChannelBytes2Seconds(this._handle, length);
            this.Duration = TimeSpan.FromSeconds(seconds);
            this.PlayState = PlayState.Stopped;
            return Result.OK;
        }

        /// <summary>
        /// Bass.Net を解放します。
        /// オーディオ再生が実行されている場合は停止されます。
        /// このメソッドを呼び出した場合、再度 BassInitialize メソッドを呼び出すまで Bass.Net は利用できません。
        /// </summary>
        public static void BassFree()
        {
            if (!AudioPlayer.IsBassInitialized) { return; }
            Bass.BASS_Stop();
            Bass.BASS_PluginFree(0);
            Bass.FreeMe();
            AudioPlayer.IsBassInitialized = false;
        }

        /// <summary>
        /// Bass.Net を初期化します。
        /// </summary>
        /// <param name="folderPath">BASS ライブラリのモジュールを格納したフォルダを示すパス文字列。</param>
        /// <exception cref="Exception">初期化に失敗した。</exception>
        public static void BassInitialize(string folderPath)
        {
            if (AudioPlayer.IsBassInitialized) { return; }

             //Bass.Net
            if (!Bass.LoadMe(folderPath))
            {
                throw new Exception("Bass.Net の初期化に失敗しました。");
            }

            // デバイス
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                var error = Bass.BASS_ErrorGetCode();
                throw new Exception("デバイスの初期化に失敗しました。\nError : " + error.ToString());
            }

             //プラグイン
            {
                var plugins = Bass.BASS_PluginLoadDirectory(folderPath);
                AudioPlayer.FileFilter = Utils.BASSAddOnGetPluginFileFilter(plugins, null);
            }

            AudioPlayer.IsBassInitialized = true;
        }

        /// <summary>
        /// ファイル ダイアログに指定する為のフィルタ文字列を取得します。
        /// </summary>
        public static string FileFilter { get; private set; }

        /// <summary>
        /// Bass.Net の初期化を終えている事を示す値を取得または設定します。
        /// </summary>
        private static bool IsBassInitialized { get; set; }

        #region IAudioPlayer メンバ

        /// <summary>
        /// 再生を一時停止します。
        /// </summary>
        public void Pause()
        {
            if (this.PlayState == PlayState.Playing)
            {
                this.PlayState = PlayState.Paused;
                Bass.BASS_ChannelPause(this._handle);
            }
        }

        /// <summary>
        /// 再生を開始します。
        /// </summary>
        public void Play()
        {
            if (this.PlayState != PlayState.Playing)
            {
                var restart = this.PlayState == PlayState.Paused;
                this.PlayState = PlayState.Playing;

                Bass.BASS_ChannelPlay(this._handle, restart);
            }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop()
        {
            if (this.PlayState != PlayState.Stopped)
            {
                this.PlayState = PlayState.Stopped;
                Bass.BASS_ChannelStop(this._handle);
                Bass.BASS_ChannelSetPosition(this._handle, 0.0);
            }
        }

        /// <summary>
        /// 再生位置の変更を行える事を示す値を取得します。
        /// </summary>
        public bool CanSeek { get { return true; } }

        /// <summary>
        /// 再生位置を時間単位で取得または設定します。
        /// </summary>
        public TimeSpan CurrentTime
        {
            get
            {
                var position = Bass.BASS_ChannelGetPosition(this._handle);
                return TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(this._handle, position));
            }
            set
            {
                var position = Bass.BASS_ChannelSeconds2Bytes(this._handle, value.TotalSeconds);
                Bass.BASS_ChannelSetPosition(this._handle, position);
            }
        }

        /// <summary>
        /// 演奏時間を取得します。
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// 演奏残り時間を取得します。
        /// </summary>
        public TimeSpan RemainedTime
        {
            get
            {
                return this.Duration - this.CurrentTime;
            }
        }

        /// <summary>
        /// 音楽再生の状態を取得します。
        /// </summary>
        public PlayState PlayState { get; private set; }

        /// <summary>
        /// 音量を取得または設定します。
        /// </summary>
        public float Volume
        {
            get
            {
                return this._volume;
            }
            set
            {
                this._volume = value;
                Bass.BASS_ChannelSetAttribute(this._handle, BASSAttribute.BASS_ATTRIB_VOL, value);
            }
        }

        #endregion

        #region IDisposable メンバ

        /// <summary>
        /// リソースの解放を行います。
        /// </summary>
        public void Dispose()
        {
            if (this._handle != 0)
            {
                this.Stop();
                Bass.BASS_StreamFree(this._handle);
                this._handle = 0;
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// オーディオ再生を行う為のストリームのハンドル。
        /// </summary>
        private int _handle;

        /// <summary>
        /// 音量。
        /// </summary>
        private float _volume = 0.4f;

        /// <summary>
        /// 再生ファイル
        /// </summary>
        private string _fileName = string.Empty;

        /// <summary>
        /// 再生ファイル
        /// </summary>
        private int _device = -1;

        #endregion

        /// <summary>
        /// 出力デバイスを初期化します。
        /// </summary>
        /// <param name="deviceIndex">出力デバイスのインデックス。</param>
        private void initializeDevice(int deviceIndex)
        {
            if (this._device == deviceIndex) return;
            this._device = deviceIndex;
            Bass.BASS_SetDevice(this._device);
            Bass.BASS_Init(this._device, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }

        private void createStream(string fileName)
        {
            if (this._fileName == fileName) return;
            this._fileName = fileName;
            this._handle = Bass.BASS_StreamCreateFile(this._fileName, 0L, 0L, BASSFlag.BASS_DEFAULT);
        }
    }
}
