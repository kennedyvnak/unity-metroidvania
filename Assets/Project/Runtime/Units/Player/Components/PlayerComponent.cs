using System;

namespace Metroidvania.Player
{
    public class PlayerComponent
    {
        public readonly PlayerController target;

        public PlayerComponent(PlayerController target)
        {
            this.target = target ? target : throw new ArgumentNullException(nameof(target));
            target.playerComponents.Add(this);
        }

        public virtual void OnDestroy()
        {
        }
    }
}