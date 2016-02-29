using System;
using System.Threading;
using System.Threading.Tasks;

using NAudio.Wave;
using Baku.LibqiDotNet;
using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.SoundStream
{
    internal class SoundUploader : IDisposable
    {
        const string ALAudioDeviceServiceName = "ALAudioDevice";
        const int OutputSampleRate = 16000;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private IQiServiceProxy _audioDevice;

        public async Task<bool> TryInitializeAsync(IQiConnectionServiceProxy conn)
        {
            if (IsStarted || IsDisposed) return false;
            if (IsInitialized) return true;

            var session = conn.CurrentSession;
            if (session?.IsConnected != true) return false;

            return await Task.Run(() =>
            {
                //仮想ロボットとかだとALAudioDeviceが無いことに注意
                var audioDevice = session.GetService("ALAudioDevice");
                if (audioDevice.Name != ALAudioDeviceServiceName) return false;

                _audioDevice = audioDevice;
                _audioDevice.Call("setParameter",
                    new QiString("outputSampleRate"),
                    new QiInt32(OutputSampleRate)
                    );

                IsInitialized = true;
                return true;
            });
        }

        public async Task Run()
        {
            if (!IsInitialized || IsStarted || IsDisposed) return;

            IsStarted = true;
            using (var waveIn = CreateWaveIn(_audioDevice))
            {
                waveIn.StartRecording();
                await Task
                    .Delay(-1, _cts.Token)
                    .ContinueWith(_ => waveIn.StopRecording());
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                _cts.Cancel();
                IsDisposed = true;
            }
        }

        
        public bool IsInitialized { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsDisposed { get; private set; }



        private static WaveInEvent CreateWaveIn(IQiServiceProxy alAudioDevice)
        {
            var waveIn = GetFormattedWaveIn();
            waveIn.DataAvailable += (_, e) =>
            {
                //QiByteDataにムダに長いバッファ投げないためにコピーを採用
                byte[] bufferToSend = new byte[e.BytesRecorded];
                Array.Copy(e.Buffer, bufferToSend, e.BytesRecorded);

                alAudioDevice.Post("sendRemoteBufferToOutput",
                    new QiInt32(e.BytesRecorded / 4),
                    new QiByteData(bufferToSend)
                    );
            };
            return waveIn;
        }

        private static WaveInEvent GetFormattedWaveIn()
        {
            return new WaveInEvent()
            {
                BufferMilliseconds = 200,
                WaveFormat = new WaveFormat(OutputSampleRate, 16, 2)
            };
        }

    }
}
