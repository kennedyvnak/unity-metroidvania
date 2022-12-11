namespace Metroidvania.SceneManagement {
    public interface ISceneTransistor {
        void OnSceneTransition(SceneLoader.SceneTransitionData transitionData);
        void BeforeUnload(SceneLoader.SceneUnloadData unloadData);
    }
}