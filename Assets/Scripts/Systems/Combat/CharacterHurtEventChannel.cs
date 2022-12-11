using Metroidvania.Entities;
using Metroidvania.Events;
using UnityEngine;

namespace Metroidvania.Characters {
    [CreateAssetMenu(menuName = "Scriptables/Characters/Hurt Event")]
    public class CharacterHurtEventChannel : EventChannelBase<CharacterBase, EntityHitData> {
    }
}