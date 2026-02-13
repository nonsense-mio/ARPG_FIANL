
using UnityEngine;

namespace ARPG
{
    public class DestoryAfterCastingSpell : MonoBehaviour
    {
        CharacterManager characterCastingSpell;

        void Awake()
        {
            characterCastingSpell = GetComponentInParent<CharacterManager>();
        }

        void Update()
        {
            if (characterCastingSpell.isFiringSpell)
            {
                Destroy(gameObject);
            }
        }
    }

}
