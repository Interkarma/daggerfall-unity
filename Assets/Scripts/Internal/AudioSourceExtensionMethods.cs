using System;
using System.Collections;
using DaggerfallWorkshop;

namespace UnityEngine
{
    public static class AudioSourceExtensionMethods
    {
        private const float audioClipMaxDelay = 0.150f; //give up if sound takes longer to load
        private const float audioLoopMaxDelay = 2.0f; // don't care as much about loops accuracy

        public static void PlayWhenReady(this AudioSource audioSource, AudioClip audioClip, float volumeScale)
        {
            DaggerfallUnity.Instance.StartCoroutine(WhenClipReadyCoroutine(audioClip, audioLoopMaxDelay,
                clip =>
                {
                    audioSource.clip = clip;
                    audioSource.volume = volumeScale * DaggerfallUnity.Settings.SoundVolume;
                    audioSource.Play();
                }));
        }

        public static void PlayOneShotWhenReady(this AudioSource audioSource, AudioClip audioClip, float volumeScale)
        {
            DaggerfallUnity.Instance.StartCoroutine(WhenClipReadyCoroutine(audioClip, audioClipMaxDelay,
                clip =>
                {
                    audioSource.volume = volumeScale * DaggerfallUnity.Settings.SoundVolume;
                    audioSource.PlayOneShot(clip);
                }));
        }

        public static void PlayClipAtPointWhenReady(AudioClip audioClip, Vector3 position, float volumeScale)
        {
            DaggerfallUnity.Instance.StartCoroutine(WhenClipReadyCoroutine(audioClip, audioClipMaxDelay,
                clip => AudioSource.PlayClipAtPoint(clip, position, volumeScale * DaggerfallUnity.Settings.SoundVolume)));
        }

        private static IEnumerator WhenClipReadyCoroutine(AudioClip audioClip, float maxDelay, Action<AudioClip> clipHandler)
        {
            float loadWaitTimer = 0f;
            while (audioClip.loadState == AudioDataLoadState.Unloaded ||
                   audioClip.loadState == AudioDataLoadState.Loading)
            {
                loadWaitTimer += Time.unscaledDeltaTime;
                if (loadWaitTimer > maxDelay)
                    yield break;
                yield return null;
            }
            clipHandler(audioClip);
        }
    }
}
