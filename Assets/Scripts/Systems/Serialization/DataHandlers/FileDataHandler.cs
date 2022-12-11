using System;
using System.IO;
using UnityEngine;

namespace Metroidvania.Serialization.Handlers {
    public class FileDataHandler : DataHandler {
        public override GameData Deserialize(int userId) {
            GameData data = null;
            string path = GetFilePath(userId);

            if (File.Exists(path)) {
                try {
                    using FileStream stream = new FileStream(path, FileMode.Open);
                    using StreamReader reader = new StreamReader(stream);
                    data = JsonUtility.FromJson<GameData>(EncryptDecrypt(reader.ReadToEnd()));
                    if (GameDebugger.instance.debugSerialization)
                        GameDebugger.Log($"Deserialized data({userId}) at path '{path}'");
                } catch (Exception e) {
                    GameDebugger.LogError($"Catch an error when try to deserialize a data file in {path}\n{e}");
                }
            }
            return data;
        }

        public override void Serialize(GameData data) {
            string path = GetFilePath(data.userId);

            try {
                using FileStream stream = new FileStream(path, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(EncryptDecrypt(JsonUtility.ToJson(data)));
                if (GameDebugger.instance.debugSerialization)
                    GameDebugger.Log($"Serialized data({data.userId}) at path '{path}'");
            } catch (Exception e) {
                GameDebugger.LogError($"Catch an error when try to serialize a data to a file in {path}\n{e}");
            }
        }

        public override void DeleteUser(int userId) {
            File.Delete(GetFilePath(userId));
        }

        public static string GetFilePath(int userId) => Path.Combine(Application.persistentDataPath, $"{string.Format(FileName, userId)}.{FileExtension}");
    }
}