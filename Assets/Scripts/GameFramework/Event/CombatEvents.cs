using ARPG;
using UnityEngine;

namespace ARPG
{
    public struct SlashEvent
    {
        public CharacterManager Character;
        public string AttackAnimation;
    }

    public struct CharacterDamageEvent
    {
        public Vector3 HitPoint;
    }

    public struct FireBallHitEvent
    {
        public Vector3 HitPoint;
    }

    public struct BombHitEvent
    {
        public Transform BombTransform;
    }

    public struct PerformInteractionEvent
    {
        public Interactable Target;
    }
}
