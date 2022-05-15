using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for handle player invincibility</summary>
    public class PlayerInvincibility : PlayerComponent
    {
        public PlayerInvincibility(PlayerController target) : base(target)
        {
            target.LogicUpdated += Update;
        }
        
        /// <summary>True if invincibility time is greater than 0</summary>
        public bool isInInvincibility => invincibilityCounter > 0;
        
        /// <summary>Counter of the invincibility</summary>
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
        
        /// <param name="time">Value added to the counter</param>
        public void AddInvincibility(float time)
        {
            invincibilityCounter += time;
        }
    }
}