using System;

namespace Metroidvania.Player
{
    /// <summary>Base class for player components, not to be confused with UnityEngine.Component</summary>
    public class PlayerComponent
    {
        /// <summary>The target player behaviour</summary>
        public readonly PlayerController player;

        public PlayerComponent(PlayerController player)
        {
            this.player = player ? player : throw new ArgumentNullException(nameof(player));
            player.playerComponents.Add(this);
        }

        /// <summary>Called when the player destroy this component</summary>
        public virtual void OnDestroy()
        {
        }
    }
}