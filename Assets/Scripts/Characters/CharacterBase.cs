using Metroidvania.Characters.SafePoints;
using Metroidvania.Entities;
using Metroidvania.SceneManagement;
using UnityEngine;

namespace Metroidvania.Characters {
    public abstract class CharacterBase : MonoBehaviour, ISceneTransistor, IEntityHittable {
        public int facingDirection { get; protected set; }

        [SerializeField] private MainCharacterLifeField m_Life;
        public MainCharacterLifeField life => m_Life;

        public abstract void OnTakeHit(EntityHitData hitData);

        public abstract void OnSceneTransition(SceneLoader.SceneTransitionData transitionData);
        public abstract void BeforeUnload(SceneLoader.SceneUnloadData unloadData);

        public void FlipTo(int newFacingDirection) {
            if (facingDirection != newFacingDirection)
                Flip();
        }

        public void Flip() {
            facingDirection *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        protected void FocusCameraOnThis() {
            CameraUtility.vCam.Follow = transform;
            CameraUtility.vCam.PreviousStateIsValid = false;
        }

        protected CharacterSpawnPoint GetSceneSpawnPoint(SceneLoader.SceneTransitionData transitionData) {
            bool isHorizontalDoor = false;
            SceneSpawnPoints spawnPoints = transitionData.currentScene.spawnPoints;
            SceneSpawnPoints.SceneSpawnPoint spawnPoint = spawnPoints.defaultSpawnPoint;

            switch (transitionData.spawnPoint) {
                case SceneLoader.SceneTransitionData.UseGameDataKey:
                case SceneLoader.SceneTransitionData.GameOverKey:
                    CharacterSafePointsArea safePoints = CharacterSafePointsArea.instance;
                    if (safePoints) {
                        CharacterSafePoint safePoint = safePoints.GetSafePoint(transitionData.gameData.lastCharacterSafePoint.pointGUID);
                        spawnPoint.facingRight = safePoint.facingRight;
                        spawnPoint.position = safePoint.position;
                        isHorizontalDoor = false;
                    } else {
                        spawnPoint = spawnPoints.defaultSpawnPoint;
                        isHorizontalDoor = spawnPoint.doFadeWalk;
                    }
                    break;
#if UNITY_EDITOR
                case SceneLoader.SceneTransitionData.EditorInitializationKey:
                    spawnPoint.position = transform.position;
                    spawnPoint.facingRight = facingDirection == 1;
                    isHorizontalDoor = false;
                    break;
#endif
                default:
                    spawnPoints.TryGetSpawnPoint(transitionData.spawnPoint, ref spawnPoint);
                    isHorizontalDoor = spawnPoint.doFadeWalk;
                    break;
            }


            return new CharacterSpawnPoint() {
                position = spawnPoint.position,
                facingToRight = spawnPoint.facingRight,
                isHorizontalDoor = isHorizontalDoor,
            };
        }
    }
}