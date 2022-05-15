using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player Component used for handle collisions checks</summary>
    public class PlayerCollisions : PlayerComponent
    {
        public PlayerCollisions(PlayerController target) : base(target)
        {
            target.PhysicsUpdate += CollisionsCheck;
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
            target.PhysicsUpdate -= CollisionsCheck;
        }
        
        /// <summary>Checks the player collisions</summary>
        public void CollisionsCheck()
        {
            var t = target.transform;
            var localScale = (Vector2)t.localScale;
            var position = (Vector2)t.localPosition;

            isGrounded = Physics2D.BoxCast(position + target.data.feetOffset * localScale, target.data.feetRadius, 0,
                Vector2.zero, 0,
                target.data.groundLayer);

            isTouchingLeftWall = Physics2D.BoxCast(position + target.data.leftHandOffset * localScale,
                target.data.leftHandSize, 0,
                Vector2.zero, 0, target.data.wallLayer);
            isTouchingRightWall = Physics2D.BoxCast(position + target.data.rightHandOffset * localScale,
                target.data.rightHandSize, 0,
                Vector2.zero, 0, target.data.wallLayer);

            isTouchingLedge = Physics2D.Raycast(position + target.data.ledgeCheckOffset * localScale,
                new Vector2(target.data.ledgeCheckLenght * localScale.x, 0), 0, target.data.wallLayer);
        }
    }
}