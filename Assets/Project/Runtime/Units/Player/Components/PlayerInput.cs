using Metroidvania.InputSystem;

namespace Metroidvania.Player
{
    public class PlayerInput : PlayerComponent
    {
        public PlayerInput(PlayerController target) : base(target)
        {
            target.Enabled += Enable;
            target.Disabled += Disable;
        }

        public InputReader reader => target.data.inputReader;

        public bool enabled { get; private set; }

        public float horizontalMove { get; private set; }
        public bool virtualJumping { get; private set; }
        public bool virtualCrouching { get; private set; }
        public bool virtualDashing { get; private set; }
        public bool virtualAttacking { get; private set; }

        public override void OnDestroy()
        {
            base.OnDestroy();
            target.Enabled -= Enable;
            target.Disabled -= Disable;
        }

        public void Enable()
        {
            if (enabled)
                return;

            reader.MoveEvent += ReadMove;
            reader.JumpEvent += JumpPerformed;
            reader.JumpCanceledEvent += JumpCanceled;
            reader.CrouchEvent += CrouchPerformed;
            reader.CrouchCanceledEvent += CrouchCanceled;
            reader.DashEvent += DashPerformed;
            reader.DashCanceledEvent += DashCanceled;
            reader.AttackEvent += AttackPerformed;
            reader.AttackCanceledEvent += AttackCanceled;

            enabled = true;
        }

        public void Disable()
        {
            if (!enabled)
                return;

            reader.MoveEvent -= ReadMove;
            reader.JumpEvent -= JumpPerformed;
            reader.JumpCanceledEvent -= JumpCanceled;
            reader.CrouchEvent -= CrouchPerformed;
            reader.CrouchCanceledEvent -= CrouchCanceled;
            reader.DashEvent -= DashPerformed;
            reader.DashCanceledEvent -= DashCanceled;
            reader.AttackEvent -= AttackPerformed;
            reader.AttackCanceledEvent -= AttackCanceled;

            enabled = false;
        }

        private void ReadMove(float move)
        {
            horizontalMove = move;
        }

        private void JumpPerformed()
        {
            virtualJumping = true;
        }

        private void JumpCanceled()
        {
            virtualJumping = false;
        }

        private void CrouchPerformed()
        {
            virtualCrouching = true;
        }

        private void CrouchCanceled()
        {
            virtualCrouching = false;
        }

        private void DashPerformed()
        {
            virtualDashing = true;
        }

        private void DashCanceled()
        {
            virtualDashing = false;
        }

        private void AttackCanceled()
        {
            virtualAttacking = false;
        }

        private void AttackPerformed()
        {
            virtualAttacking = true;
        }
    }
}