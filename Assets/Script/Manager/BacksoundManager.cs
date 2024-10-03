using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacksoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    public AudioClip titleScreen;
    public AudioClip bustlingCityLevel;


    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        PlayBGM(titleScreen);
    }



    public void PlayBGM(AudioClip bgmClip, bool loop = true)
    {
        if (bgmClip != null && bgmSource != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip or AudioSource is missing. Please ensure both are assigned.");
        }
    }

    public void StopBGM(bool fadeOut = false)
    {
        if (bgmSource.isPlaying)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeOutAndStop());
            }
            else
            {
                bgmSource.Stop();
            }
        }
    }

    private IEnumerator FadeOutAndStop()
    {
        float startVolume = bgmSource.volume;

        // Gradually reduce the volume to 0
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        // Stop the music and reset the volume to its original level
        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }

    public void FadeInBGM(AudioClip bgmClip, bool loop = true)
    {
        StartCoroutine(FadeIn(bgmClip, loop));
    }

    private IEnumerator FadeIn(AudioClip bgmClip, bool loop)
    {
        PlayBGM(bgmClip, loop);

        bgmSource.volume = 0;
        float targetVolume = 1f;

        // Gradually increase the volume to its target level
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }


    public void SwitchBGM(AudioClip newClip)
    {
        if (bgmSource.clip != newClip)
        {
            StartCoroutine(SwitchBGMWithFade(newClip));
        }
    }

    private IEnumerator SwitchBGMWithFade(AudioClip newClip)
    {
        // Fade out current BGM
        yield return StartCoroutine(FadeOutAndStop());

        // Fade in new BGM
        FadeInBGM(newClip);
    }


    // public void SetVolume(float volume)
    // {
    //     bgmSource.volume = Mathf.Clamp(volume, 0f, 1f);
    // }


}
