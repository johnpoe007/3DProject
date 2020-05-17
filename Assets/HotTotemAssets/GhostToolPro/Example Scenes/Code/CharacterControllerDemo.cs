using UnityEngine;
using System.Collections;
namespace HotTotem.GhostToolPro.DemoScripts
{
    public class CharacterControllerDemo : MonoBehaviour
    {
        private Animator anim;
        private Transform myTransform;
        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            myTransform = GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                anim.SetBool("WalkRight", true);
                myTransform.position += new Vector3(0.1f, 0, 0);
            }
            else
            {
                anim.SetBool("WalkRight", false);
            }
            if (Input.GetKey(KeyCode.A))
            {
                anim.SetBool("WalkLeft", true);
                myTransform.position += new Vector3(-0.1f, 0, 0);
            }
            else
            {
                anim.SetBool("WalkLeft", false);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    anim.SetTrigger("JumpLeft");
                }
                else
                {
                    anim.SetTrigger("Jump");
                }
                anim.applyRootMotion = true;
            }
        }
    }
}
