using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player Component used for handle collisions checks</summary>
    public class PlayerCollisions : PlayerComponent
    {
        public readonly Transform transform;
        public readonly BoxCollider2D colliderBehaviour;

        public PlayerDataChannel.ColliderData collisionDataSource { get; private set; }

        /// <summary>Is the player touching the ground?</summary>
        public bool isGrounded { get; private set; }

        /// <summary>Is the player left hand touching a wall?</summary>
        public bool isTouchingLeftWall { get; private set; }

        /// <summary>Is the player right hand touching a wall?</summary>
        public bool isTouchingRightWall { get; private set; }

        public bool canStand { get; private set; }

        /// <summary>Is the player touching a ledge?</summary>
        public bool isTouchingLedge { get; private set; }

        /// <summary>Is the player touching a wall?</summary>
        public bool isTouchingWall => isTouchingLeftWall || isTouchingRightWall;

        public PlayerCollisions(PlayerController player) : base(player)
        {
            transform = player.transform;
            colliderBehaviour = transform.GetComponent<BoxCollider2D>();
            player.PhysicsUpdate += CollisionsCheck;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            player.PhysicsUpdate -= CollisionsCheck;
        }

        /// <summary>Checks the player collisions</summary>
        public void CollisionsCheck()
        {
            var localScale = (Vector2)transform.localScale;
            var position = (Vector2)transform.localPosition;

            isGrounded = OverlapBoxOnGround(collisionDataSource.feetRect);
            isTouchingLeftWall = OverlapBoxOnGround(collisionDataSource.leftHandRect);
            isTouchingRightWall = OverlapBoxOnGround(collisionDataSource.rightHandRect);
            canStand = !OverlapBoxOnGround(player.data.crouchHeadRect);

            // isTouchingLedge = Physics2D.Raycast(position + collisionDataSource.ledgeCheckOffset,
            //     new Vector2(collisionDataSource.ledgeCheckLength * localScale.x, 0), 0, player.data.wallLayer);
        }

        public void SetCollisionsData(PlayerDataChannel.ColliderData colliderData)
        {
            collisionDataSource = colliderData;
            colliderBehaviour.offset = colliderData.bounds.min;
            colliderBehaviour.size = colliderData.bounds.size;
            CollisionsCheck();
        }

        public Collider2D OverlapBoxOnGround(Rect bounds)
        {
            return Physics2D.OverlapBox((Vector2)transform.position + bounds.position * (Vector2)transform.localScale, bounds.size, 0, player.data.groundLayer);
        }
    }
}