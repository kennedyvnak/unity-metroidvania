using UnityEngine;

namespace Metroidvania.Player
{
    public class PlayerCollisions : PlayerComponent
    {
        public PlayerCollisions(PlayerController target) : base(target)
        {
            target.PhysicsUpdate += CollisionsCheck;
        }

        public bool isGrounded { get; private set; }
        public bool isTouchingLeftWall { get; private set; }
        public bool isTouchingRightWall { get; private set; }
        public bool isTouchingLedge { get; private set; }

        public bool isTouchingWall => isTouchingLeftWall || isTouchingRightWall;

        public override void OnDestroy()
        {
            base.OnDestroy();
            target.PhysicsUpdate -= CollisionsCheck;
        }

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