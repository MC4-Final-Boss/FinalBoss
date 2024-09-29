using Unity.Netcode;
using UnityEngine;

public class NetworkTimeSyncHandler : NetworkBehaviour
{
    [SerializeField] private float interpolationBackTime = 0.1f; // 100ms, adjust as needed
    [SerializeField] private float timeSyncInterval = 1f; // Sync every second

    private NetworkVariable<double> m_NetworkTime = new NetworkVariable<double>();
    private double m_LastServerTimeSent;
    private double m_ClientTimeOffset;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            m_NetworkTime.Value = NetworkManager.ServerTime.Time;
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateServerTime();
        }
        else
        {
            UpdateClientTime();
        }
    }

    private void UpdateServerTime()
    {
        if (NetworkManager.ServerTime.Time - m_LastServerTimeSent > timeSyncInterval)
        {
            m_NetworkTime.Value = NetworkManager.ServerTime.Time;
            m_LastServerTimeSent = NetworkManager.ServerTime.Time;
        }
    }

    private void UpdateClientTime()
    {
        // Recalculate client time offset
        m_ClientTimeOffset = m_NetworkTime.Value - NetworkManager.LocalTime.Time + interpolationBackTime;
    }

    // Use this method to get the current network time for gameplay logic
    public double GetNetworkTime()
    {
        if (IsServer)
        {
            return NetworkManager.ServerTime.Time;
        }
        else
        {
            return NetworkManager.LocalTime.Time + m_ClientTimeOffset;
        }
    }

    // Use this to get the server time on clients, accounting for interpolation
    public double GetInterpolatedServerTime()
    {
        return GetNetworkTime() - interpolationBackTime;
    }
}