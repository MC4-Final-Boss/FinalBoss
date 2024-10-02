using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource; 

    public AudioClip levelClear;
    public AudioClip platformMoving;
    public AudioClip exploding;
    public AudioClip walking;
    public AudioClip jumping;

    public AudioClip buttonPress; 

    // Play a one-shot sound effect
    public void PlaySFX(AudioClip audio)
    {
        if (audio != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(audio);
        }
        else
        {
            Debug.LogWarning("AudioClip or AudioSource is missing. Please ensure both are assigned.");
        }
    }

    // Play walking sound (looping)
    public void PlayWalkingSFX()
    {
        if (sfxSource != null && !sfxSource.isPlaying)
        {
            sfxSource.clip = walking;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    // Stop walking sound
    public void StopWalkingSFX()
    {
        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }

    // Play jumping sound
    public void PlayJumpingSFX()
    {
        PlaySFX(jumping);
    }

    // Additional methods for other sound effects
    public void PlayLevelClearSFX()
    {
        PlaySFX(levelClear);
    }

    public void PlayPlatformMovingSFX()
    {
        PlaySFX(platformMoving);
    }

    public void PlayExplodingSFX()
    {
        PlaySFX(exploding);
    }

    public void PlayButtonPressSFX()
    {
        PlaySFX(buttonPress);
    }
}
