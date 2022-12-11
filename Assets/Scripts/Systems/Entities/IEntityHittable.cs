namespace Metroidvania.Entities {
    public interface IEntityHittable {
        void OnTakeHit(EntityHitData hitData);
    }
}