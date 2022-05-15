using System;

namespace Metroidvania.Player
{
    /// <summary>Base class for player components, not to be confused with UnityEngine.Component</summary>
    public class PlayerComponent
    {
        /// <summary>The target player behaviour</summary>
        public readonly PlayerController target;

        public PlayerComponent(PlayerController target)
        {
            this.target = target ? target : throw new ArgumentNullException(nameof(target));
            target.playerComponents.Add(this);
        }

        /// <summary>Called when the player destroy this component</summary>
        public virtual void OnDestroy()
        {
        }
    }
}