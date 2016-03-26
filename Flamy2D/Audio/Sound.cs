using Flamy2D.Assets;
using NVorbis;
using OpenTK.Audio.OpenAL;
using System;
using System.IO;

namespace Flamy2D.Audio
{
    public class Sound : IDisposable, IAsset
    {
        const int DefaultBufferCount = 3;

        internal readonly object stopMutex = new object();
        internal readonly object prepareMutex = new object();

        internal readonly int alSourceId;
        internal readonly int[] alBufferIds;

        readonly int alFilterId;
        readonly Stream underlyingStream;

        internal VorbisReader Reader { get; private set; }
        public bool Ready { get; private set; }
        internal bool Preparing { get; private set; }

        public int BufferCount { get; private set; }

        internal EventHandler Finished;

        public Sound(string filename, int bufferCount = DefaultBufferCount) : this(File.OpenRead(filename), bufferCount) { }
        public Sound(Stream stream, int bufferCount = DefaultBufferCount)
        {
            BufferCount = bufferCount;

            alBufferIds = AL.GenBuffers(bufferCount);
            alSourceId = AL.GenSource();

            if (ALHelper.XRam.IsInitialized)
            {
                ALHelper.XRam.SetBufferMode(BufferCount, ref alBufferIds[0], XRamExtension.XRamStorage.Hardware);
                ALHelper.Check();
            }

            Volume = 1;

            if (ALHelper.Efx.IsInitialized)
            {
                alFilterId = ALHelper.Efx.GenFilter();
                ALHelper.Efx.Filter(alFilterId, EfxFilteri.FilterType, (int)EfxFilterType.Lowpass);
                ALHelper.Efx.Filter(alFilterId, EfxFilterf.LowpassGain, 1);
                LowPassHFGain = 1;
            }

            underlyingStream = stream;
        }

        public void Prepare()
        {
            if (Preparing) return;

            var state = AL.GetSourceState(alSourceId);

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
                            Reader.DecodedTime = TimeSpan.Zero;
                            Ready = false;
                            Empty();
                        }
                        break;
                }

                if (!Ready)
                {
                    lock (prepareMutex)
                    {
                        Preparing = true;
                        Open(precache: true);
                    }
                }
            }
        }

        public void Play()
        {
            var state = AL.GetSourceState(alSourceId);

            switch (state)
            {
                case ALSourceState.Playing: return;
                case ALSourceState.Paused:
                    Resume();
                    return;
            }

            Prepare();

            AL.SourcePlay(alSourceId);
            ALHelper.Check();

            Preparing = false;

            AudioDevice.Instance.AddStream(this);
        }

        public void Pause()
        {
            if (AL.GetSourceState(alSourceId) != ALSourceState.Playing)
                return;

            AudioDevice.Instance.RemoveStream(this);
            AL.SourcePause(alSourceId);
            ALHelper.Check();
        }

        public void Resume()
        {
            if (AL.GetSourceState(alSourceId) != ALSourceState.Paused)
                return;

            AudioDevice.Instance.AddStream(this);
            AL.SourcePlay(alSourceId);
            ALHelper.Check();
        }

        public void Stop()
        {
            var state = AL.GetSourceState(alSourceId);
            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
            {
                StopPlayback();
            }

            lock (stopMutex)
            {
                NotifyFinished();
                AudioDevice.Instance.RemoveStream(this);
            }
        }

        float lowPassHfGain;
        public float LowPassHFGain
        {
            get { return lowPassHfGain; }
            set
            {
                if (ALHelper.Efx.IsInitialized)
                {
                    ALHelper.Efx.Filter(alFilterId, EfxFilterf.LowpassGainHF, lowPassHfGain = value);
                    ALHelper.Efx.BindFilterToSource(alSourceId, alFilterId);
                    ALHelper.Check();
                }
            }
        }

        float volume;
        public float Volume
        {
            get { return volume; }
            set
            {
                AL.Source(alSourceId, ALSourcef.Gain, volume = value);
                ALHelper.Check();
            }
        }

        public bool IsLooped { get; set; }

        public void Dispose()
        {
            var state = AL.GetSourceState(alSourceId);
            if (state == ALSourceState.Playing || state == ALSourceState.Paused)
                StopPlayback();

            lock (prepareMutex)
            {
                AudioDevice.Instance.RemoveStream(this);

                if (state != ALSourceState.Initial)
                    Empty();

                Close();

                underlyingStream.Dispose();
            }

            AL.DeleteSource(alSourceId);
            AL.DeleteBuffers(alBufferIds);

            if (ALHelper.Efx.IsInitialized)
                ALHelper.Efx.DeleteFilter(alFilterId);

            ALHelper.Check();
        }

        void StopPlayback()
        {
            AL.SourceStop(alSourceId);
            ALHelper.Check();
        }

        internal void NotifyFinished()
        {
            var callback = Finished;
            if (callback != null)
            {
                callback(this, EventArgs.Empty);
                Finished = null;  // This is not typical...  Usually we count on whatever code added the event handler to also remove it
            }
        }

        void Empty()
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
                catch (InvalidOperationException)
                {
                    // This is a bug in the OpenAL implementation
                    // Salvage what we can
                    int processed;
                    AL.GetSource(alSourceId, ALGetSourcei.BuffersProcessed, out processed);
                    var salvaged = new int[processed];
                    if (processed > 0)
                    {
                        AL.SourceUnqueueBuffers(alSourceId, processed, salvaged);
                        ALHelper.Check();
                    }

                    // Try turning it off again?
                    AL.SourceStop(alSourceId);
                    ALHelper.Check();

                    Empty();
                }
            }
        }

        internal void Open(bool precache = false)
        {
            underlyingStream.Seek(0, SeekOrigin.Begin);
            Reader = new VorbisReader(underlyingStream, false);

            if (precache)
            {
                // Fill first buffer synchronously
                AudioDevice.Instance.FillBuffer(this, alBufferIds[0]);
                AL.SourceQueueBuffer(alSourceId, alBufferIds[0]);
                ALHelper.Check();

                // Schedule the others asynchronously
                AudioDevice.Instance.AddStream(this);
            }

            Ready = true;
        }

        internal void Close()
        {
            if (Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }
            Ready = false;
        }
    }

    
}
