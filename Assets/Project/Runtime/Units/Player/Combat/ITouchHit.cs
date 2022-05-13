using Metroidvania.Player;

namespace Metroidvania.Combat
{
    public interface ITouchHit
    {
        void OnHitPlayer();
        EntityHitData GetHit(PlayerController playerController);
    }
}