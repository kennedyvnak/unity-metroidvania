using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania
{
    /// <summary>
    /// Usage: Run EvaluateCollisions() on the start of FixedUpdate().
    /// Run CollisionEnter(), CollisionStay() and CollisionExit() on their perspective events.
    /// </summary>
    public class CollisionChecker
    {
        public readonly Dictionary<Collider2D, byte> collisions = new Dictionary<Collider2D, byte>();

        public byte currentCollisions { get; private set; }

        public bool isGrounded => GetBit(0);
        public bool inLeftWall => GetBit(1);
        public bool inRightWall => GetBit(2);
        public bool inCeil => GetBit(3);

        public bool GetBit(int index)
        {
            return (currentCollisions & (1 << index)) != 0;
        }

        public bool CollidingInWall(float dir)
        {
            return (inLeftWall && dir < 0.0f) || (inRightWall && dir > 0.0f);
        }

        public void CollisionEnter(Collision2D other)
        {
            CheckCollision(other);
        }

        public void CollisionStay(Collision2D other)
        {
            CheckCollision(other);
        }

        public void CollisionExit(Collision2D other)
        {
            collisions.Remove(other.collider);
        }

        public void EvaluateCollisions()
        {
            byte val = 0;
            foreach (var pair in collisions)
            {
                val |= pair.Value;
            }
            currentCollisions = val;
        }

        private void CheckCollision(Collision2D other)
        {
            const float k_Bounds = 0.55f;
            byte collisionFlags = 0;
            foreach (var contact in other.contacts)
            {
                var normal = contact.normal;
                bool ground = normal.y > k_Bounds;
                bool leftWall = normal.x > k_Bounds;
                bool rightWall = normal.x < -k_Bounds;
                bool ceil = normal.y < -k_Bounds;
                collisionFlags |= (byte)((ground ? 1 : 0) | (leftWall ? 2 : 0) | (rightWall ? 4 : 0) | (ceil ? 8 : 0));
            }
            collisions[other.collider] = collisionFlags;
        }
    }
}