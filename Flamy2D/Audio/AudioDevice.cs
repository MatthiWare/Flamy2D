using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Flamy2D.Audio
{
    public sealed class AudioDevice : IDisposable
    {
        const float DefaultUpdateRate = 10;
        const int DefaultBufferSize = 44100;

        static readonly object singletonMutex = new object();

        readonly object iterationMutex = new object();
        readonly object readMutex = new object();

        readonly float[] readSampleBuffer;
        readonly short[] castBuffer;

        readonly HashSet<Sound> streams = new HashSet<Sound>();
        readonly List<Sound> threadLocalStreams = new List<Sound>();

        Thread underlyingThread;
        volatile bool cancelled;

        public float UpdateRate { get; private set; }
        public int BufferSize { get; private set; }

        static AudioDevice instance;
        public static AudioDevice Instance
        {
            get
            {
                lock (singletonMutex)
                {
                    if (instance == null)
                        throw new InvalidOperationException("No instance running");
                    return instance;
                }
            }
            set { lock (singletonMutex) instance = value; }
        }

        /// <summary>
        /// Constructs an AudioDevice that plays ogg files in the background
        /// </summary>
        /// <param name="bufferSize">Buffer size</param>
        /// <param name="updateRate">Number of times per second to update</param>
        /// <param name="internalThread">True to use an internal thread, false to use your own thread, in which case use must call EnsureBuffersFilled periodically</param>
        public AudioDevice(int bufferSize = DefaultBufferSize, float updateRate = DefaultUpdateRate, bool internalThread = true)
        {
            lock (singletonMutex)
            {
                if (instance != null)
                    throw new InvalidOperationException("Already running");

                Instance = this;
                if (internalThread)
                {
                    underlyingThread = new Thread(EnsureBuffersFilled) { Priority = ThreadPriority.Lowest };
                    underlyingThread.Start();
                }
                else
                {
                    // no need for this, user is in charge
                    updateRate = 0;
                }
            }

            UpdateRate = updateRate;
            BufferSize = bufferSize;

            readSampleBuffer = new float[bufferSize];
            castBuffer = new short[bufferSize];
        }

        

        internal bool AddStream(Sound stream)
        {
            lock (iterationMutex)
                return streams.Add(stream);
        }
        internal bool RemoveStream(Sound stream)
        {
            lock (iterationMutex)
                return streams.Remove(stream);
        }

        public bool FillBuffer(Sound stream, int bufferId)
        {
            int readSamples;
            lock (readMutex)
            {
                readSamples = stream.Reader.ReadSamples(readSampleBuffer, 0, BufferSize);
                CastBuffer(readSampleBuffer, castBuffer, readSamples);
            }
            AL.BufferData(bufferId, stream.Reader.Channels == 1 ? ALFormat.Mono16 : ALFormat.Stereo16, castBuffer,
                          readSamples * sizeof(short), stream.Reader.SampleRate);
            ALHelper.Check();

            return readSamples != BufferSize;
        }
        public static void CastBuffer(float[] inBuffer, short[] outBuffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                var temp = (int)(32767f * inBuffer[i]);
                if (temp > short.MaxValue) temp = short.MaxValue;
                else if (temp < short.MinValue) temp = short.MinValue;
                outBuffer[i] = (short)temp;
            }
        }

        public void EnsureBuffersFilled()
        {
            do
            {
                threadLocalStreams.Clear();
                lock (iterationMutex) threadLocalStreams.AddRange(streams);

                foreach (var stream in threadLocalStreams)
                {
                    lock (stream.prepareMutex)
                    {
                        lock (iterationMutex)
                            if (!streams.Contains(stream))
                                continue;

                        bool finished = false;

                        int queued;
                        AL.GetSource(stream.alSourceId, ALGetSourcei.BuffersQueued, out queued);
                        ALHelper.Check();
                        int processed;
                        AL.GetSource(stream.alSourceId, ALGetSourcei.BuffersProcessed, out processed);
                        ALHelper.Check();

                        if (processed == 0 && queued == stream.BufferCount) continue;

                        int[] tempBuffers;
                        if (processed > 0)
                            tempBuffers = AL.SourceUnqueueBuffers(stream.alSourceId, processed);
                        else
                            tempBuffers = stream.alBufferIds.Skip(queued).ToArray();

                        int bufIdx = 0;
                        for (; bufIdx < tempBuffers.Length; bufIdx++)
                        {
                            finished |= FillBuffer(stream, tempBuffers[bufIdx]);

                            if (finished)
                            {
                                if (stream.IsLooped)
                                {
                                    stream.Reader.DecodedTime = TimeSpan.Zero;
                                    if (bufIdx == 0)
                                    {
                                        // we didn't have any buffers left over, so let's start from the beginning on the next cycle...
                                        continue;
                                    }
                                }
                                else
                                {
                                    lock (stream.stopMutex)
                                    {
                                        stream.NotifyFinished();
                                    }
                                    streams.Remove(stream);
                                    break;
                                }
                            }
                        }

                        AL.SourceQueueBuffers(stream.alSourceId, bufIdx, tempBuffers);
                        ALHelper.Check();

                        if (finished && !stream.IsLooped)
                            continue;
                    }

                    lock (stream.stopMutex)
                    {
                        if (stream.Preparing) continue;

                        lock (iterationMutex)
                            if (!streams.Contains(stream))
                                continue;

                        var state = AL.GetSourceState(stream.alSourceId);
                        if (state == ALSourceState.Stopped)
                        {
                            AL.SourcePlay(stream.alSourceId);
                            ALHelper.Check();
                        }
                    }
                }

                if (UpdateRate > 0)
                {
                    Thread.Sleep((int)(1000 / UpdateRate));
                }
            }
            while (underlyingThread != null && !cancelled);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (singletonMutex)
                    {
                        cancelled = true;
                        lock (iterationMutex)
                            streams.Clear();

                        Instance = null;
                        underlyingThread = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AudioDevice() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
