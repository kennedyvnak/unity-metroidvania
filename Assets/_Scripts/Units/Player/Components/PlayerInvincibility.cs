using System.Collections;
using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for handle player invincibility</summary>
    public class PlayerInvincibility : PlayerComponent
    {
        /// <summary>True if the player has any invincibility</summary>
        public bool isInvincible => _invincibilityCount > 0;

        /// <summary>The count of invincibility</summary>
        private int _invincibilityCount;

        /// <summary>The count of animations</summary>
        private int _animationsCount;

        /// <summary>Animation coroutine to create the fade</summary>
        private Coroutine _animationCoroutine;

        public PlayerInvincibility(PlayerController player) : base(player)
        {
        }

        /// <summary>
        /// Add invincibility to the player
        /// </summary>
        /// <param name="time">The time of the invincibility</param>
        /// <param name="shouldAnim">If true, the player fade while the invincibility is active</param>
        public void AddInvincibility(float time, bool shouldAnim)
        {
            player.StartCoroutine(StartInvincibility(time, shouldAnim));
        }

        private IEnumerator StartInvincibility(float time, bool shouldAnim)
        {
            if (shouldAnim) _animationsCount++;
            _invincibilityCount++;

            if (_animationsCount > 0 && _animationCoroutine == null)
                _animationCoroutine = player.StartCoroutine(StartAnimation());

            yield return CoroutinesUtility.GetYieldSeconds(time);

            _invincibilityCount--;
            if (shouldAnim) _animationsCount--;
        }

        private IEnumerator StartAnimation()
        {
            float elapsedTime = 0;
            while (_animationsCount > 0)
            {
                elapsedTime += Time.deltaTime * player.data.invincibilityFadeSpeed;
                player.animator.graphic.SetAlpha(1 - Mathf.PingPong(elapsedTime, player.data.invincibilityAlphaChange));
                yield return null;
            }

            player.animator.graphic.SetAlpha(1);
            _animationCoroutine = null;
        }
    }
}