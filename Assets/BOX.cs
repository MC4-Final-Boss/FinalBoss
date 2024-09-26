using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BOX : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
