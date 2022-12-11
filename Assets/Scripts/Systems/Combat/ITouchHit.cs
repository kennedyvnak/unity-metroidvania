using Metroidvania.Entities;

namespace Metroidvania.Combat {
    /// <summary>An interface used to define objects that hit the character on touch</summary>
    public interface ITouchHit {
        /// <summary>Ignores the character invincibility if true</summary>
        bool ignoreInvincibility { get; }

        /// <summary>Called when the character touch this object</summary>
        /// <param name="characterController">The character touched</param>
        /// <returns>An hit data</returns>
        EntityHitData OnHitCharacter(Characters.CharacterBase characterController);
    }
}