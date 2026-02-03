using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        protected CharacterManager character;
        public Vector3 moveDirection;
        public LayerMask groundLayer;

        [Header("重力设置")]
        public float inAirTimer;
        [SerializeField] protected Vector3 yVelocity;
        [SerializeField] protected float groundedYVelocity = -10;
        [SerializeField] protected float fallStartYVelocity = -7;
        [SerializeField] protected float gravityForce = -20;
        [SerializeField] float groundCheckSphereRadius = 1f;
        protected bool fallingVelocitySet = false;

        public virtual void Init(CharacterManager characterMgr)
        {
            character = characterMgr;
        }
        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if (character.isDead)
                return;
            HandleGroundCheck();
        }



        public virtual void HandleGroundCheck()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
            if (character.isGrounded)
            {
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocitySet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                if (!fallingVelocitySet)
                {
                    fallingVelocitySet = true;
                    yVelocity.y = fallStartYVelocity;
                }
                inAirTimer += Time.deltaTime;
                character.animator.SetFloat("inAirTimer", inAirTimer);
                yVelocity.y += gravityForce * Time.deltaTime;

            }
            character.characterController.Move(yVelocity * Time.deltaTime);


        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, groundCheckSphereRadius);
        }
    }

}
