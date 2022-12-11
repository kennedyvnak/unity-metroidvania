using Metroidvania.Characters;

namespace Metroidvania.Combat {
    /// <summary>Used for handle the character's hits</summary>
    public readonly struct CharacterHitData {
        /// <summary>The damage of the attack</summary>
        public readonly int damage;

        /// <summary>The force of the attack, used in knockback</summary>
        public readonly float force;

        /// <summary>The character that attacked</summary>
        public readonly CharacterBase character;

        public CharacterHitData(int damage, float force, CharacterBase character) {
            this.damage = damage;
            this.force = force;
            this.character = character;
        }
    }
}