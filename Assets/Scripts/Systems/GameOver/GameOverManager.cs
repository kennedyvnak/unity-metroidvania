using DG.Tweening;
using Metroidvania.Events;
using Metroidvania.InputSystem;
using Metroidvania.RuntimeFields;
using Metroidvania.SceneManagement;
using Metroidvania.Serialization;
using System.Collections;
using UnityEngine;

namespace Metroidvania.GameOver
{
    public class GameOverManager : SingletonPersistent<GameOverManager>
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject m_gameOverScreenPrefab;

        [Header("Scenes")]
        [SerializeField] private AssetReferenceSceneChannel m_gameOverScene;

        [Header("Events")]
        [SerializeField] private VoidEventChannel m_onGameOverChannel;
        [SerializeField] private ObjectEventChannel m_onPlayerDieChannel;

        [Header("Properties")]
        [SerializeField] private RuntimeFloatField m_playerHP;
        [SerializeField] private float m_gameOverScreenTime = 5f;
        [SerializeField] private float m_timeAfterPlayerDie = 2f;
        [SerializeField] private float m_fadeTime = 1f;

        private GameObject _gameOverScreen;

        private void Start()
        {
            _gameOverScreen = Instantiate(m_gameOverScreenPrefab, FadeScreen.instance.canvas.transform);
            _gameOverScreen.SetActive(false);
            m_onPlayerDieChannel.OnEventRaise += OnPlayerDie;
        }

        private void OnPlayerDie(UnityEngine.Object player)
        {
            StartCoroutine(DOPlayerDie());
        }

        private IEnumerator DOPlayerDie()
        {
            yield return CoroutinesUtility.GetYieldSeconds(m_timeAfterPlayerDie);
            yield return DOGameOver();
        }

        public void StartGameOver() => StartCoroutine(DOGameOver());

        private IEnumerator DOGameOver()
        {
            InputReader.instance.DisableAllInput();
            _gameOverScreen.SetActive(true);
            yield return FadeScreen.instance.DOFadeIn(m_fadeTime).WaitForCompletion();
            yield return CoroutinesUtility.GetYieldSeconds(m_gameOverScreenTime);
            m_onGameOverChannel?.Raise();
            GameData gameData = DataManager.instance.gameData;
            gameData.playerLife = m_playerHP.defaultValue;
            yield return SceneLoader.instance.LoadSceneWithoutTransition(m_gameOverScene, SceneLoader.SceneTransitionData.GameOver);
            yield return FadeScreen.instance.DOFadeOut(m_fadeTime).WaitForCompletion();
            _gameOverScreen.SetActive(false);
        }
    }
}
