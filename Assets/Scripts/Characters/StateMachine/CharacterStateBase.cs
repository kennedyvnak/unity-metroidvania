namespace Metroidvania.Characters {
    public class CharacterStateBase<TCharacter> where TCharacter : CharacterBase {
        public readonly CharacterStateMachine<TCharacter> machine;

        public TCharacter character => machine.character;

        public CharacterStateBase(CharacterStateMachine<TCharacter> machine) {
            this.machine = machine;
        }

        public virtual void Enter(CharacterStateBase<TCharacter> previousState) {
        }

        public virtual void Update() {
        }

        public virtual void Exit() {
        }
    }
}