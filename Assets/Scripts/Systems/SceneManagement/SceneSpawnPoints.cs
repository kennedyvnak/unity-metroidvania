using System;
using UnityEngine;

namespace Metroidvania.SceneManagement {
    [System.Serializable]
    public class SceneSpawnPoints {
        [System.Serializable]
        public struct SceneSpawnPoint {
            public string key;
            public Vector2 position;
            public bool facingRight;
            public bool doFadeWalk;
        }

        public SceneSpawnPoint defaultSpawnPoint = new SceneSpawnPoint() { key = "default" };

        public SceneSpawnPoint[] spawnPoints;

        private SceneSpawnPoint FindSpawnPoint(string key) {
            foreach (SceneSpawnPoint spawnPoint in spawnPoints)
                if (spawnPoint.key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return spawnPoint;
            return defaultSpawnPoint;
        }

        public void TryGetSpawnPoint(string key, ref SceneSpawnPoint spawnPoint) {
            SceneSpawnPoint sp = FindSpawnPoint(key);
            if (!sp.key.Equals("default"))
                spawnPoint = sp;
        }
    }
}