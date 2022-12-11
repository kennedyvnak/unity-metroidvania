using System.Collections;

namespace Metroidvania.UI.Menus {
    public abstract class GameplayMenuInstance : CanvasMenuBase {
        public GameplayMenuChannel channel { get; protected set; }

        public abstract IEnumerator InitOperation(GameplayMenuChannel channel);

        public abstract IEnumerator ReleaseOperation();
    }
}