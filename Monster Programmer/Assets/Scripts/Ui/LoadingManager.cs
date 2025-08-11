using Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ui
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance;

        [Header("UI Loading")]
        public GameObject loadingPanel;
        public Image fadeImage; // Panel gelap dengan Image (alpha hitam)
        public float fadeDuration = 0.5f;

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

        public void LoadingNewScene(string _sceneName)
        {

            Time.timeScale = 1f;
            SoundManager.Instance?.StopBacksound();

            StartCoroutine(LoadSceneAsync(_sceneName));
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            // Tampilkan panel loading dan fade in
            loadingPanel.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f)); // dari transparan → gelap

            // Mulai load scene secara async
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            // Tunggu hingga load hampir selesai
            while (operation.progress < 0.9f)
            {
                yield return null;
            }

            // Aktifkan scene
            operation.allowSceneActivation = true;

            // Tunggu 1 frame agar scene benar-benar aktif
            yield return new WaitForSecondsRealtime(0.4f);

            // Fade out panel
            yield return StartCoroutine(Fade(1f, 0f));
            loadingPanel.SetActive(false);
        }

        IEnumerator Fade(float startAlpha, float endAlpha)
        {
            float elapsed = 0f;
            Color c = fadeImage.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
                c.a = alpha;
                fadeImage.color = c;
                yield return null;
            }

            c.a = endAlpha;
            fadeImage.color = c;
        }
    }
}

