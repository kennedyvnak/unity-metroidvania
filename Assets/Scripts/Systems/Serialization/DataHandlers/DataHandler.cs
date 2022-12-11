using System.Text;

namespace Metroidvania.Serialization.Handlers {
    public abstract class DataHandler {
        public const string FileName = "user_{0}";
        public const string FileExtension = "save";

        public const string k_EncryptionKey = "{91F6F631-2E90-467A-9E60-60049ACB2B4F}";

        public abstract GameData Deserialize(int userId);

        public abstract void Serialize(GameData data);

        public abstract void DeleteUser(int userId);

        public static string EncryptDecrypt(string input) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
                sb.Append((char)(input[i] ^ k_EncryptionKey[i % k_EncryptionKey.Length]));
            return sb.ToString();
        }
    }
}