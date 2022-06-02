using UnityEngine;

namespace Metroidvania.Player
{
    // TODO: Changes the collider based on the current animation
    /// <summary>Player Component used for handle collisions checks</summary>
    public class PlayerCollisions : PlayerComponent
    {
        public PlayerCollisions(PlayerController player) : base(player)
        {
            player.PhysicsUpdate += CollisionsCheck;
        }

        /// <summary>Is the player touching the ground?</summary>
        public bool isGrounded { get; private set; }

        /// <summary>Is the player left hand touching a wall?</summary>
        public bool isTouchingLeftWall { get; private set; }

        /// <summary>Is the player right hand touching a wall?</summary>
        public bool isTouchingRightWall { get; private set; }

        /// <summary>Is the player touching a ledge?</summary>
        public bool isTouchingLedge { get; private set; }

        /// <summary>Is the player touching a wall?</summary>
        public bool isTouchingWall => isTouchingLeftWall || isTouchingRightWall;

        public override void OnDestroy()
        {
            base.OnDestroy();
            player.PhysicsUpdate -= CollisionsCheck;
        }

        /// <summary>Checks the player collisions</summary>
        public void CollisionsCheck()
        {
            var t = player.transform;
            var localScale = (Vector2)t.localScale;
            var position = (Vector2)t.localPosition;

            isGrounded = Physics2D.BoxCast(position + player.data.feetOffset * localScale, player.data.feetRadius, 0,
                Vector2.zero, 0,
                player.data.groundLayer);

            isTouchingLeftWall = Physics2D.BoxCast(position + player.data.leftHandOffset * localScale,
                player.data.leftHandSize, 0,
                Vector2.zero, 0, player.data.wallLayer);
            isTouchingRightWall = Physics2D.BoxCast(position + player.data.rightHandOffset * localScale,
                player.data.rightHandSize, 0,
                Vector2.zero, 0, player.data.wallLayer);

            isTouchingLedge = Physics2D.Raycast(position + player.data.ledgeCheckOffset * localScale,
                new Vector2(player.data.ledgeCheckLength * localScale.x, 0), 0, player.data.wallLayer);
        }
    }
}