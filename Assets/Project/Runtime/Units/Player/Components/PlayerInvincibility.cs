using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerInvincibility : PlayerComponent
    {
        public PlayerInvincibility(PlayerController target) : base(target)
        {
            target.LogicUpdated += Update;
        }

        public bool isInInvincibility => invincibilityCounter > 0;

        public float invincibilityCounter { get; private set; }

        public override void OnDestroy()
        {
            base.OnDestroy();
            target.LogicUpdated -= Update;
        }

        private void Update()
        {
            if (!isInInvincibility) return;
            invincibilityCounter -= Time.deltaTime;
        }

        public void AddInvincibility(float time)
        {
            invincibilityCounter += time;
        }
    }
}