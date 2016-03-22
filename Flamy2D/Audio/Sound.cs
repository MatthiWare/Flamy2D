using Flamy2D.Assets;
using NVorbis;
using OpenTK.Audio.OpenAL;
using System;
using System.IO;

namespace Flamy2D.Audio
{
    public class Sound : IDisposable, IAsset
    {
        const int DEFAULT_BUFFER_COUNT = 3;

        private static readonly XRamExtension XRam = new XRamExtension();
        private static readonly EffectsExtension Efx = new EffectsExtension();

        private readonly object stopMutex = new object();
        private readonly object prepareMutex = new object();

        private readonly int alSourceId;
        private readonly int[] alBufferIds;

        private readonly int alFilterId;
        private readonly Stream underlyingStream;

        private VorbisReader reader;
        private bool ready;
        private bool preparing;
        private int bufferCount;

        private float _volume;
        public float Volume
        {
            get { return _volume; }
            set
            {
                AL.Source(alSourceId, ALSourcef.Gain, _volume = value);
                ALHelper.Check();
            }
        }

        public bool IsLoop { get; set; }

        public ALSourceState SourceState { get { return AL.GetSourceState(alSourceId); } }


        private float _lowPassHfGain;
        public float LowPassHFGain
        {
            get { return _lowPassHfGain; }
            set
            {
                if (Efx.IsInitialized)
                {
                    Efx.Filter(alFilterId, EfxFilterf.LowpassGainHF, _lowPassHfGain = value);
                    Efx.BindFilterToSource(alSourceId, alFilterId);
                    ALHelper.Check();
                }
            }
        }

        public Sound(string filename, int bufferCount = DEFAULT_BUFFER_COUNT)
            : this(File.OpenRead(filename), bufferCount)
        { }

        public Sound(Stream stream, int bufferCount = DEFAULT_BUFFER_COUNT)
        {
            underlyingStream = stream;

            this.bufferCount = bufferCount;
            alBufferIds = AL.GenBuffers(bufferCount);
            alSourceId = AL.GenSource();

            Volume = 1f;

            if (XRam.IsInitialized)
            {
                XRam.SetBufferMode(bufferCount, ref alBufferIds[0], XRamExtension.XRamStorage.Hardware);
                ALHelper.Check();
            }

            if (Efx.IsInitialized)
            {
                alFilterId = Efx.GenFilter();
                Efx.Filter(alFilterId, EfxFilteri.FilterType, (int)EfxFilterType.Lowpass);
                Efx.Filter(alFilterId, EfxFilterf.LowpassGain, 1);
                _lowPassHfGain = 1;
            }
        }

        public void Prepare()
        {
            if (preparing)
                return;

            ALSourceState state = SourceState;

            lock (stopMutex)
            {
                switch (state)
                {
                    case ALSourceState.Playing:
                    case ALSourceState.Paused:
                        return;

                    case ALSourceState.Stopped:
                        lock (prepareMutex)
                        {
                            reader.DecodedTime = TimeSpan.Zero;
                            ready = true;
                            Empty();
                        }
                        break;
                }

                if (!ready)
                {
                    lock (prepareMutex)
                    {
                        preparing = true;
                        Open(true);
                    }
                }
            }
        }

        public void Play()
        {
            ALSourceState state = SourceState;

            switch (state)
            {
                case ALSourceState.Playing:
                    return;
                case ALSourceState.Paused:
                    Resume();
                    break;
            }

            Prepare();

            AL.SourcePlay(alSourceId);
            ALHelper.Check();

            preparing = false;
        }

        public void Pause()
        {
            if (SourceState != ALSourceState.Playing)
                return;

            AL.SourcePause(alSourceId);
            ALHelper.Check();
        }

        public void Resume()
        {
            if (SourceState != ALSourceState.Playing)
                return;

            AL.SourcePlay(alSourceId);
            ALHelper.Check();
        }

        public void Stop()
        {
            ALSourceState state = SourceState;
            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
                StopPlayback();

            lock(stopMutex)
            {
                // notify if stopped
            }
        }

        private void StopPlayback()
        {
            AL.SourceStop(alSourceId);
            ALHelper.Check();
        }

        private void Empty()
        {
            int queued;
            AL.GetSource(alSourceId, ALGetSourcei.BuffersQueued, out queued);
            if (queued > 0)
            {
                try
                {
                    AL.SourceUnqueueBuffers(alSourceId, queued);
                    ALHelper.Check();
                }
                catch (ALException)
                {
                    int processed;
                    AL.GetSource(alSourceId, ALGetSourcei.BuffersProcessed, out processed);

                    int[] salvaged = new int[processed];
                    if (processed > 0)
                    {
                        AL.SourceUnqueueBuffers(alSourceId, processed, salvaged);
                        ALHelper.Check();
                    }

                    AL.SourceStop(alSourceId);
                    ALHelper.Check();

                    Empty();
                }
            }
        }

        private void Open(bool precache = false)
        {
            underlyingStream.Seek(0, SeekOrigin.Begin);
            reader = new VorbisReader(underlyingStream, false);

            if (precache)
            {
                AL.SourceQueueBuffer(alSourceId, alBufferIds[0]);
                ALHelper.Check();
            }

            ready = true;
        }

        private void Close()
        {
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            ready = false;
        }

        public void Dispose()
        {
            ALSourceState state = SourceState;
            if(state == ALSourceState.Playing || state == ALSourceState.Paused)
                StopPlayback();

            lock(prepareMutex)
            {
                if (state != ALSourceState.Initial)
                    Empty();

                Close();

                underlyingStream.Dispose();
            }

            AL.DeleteSource(alSourceId);
            AL.DeleteBuffers(alBufferIds);

            if (Efx.IsInitialized)
                Efx.DeleteFilter(alFilterId);

            ALHelper.Check();
        }
    }
}
