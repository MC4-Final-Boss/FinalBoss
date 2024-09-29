using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    private static RelayManager _instance;
    public static RelayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RelayManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("RelayManager");
                    _instance = go.AddComponent<RelayManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private async Task AuthenticatePlayer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (AuthenticationException e)
        {
            Debug.LogException(e);
        }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            await AuthenticatePlayer();
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Set up the relay server data
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public async Task<bool> JoinRelay(string joinCode)
    {
        try
        {
            await AuthenticatePlayer();
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            // Set up the relay server data
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            return true;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }

        public async Task<bool> JoinRelayWithRetry(string joinCode, int retries = 3)
    {
        while (retries > 0)
        {
            try
            {
                return await JoinRelay(joinCode);
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"JoinRelay failed: {e.Message}, Retries left: {retries}");
                retries--;
                await Task.Delay(2000); // Tunggu 2 detik sebelum mencoba lagi
            }
        }
        return false;
    }
}