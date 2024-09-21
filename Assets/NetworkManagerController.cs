using UnityEngine;

public class NetworkManagerController : MonoBehaviour
{
    private void Awake()
    {
        // Pastikan NetworkManager tidak dihancurkan saat ganti scene
        DontDestroyOnLoad(gameObject);

        // Hapus instance duplikat jika ada NetworkManager lain yang sudah aktif
        if (FindObjectsOfType<NetworkManagerController>().Length > 1)
        {
            Destroy(gameObject); // Hancurkan duplikat
        }
    }
}
