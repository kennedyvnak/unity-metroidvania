namespace Metroidvania.SceneManagement {
    [System.Serializable]
    public class SceneLoadTransition {
        public AssetReferenceSceneChannel channel;
        public string spawnPoint;

        public SceneLoader.SceneTransitionData CreateData() {
            return SceneLoader.SceneTransitionData.FromSpawnPoint(spawnPoint);
        }
    }
}