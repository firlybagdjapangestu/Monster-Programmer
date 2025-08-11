using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Manager
{
    public class VideoManager : MonoBehaviour
    {
        public static VideoManager Instance;

        public VideoPlayer videoPlayer;

        public Button playButton;
        public Button restartButton;

        private VideoClip clip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Hapus jika sudah ada instance lain
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // Tetap hidup di scene berikutnya
        }

        void Start()
        {
            playButton.onClick.AddListener(PlayVideo);
            restartButton.onClick.AddListener(RestartVideo);
        }

        public void ChangeVideo(VideoClip _clip)
        {
            clip = _clip;
            videoPlayer.clip = clip;
        }

        public void StartVideo()
        {
            SoundManager.Instance?.PauseBacksound(true);
            videoPlayer.Play();
        }

        public void PlayVideo()
        {
            SoundManager.Instance?.PauseBacksound(true);

            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Pause();
            }

            SoundManager.Instance?.PlaySoundEffect(0);
        }

        public void StopVideo()
        {
            videoPlayer.Stop();
            SoundManager.Instance?.PauseBacksound(false);
        }

        public void RestartVideo()
        {
            videoPlayer.Stop();          // Hentikan video sepenuhnya
            videoPlayer.time = 0;        // Set waktu ke awal
            videoPlayer.Play();          // Mainkan ulang
            SoundManager.Instance?.PlaySoundEffect(0);

        }
    }
}
