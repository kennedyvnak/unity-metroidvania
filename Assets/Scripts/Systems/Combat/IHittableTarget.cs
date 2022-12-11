namespace Metroidvania.Combat {
    /// <summary>
    /// Add a script with this interface to a collider 2D on the layer 'Hittable' to call OnTakeHit() when the character hits the collider
    /// </summary>
    public interface IHittableTarget {
        /// <summary>Called when the character hits the collider attached to this gameObject</summary>
        /// <param name="hitData">The data of the hit</param>
        void OnTakeHit(CharacterHitData hitData);
    }
}