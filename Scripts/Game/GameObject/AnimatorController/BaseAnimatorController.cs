using UnityEngine;
using System.Collections;

namespace MTB
{
    public class BaseAnimatorController : MonoBehaviour
    {
        protected Animator animator;
        protected CharacterMotor characterMotor;
        protected string ObjectTag;


        protected bool isJumping = false;
        // Use this for initialization
        void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            animator = gameObject.GetComponent<Animator>();
            characterMotor = GameObject.FindGameObjectWithTag(ObjectTag).GetComponent<CharacterMotor>();
            AddEvent();
        }

        // Update is called once per frame
        void Update()
        {
            if (!((!characterMotor.jumping.jumping && !isJumping) || (characterMotor.jumping.jumping && isJumping)))
            {
                if (characterMotor.jumping.jumping)
                {
                    Jump();
                    isJumping = true;
                }
                else
                {
                    JumpEnd();
                    isJumping = false;
                }
            }
        }

        void OnDestroy()
        {
            disPose();
        }

        public virtual void Move()
        {
            if (animator.isActiveAndEnabled)
            {
                animator.SetFloat("Speed", 0.5F);
            }
        }

        public virtual void StopMove()
        {
            if (animator.isActiveAndEnabled)
            {
                animator.SetFloat("Speed", 0);
            }
        }

        public virtual void DoAction(string key)
        {
            if (animator.isActiveAndEnabled)
            {
                CancelAllAction();
                animator.SetBool(key, true);
            }
        }

        public virtual void CancelAction(string key)
        {
            if (animator.isActiveAndEnabled)
            {
                animator.SetBool(key, false);
            }
        }

        public virtual void CancelAllAction()
        {
        }

        //跳跃动作是根据characterMotor自动判断的外面不必调此方法
        protected virtual void Jump()
        {
            if (animator.isActiveAndEnabled)
            {
                animator.SetBool("Jump", true);
            }
        }
        //跳跃动作是根据characterMotor自动判断的外面不必调此方法
        protected virtual void JumpEnd()
        {
            if (animator.isActiveAndEnabled)
            {
                animator.SetBool("Jump", false);
            }
        }

        protected virtual void AddEvent()
        { }

        protected virtual void RemoveEvent()
        { }

        protected virtual void disPose()
        { }

        void OnJump()
        {
            Debug.Log("jump");
        }
    }
}
