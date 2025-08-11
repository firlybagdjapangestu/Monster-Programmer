using UnityEngine;
using UnityEngine.Events;


namespace Manager
{
    public class SceneAction : MonoBehaviour
    {
        [Header("[Action]")]
        public UnityEvent OnSceneLoaded;

        private void Start()
        {
            OnSceneLoaded?.Invoke();    
        }

        public void PlayBacksound(AudioClip _clip)
        {
            SoundManager.Instance?.PlayBacksound( _clip);
        }

    }
}

