using System;
using UnityEngine;

namespace Metroidvania {
    [Serializable]
    public struct SerializableGuid : ISerializationCallbackReceiver, IEquatable<Guid> {
        private const string k_GuidFormat = "N";

        // Small var name is to decrease JSON serialization size
        [SerializeField] private string str;
        [NonSerialized] private Guid guid;

        public SerializableGuid(Guid guid) {
            this.guid = guid;
            str = null;
        }

        public override bool Equals(object obj) {
            return obj is SerializableGuid guid && Equals(guid);
        }

        public bool Equals(Guid guid) {
            return this.guid.Equals(guid);
        }

        public override int GetHashCode() {
            return HashCode.Combine(str, guid);
        }

        public void OnAfterDeserialize() {
            guid = Guid.TryParseExact(str, k_GuidFormat, out Guid serializedGuid) ? serializedGuid : Guid.Empty;
        }

        public void OnBeforeSerialize() {
            str = guid.ToString(k_GuidFormat);
        }

        public override string ToString() => guid.ToString(k_GuidFormat);

        public static bool operator ==(SerializableGuid a, SerializableGuid b) => a.guid == b.guid;
        public static bool operator !=(SerializableGuid a, SerializableGuid b) => a.guid != b.guid;
        public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);
        public static implicit operator Guid(SerializableGuid serializable) => serializable.guid;
        public static implicit operator SerializableGuid(string serializedGuid) => new SerializableGuid(Guid.Parse(serializedGuid));
        public static implicit operator string(SerializableGuid serializedGuid) => serializedGuid.ToString();
    }
}