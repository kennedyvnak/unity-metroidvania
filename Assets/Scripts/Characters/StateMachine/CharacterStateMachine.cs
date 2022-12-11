namespace Metroidvania.Characters {
    public class CharacterStateMachine<TCharacter> where TCharacter : CharacterBase {
        public readonly TCharacter character;

        public CharacterStateBase<TCharacter> currentState { get; private set; }

        public CharacterStateMachine(TCharacter character) {
            this.character = character;
            EnterState(new CharacterValidationState<TCharacter>(this));
        }

        public virtual void EnterState(CharacterStateBase<TCharacter> state) {
            CharacterStateBase<TCharacter> previousState = currentState;
            currentState = state;
            previousState?.Exit();
            state.Enter(previousState);
        }
    }
}