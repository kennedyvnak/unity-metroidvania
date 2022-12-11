using UnityEngine;

namespace Metroidvania {
    public abstract class StaticInstance<T> : MonoBehaviour where T : StaticInstance<T> {
        public static T instance { get; private set; }

        protected virtual void Awake() => instance = this as T;

        protected virtual void OnDestroy() {
            if (instance == (this as T))
                instance = null;
        }
    }

    public abstract class Singleton<T> : StaticInstance<T> where T : Singleton<T> {
        protected override void Awake() {
            if (instance)
                Destroy(gameObject);
            base.Awake();
        }
    }

    public abstract class SingletonPersistent<T> : Singleton<T> where T : SingletonPersistent<T> {
        protected override void Awake() {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            name = $"[{typeof(T).Name}]";
        }
    }
}