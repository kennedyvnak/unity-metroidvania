using Metroidvania.Entities;
using Metroidvania.SceneManagement;

namespace Metroidvania.Characters.Archer {
    public class ArcherCharacterController : CharacterBase, ISceneTransistor, IEntityHittable {
        public override void BeforeUnload(SceneLoader.SceneUnloadData unloadData) {
            throw new System.NotImplementedException();
        }

        public override void OnSceneTransition(SceneLoader.SceneTransitionData transitionData) {
            throw new System.NotImplementedException();
        }

        public override void OnTakeHit(EntityHitData hitData) {
            throw new System.NotImplementedException();
        }
    }
}