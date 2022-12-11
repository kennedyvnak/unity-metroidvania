using Metroidvania.Events;
using UnityEngine;

namespace Metroidvania.SceneManagement {
    [CreateAssetMenu(menuName = "Scriptables/Scene Management/Scene Event")]
    public class SceneEventChannel : EventChannelBase<SceneChannel> {
    }
}