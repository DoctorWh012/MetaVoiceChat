#if FISHNET || UNITY_SERVER
using System;
using System.Collections.Generic;
using MetaVoiceChat.Utils;
using FishNet.Object;
using UnityEngine;
using FishNet;
using FishNet.Transporting;

// A possible optimization is to handle all of the networking in one manager class and batch frames with a single timestamp.
// However, this is complex and benefits are negligible.

namespace MetaVoiceChat.NetProviders.FishNet
{
    [RequireComponent(typeof(MetaVc))]
    public class FishNetNetProvider : NetworkBehaviour, INetProvider
    {
        #region Singleton
        public static FishNetNetProvider LocalPlayerInstance { get; private set; }
        private readonly static List<FishNetNetProvider> instances = new();
        public static IReadOnlyList<FishNetNetProvider> Instances => instances;
        #endregion

        bool INetProvider.IsLocalPlayerDeafened => LocalPlayerInstance.MetaVc.isDeafened;

        public MetaVc MetaVc { get; private set; }

        public override void OnStartClient()
        {
            #region Singleton
            if (IsOwner)
            {
                LocalPlayerInstance = this;
            }

            instances.Add(this);
            #endregion

            static int GetMaxDataBytesPerPacket()
            {
                int bytes = InstanceFinder.TransportManager.GetLowestMTU() - 13;
                bytes -= sizeof(int); // Index
                bytes -= sizeof(double); // Timestamp
                bytes -= sizeof(byte); // Additional latency
                bytes -= sizeof(ushort); // Array length
                return bytes;
            }

            MetaVc = GetComponent<MetaVc>();
            MetaVc.StartClient(this, IsOwner, GetMaxDataBytesPerPacket());
        }

        public override void OnStopClient()
        {
            #region Singleton
            if (IsOwner)
            {
                LocalPlayerInstance = null;
            }

            instances.Remove(this);
            #endregion

            MetaVc.StopClient();
        }

        void INetProvider.RelayFrame(int index, double timestamp, ReadOnlySpan<byte> data)
        {
            var array = FixedLengthArrayPool<byte>.Rent(data.Length);
            data.CopyTo(array);

            float additionalLatency = Time.deltaTime;
            FishNetFrame frame = new(index, timestamp, additionalLatency, array);

            if (IsServerInitialized)
            {
                ObsReceiveFrame(frame);
            }
            else
            {
                ServerRelayFrame(frame);
            }

            FixedLengthArrayPool<byte>.Return(array);
        }

        [ServerRpc]
        private void ServerRelayFrame(FishNetFrame frame, Channel channel = Channel.Unreliable)
        {
            float additionalLatency = frame.additionalLatency + Time.deltaTime;
            frame = new(frame.index, frame.timestamp, additionalLatency, frame.data);
            ObsReceiveFrame(frame);
        }

        // A possible optimization is to use target RPCs and only send filled arrays to clients that are within audible range, and empty arrays to others.
        // Audible range would be determined by the distance between the reciever's position and the sender's audio source position.
        [ObserversRpc(ExcludeOwner = true)]
        private void ObsReceiveFrame(FishNetFrame frame, Channel channel = Channel.Unreliable)
        {
            if (IsServerInitialized)
            {
                // Don't apply server Time.deltaTime to additionalLatency -- this frame did not go over the network again.
                float additionalLatency = frame.additionalLatency - Time.deltaTime;
                MetaVc.ReceiveFrame(frame.index, frame.timestamp, additionalLatency, frame.data);
            }
            else
            {
                MetaVc.ReceiveFrame(frame.index, frame.timestamp, frame.additionalLatency, frame.data);
            }
        }
    }
}
#endif
