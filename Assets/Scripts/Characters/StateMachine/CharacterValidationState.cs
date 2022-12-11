namespace Metroidvania.Characters {
    public class CharacterValidationState<TCharacter> : CharacterStateBase<TCharacter> where TCharacter : CharacterBase {
        public CharacterValidationState(CharacterStateMachine<TCharacter> machine)
            : base(machine) { }
    }
}