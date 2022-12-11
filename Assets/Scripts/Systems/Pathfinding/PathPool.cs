using UnityEngine.Pool;

namespace Metroidvania.Pathfinding {
    public class PathPool {
        private ObjectPool<Path> _pool;

        public PathPool() {
            _pool = new ObjectPool<Path>(() => new Path());
        }

        public Path Get() => _pool.Get();

        public void Release(ref Path p) {
            _pool.Release(p);
            p = null;
        }
    }
}