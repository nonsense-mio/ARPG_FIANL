using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class IllusionaryWall : MonoBehaviour
    {
        public bool wallHasBeenHit;
        public Material illusionaryWallMaterial;
        public float alpha;
        public float fadeTimer = 2.5f;
        public BoxCollider wallCollider;

        public AudioSource audioSource;
        public AudioClip illusionaryWallSound;
        void Update()
        {
            if(wallHasBeenHit)
            {
                FadeIllusionaryWall();
            }
        }

    
        public void FadeIllusionaryWall()
        {
            alpha = illusionaryWallMaterial.color.a;
            alpha -= Time.deltaTime / fadeTimer;
            Color fadedWallColor = new Color(1,1,1,alpha);
            illusionaryWallMaterial.color = fadedWallColor;
            if(wallCollider.enabled)
            {
                wallCollider.enabled = false;
            }
            if(alpha <= 0)
            {
                Destroy(this);
            }
        }
    }
}

