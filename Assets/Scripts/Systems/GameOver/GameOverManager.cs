using DG.Tweening;
using Metroidvania.Events;
using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;
using Metroidvania.Serialization;
using System.Collections;
using UnityEngine;

namespace Metroidvania.GameOver {
    public class GameOverManager : SingletonPersistent<GameOverManager> {
        [Header("Prefabs")]
        [SerializeField] private GameObject m_gameOverScreenPrefab;

        [Header("Scenes")]
        [SerializeField] private AssetReferenceSceneChannel m_gameOverScene;

        [Header("Events")]
        [SerializeField] private VoidEventChannel m_onGameOverChannel;
        [UnityEngine.Serialization.FormerlySerializedAs("m_onPlayerDieChannel")]
        [SerializeField] private ObjectEventChannel m_onMainCharDieChannel;

        [Header("Properties")]
        [SerializeField] private float m_gameOverScreenTime = 5f;
        [UnityEngine.Serialization.FormerlySerializedAs("m_timeAfterPlayerDie")]
        [SerializeField] private float m_timeAfterCharacterDie = 2f;
        [SerializeField] private float m_fadeTime = 1f;

        private GameObject _gameOverScreen;

        private void Start() {
            _gameOverScreen = Instantiate(m_gameOverScreenPrefab, FadeScreen.instance.canvas.transform);
            _gameOverScreen.SetActive(false);
            m_onMainCharDieChannel.OnEventRaise += OnMainCharacterDie;
        }

        private void OnMainCharacterDie(UnityEngine.Object character) {
            StartCoroutine(DoCharacterDie());
        }

        private IEnumerator DoCharacterDie() {
            yield return CoroutinesUtility.GetYieldSeconds(m_timeAfterCharacterDie);
            yield return DOGameOver();
        }

        public void StartGameOver() => StartCoroutine(DOGameOver());

        private IEnumerator DOGameOver() {
            InputReader.instance.DisableAllInput();
            _gameOverScreen.SetActive(true);
            yield return FadeScreen.instance.DOFadeIn(m_fadeTime).WaitForCompletion();
            yield return CoroutinesUtility.GetYieldSeconds(m_gameOverScreenTime);
            m_onGameOverChannel?.Raise();
            GameData gameData = DataManager.instance.gameData;
            yield return SceneLoader.instance.LoadSceneWithoutTransition(m_gameOverScene, SceneLoader.SceneTransitionData.GameOver);
            yield return FadeScreen.instance.DOFadeOut(m_fadeTime).WaitForCompletion();
            _gameOverScreen.SetActive(false);
        }
    }
}
