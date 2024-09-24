using Unity.Netcode;
using UnityEngine;

public class Player1Observed : NetworkBehaviour
{
    public NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>();

    void Update()
    {
        // Hanya pemain yang memiliki kontrol (IsOwner) yang boleh menggerakkan Tanko
        if (IsOwner)
        {
            // Sinkronkan posisi hanya jika berubah, dan tambahkan threshold untuk mengurangi bandwidth
            if (Vector3.Distance(playerPosition.Value, transform.position) > 0.01f)
            {
                playerPosition.Value = transform.position;
            }
        }
        else
        {
            // Terima dan terapkan posisi yang disinkronkan dari jaringan
            if (transform.position != playerPosition.Value)
            {
                transform.position = playerPosition.Value;
            }
        }
    }
}
