namespace Metroidvania.Serialization {
    [System.Serializable]
    public struct CharacterSafePointData {
        public string sceneGUID;
        public SerializableGuid pointGUID;
    }

    public partial class GameData {
        public CharacterSafePointData lastCharacterSafePoint;
    }
}