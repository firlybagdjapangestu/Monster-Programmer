using Manager;
using UnityEngine;


namespace Ui
{
    public class UiSetting : MonoBehaviour
    {
        private void Start()
        {
            NormalizeTime();
        }

        public void PlayButtonSoundEffect()
        {
            SoundManager.Instance?.PlaySoundEffect(0);
        }

        public void NormalizeTime()
        {
            SetGameTime(1f);
        }

        public void SetGameTime(float _time)
        {
            Time.timeScale = _time;
            SoundManager.Instance?.PauseBacksound(_time < 1);
        }

        public void BackToMenu()
        {
            LoadingManager.Instance?.LoadingNewScene("MainMenu");
        }

        public void ChangeSceneTo(string _sceneName)
        {
            LoadingManager.Instance?.LoadingNewScene(_sceneName);
        }

        public void QuitAplication()
        {
            Application.Quit();
        }
    }

}
