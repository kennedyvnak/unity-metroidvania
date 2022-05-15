using Metroidvania.Entities;
using Metroidvania.Player;

namespace Metroidvania.Combat
{
    /// <summary>
    /// An interface used to define objects that hit the player on touch
    /// </summary>
    public interface ITouchHit
    {
        /// <summary>Called when the player touch this object</summary>
        /// <param name="playerController">The player touched</param>
        /// <returns>An hit data</returns>
        EntityHitData OnHitPlayer(PlayerController playerController);
    }
}