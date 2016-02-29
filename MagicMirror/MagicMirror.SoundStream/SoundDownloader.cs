using System;
using System.Threading;
using System.Threading.Tasks;

using NAudio.Wave;
using NAudio.CoreAudioApi;
using Baku.LibqiDotNet;
using Baku.MagicMirror.Plugin;

namespace Baku.MagicMirror.SoundStream
{
    internal class SoundDownloader : IDisposable
    {
        const int SoundSamplingRate = 16000;
        const string ALAudioDeviceServiceName = "ALAudioDevice";
        const string SoundSubscriberServiceName = "MagicMirrorSoundDownloader";

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private IQiSessionProxy _session;
        private IQiServiceProxy _audioDevice;

        internal SoundDownloader()
        {
            //_mmdevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            //_bwp = new BufferedWaveProvider(new WaveFormat(SoundSamplingRate, 16, 1));
            //_wavProvider = new VolumeWaveProvider16(_bwp);

            //_wavPlayer = new WasapiOut(_mmdevice, AudioClientShareMode.Shared, false, 200);
            //_wavPlayer.Init(_wavProvider);
            //wavPlayer.Play();
        }

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

                _session = session;
                _audioDevice = audioDevice;
                IsInitialized = true;
                return true;
            });
        }

        public async Task Run()
        {
            if (!IsInitialized || IsStarted || IsDisposed) return;

            IsStarted = true;

            var mmDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var wavProvider = new BufferedWaveProvider(new WaveFormat(SoundSamplingRate, 16, 1));

            var wavPlayer = new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 200);
            wavPlayer.Init(new VolumeWaveProvider16(wavProvider));
            wavPlayer.Play();

            var objBuilder = QiObjectBuilder.Create();
            objBuilder.AdvertiseMethod(
                "processRemote::v(iimm)",
                (sig, arg) =>
                {
                    byte[] raw = arg[3].GetRaw();
                    wavProvider.AddSamples(raw, 0, raw.Length);
                    return QiValue.Void;
                });

            var _soundSubscriberService = objBuilder.BuildObject();
            uint _soundSubscriberId = _session.RegisterService(SoundSubscriberServiceName, _soundSubscriberService);


            _audioDevice.Call("setClientPreferences",
                new QiString(SoundSubscriberServiceName), new QiInt32(SoundSamplingRate), new QiInt32(3), new QiInt32(0)
                );
            _audioDevice.Call("subscribe", new QiString(SoundSubscriberServiceName));

            await Task.Delay(-1, _cts.Token)
                .ContinueWith(_ => 
                {
                    _audioDevice.Call("unsubscribe", new QiString(SoundSubscriberServiceName));
                    _session.UnregisterService(_soundSubscriberId);
                    wavPlayer.Stop();
                    wavPlayer.Dispose();
                });
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


    }
}
