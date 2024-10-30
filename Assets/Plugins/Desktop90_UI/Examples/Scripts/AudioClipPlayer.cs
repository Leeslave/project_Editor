using UnityEngine;

namespace float_oat.Desktop90.Examples
{
    /// <summary>
    /// Component for the example scene to play the different audioclips in the asset package
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] AudioClips = default;

        private AudioSource AudioSource;

        void Start()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        public void PlayClip(int clipIndex)
        {
            if (clipIndex < 0 || clipIndex >= AudioClips.Length)
            {
                Debug.LogException(new System.ArgumentOutOfRangeException("Clip index " + clipIndex + " is out of range"));
            }
            AudioSource.Stop();
            AudioSource.PlayOneShot(AudioClips[clipIndex]);
        }
    }
}
