namespace Metroidvania.Combat
{
    public readonly struct PlayerHitData
    {
        public readonly int damage;
        public readonly float force;

        public PlayerHitData(int damage, float force)
        {
            this.damage = damage;
            this.force = force;
        }
    }
}