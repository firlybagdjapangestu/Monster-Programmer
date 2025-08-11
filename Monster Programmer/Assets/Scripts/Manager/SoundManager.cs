using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Manager
{
    public class SoundManager : MonoBehaviour
    {
        [Header("[Reference]")]
        [SerializeField] private AudioSource backgroundSource;
        [SerializeField] private AudioSource[] audioSources = new AudioSource[0];

        [Header("[Clip List]")]
        [SerializeField] private AudioClip[] clipList = new AudioClip[0];

        public static SoundManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void PlaySoundEffect(int index)
        {
            if (index < 0 || index >= clipList.Length)
                return;

            PlaySoundEffect(clipList[index]);
        }

        public void PlaySoundEffect(AudioClip _clip)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (audioSources[i].isPlaying)
                    continue;

                audioSources[i].PlayOneShot(_clip);
                break;
            }
        }

        public void PlayBacksound(AudioClip _clip)
        {
            if (backgroundSource.clip == _clip)
                return;

            backgroundSource.clip = _clip;
            backgroundSource.Play();
            backgroundSource.loop = true;
        }

        public void PauseBacksound(bool pause)
        {
            if (pause)
            {
                backgroundSource.Pause();
                return;
            }

            backgroundSource.UnPause();
        }

        public void StopBacksound()
        {
            backgroundSource.clip = null;
            backgroundSource.Stop();
        }
    }
}


