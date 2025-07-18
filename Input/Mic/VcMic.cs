using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MetaVoiceChat.Input.Mic
{
    public class VcMic : IDisposable
    {
        private readonly MonoBehaviour coroutineProvider;
        private readonly int samplesPerFrame;

        public bool IsRecording { get; private set; } = false;

        public AudioClip AudioClip { get; private set; }

        public string[] Devices => Microphone.devices;
        public string SelectedDevice { get; private set; } = null;
        public string ActiveDevice { get; private set; } = null;

        private int nextFrameIndex = 0;
        private int NextFrameIndex => nextFrameIndex++;

        private Coroutine recordCoroutine;

        public event Action<int, float[]> OnFrameReady;
        public event Action<string> OnActiveDeviceChanged;

        public VcMic(MonoBehaviour coroutineProvider, int samplesPerFrame)
        {
            this.coroutineProvider = coroutineProvider;
            this.samplesPerFrame = samplesPerFrame;
        }

        public void SetSelectedDevice(string device)
        {
            if (device == SelectedDevice)
            {
                return;
            }

            SelectedDevice = device;

            if (IsRecording)
            {
                StartRecording();
            }
        }

        public bool StartRecording()
        {
            StopRecording();

            if (Devices.Length <= 0)
            {
                Debug.LogWarning("No microphone detected for voice chat!");
                return false;
            }

            if (!Devices.Contains(SelectedDevice))
            {
                ActiveDevice = Devices[0];
            }
            else
            {
                ActiveDevice = SelectedDevice;
            }

            OnActiveDeviceChanged?.Invoke(ActiveDevice);

            AudioClip = Microphone.Start(ActiveDevice, true, VcConfig.ClipLoopSeconds, VcConfig.SamplesPerSecond);

            if (AudioClip == null)
            {
                Debug.LogWarning("Microphone failed to start recording for voice chat!");

                StopRecording();
                return false;
            }

            if (AudioClip.channels != 1)
            {
                Debug.LogWarning("Microphone must have exactly one channel for voice chat!");

                StopRecording();
                return false;
            }

            recordCoroutine = coroutineProvider.StartCoroutine(CoRecord());

            IsRecording = true;

            return true;
        }

        public void StopRecording()
        {
            if (recordCoroutine != null)
            {
                coroutineProvider.StopCoroutine(recordCoroutine);
                recordCoroutine = null;
            }

            IsRecording = false;

            if (Microphone.IsRecording(ActiveDevice))
            {
                Microphone.End(ActiveDevice);
            }

            UnityEngine.Object.Destroy(AudioClip);
            AudioClip = null;

            if (ActiveDevice != null)
            {
                ActiveDevice = null;
                OnActiveDeviceChanged?.Invoke(ActiveDevice);
            }
        }

        private IEnumerator CoRecord()
        {
            int i = 0;
            int readAbsPos = 0;
            int prevPos = 0;
            float[] samples = new float[samplesPerFrame];

            while (AudioClip != null && Microphone.IsRecording(ActiveDevice))
            {
                bool isNewDataAvailable = true;

                while (isNewDataAvailable)
                {
                    int currPos = Microphone.GetPosition(ActiveDevice);
                    if (currPos < prevPos)
                    {
                        i++;
                    }

                    prevPos = currPos;

                    int currAbsPos = i * AudioClip.samples + currPos;
                    int nextReadAbsPos = readAbsPos + samples.Length;

                    if (nextReadAbsPos < currAbsPos)
                    {
                        // A possible optimization is to allocate a larger fixed sized pooled array
                        // Allocate the array size by the number of samples that are ready to read
                        // Read these all at once instead of using multiple AudioClip.GetData() calls

                        int offsetSamples = readAbsPos % AudioClip.samples;
                        AudioClip.GetData(samples, offsetSamples);

                        int index = NextFrameIndex;
                        OnFrameReady?.Invoke(index, samples);

                        readAbsPos = nextReadAbsPos;
                        isNewDataAvailable = true;
                    }
                    else
                    {
                        isNewDataAvailable = false;
                    }
                }

                yield return null;
            }

            StopRecording();
        }

        public void Dispose()
        {
            StopRecording();
        }
    }
}