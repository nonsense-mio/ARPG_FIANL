using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class PoisonSurface : MonoBehaviour
    {
        public float poisonBuildUpAmount =7;
        public List<CharacterEffectsManager> charactersInsidePoisonSurfaceList = new List<CharacterEffectsManager>();

        private void OnTriggerEnter(Collider other)
        {
            CharacterEffectsManager character = other.GetComponent<CharacterEffectsManager>();
            if(character != null)
            {
                charactersInsidePoisonSurfaceList.Add(character);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            CharacterEffectsManager character = other.GetComponent<CharacterEffectsManager>();
            if(character != null)
            {
                charactersInsidePoisonSurfaceList.Remove(character);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            foreach(CharacterEffectsManager character in charactersInsidePoisonSurfaceList)
            {
                if(character.isPoisoned)
                    return;
                character.poisonBuildUp += poisonBuildUpAmount * Time.deltaTime;
            }
        }
       
    }
}

