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
            DaggerfallUnity.Instance.StartCoroutine(audioSource.PlayWhenReadyCoroutine(audioClip, volumeScale * DaggerfallUnity.Settings.SoundVolume));
        }

        private static IEnumerator PlayWhenReadyCoroutine(this AudioSource audioSource, AudioClip audioClip, float volume)
        {
            float loadWaitTimer = 0f;
            while (audioClip.loadState == AudioDataLoadState.Unloaded ||
                   audioClip.loadState == AudioDataLoadState.Loading)
            {
                loadWaitTimer += Time.deltaTime;
                if (loadWaitTimer > audioLoopMaxDelay)
                    yield break;
                yield return null;
            }
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
        }

        public static void PlayOneShotWhenReady(this AudioSource audioSource, AudioClip audioClip, float volumeScale)
        {
            DaggerfallUnity.Instance.StartCoroutine(audioSource.PlayOneShotWhenReadyCoroutine(audioClip, volumeScale * DaggerfallUnity.Settings.SoundVolume));
        }

        private static IEnumerator PlayOneShotWhenReadyCoroutine(this AudioSource audioSource, AudioClip audioClip, float volume)
        {
            float loadWaitTimer = 0f;
            while (audioClip.loadState == AudioDataLoadState.Unloaded ||
                   audioClip.loadState == AudioDataLoadState.Loading)
            {
                loadWaitTimer += Time.deltaTime;
                if (loadWaitTimer > audioClipMaxDelay)
                    yield break;
                yield return null;
            }
            audioSource.volume = volume;
            audioSource.PlayOneShot(audioClip);
        }

    }
}
