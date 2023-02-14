using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BoschingMachine
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] Signal playerDeadSignal;
        [SerializeField] CanvasGroup cGroup;
        [SerializeField] float fadeTime;

        void Start ()
        {
            playerDeadSignal.RaiseEvent += OnPlayerDead;

            cGroup.blocksRaycasts = false;
            cGroup.interactable = false;
            cGroup.alpha = 0.0f;
        }

        private void OnDestroy()
        {
            playerDeadSignal.RaiseEvent -= OnPlayerDead;
        }

        private void OnPlayerDead(object sender, System.EventArgs e)
        {
            StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            yield return new WaitForSeconds(1.0f);

            cGroup.blocksRaycasts = true;
            cGroup.interactable = true;
            float p = 0.0f;
            while (p < 1.0f)
            {
                cGroup.alpha = p;

                p += Time.deltaTime / fadeTime;
                yield return null;
            }
        }

        public void OnTryAgain ()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnQuit ()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
