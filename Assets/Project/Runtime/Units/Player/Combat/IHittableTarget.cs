namespace Metroidvania.Combat
{
    public interface IHittableTarget
    {
        void OnTakeHit(PlayerHitData hitData);
    }
}