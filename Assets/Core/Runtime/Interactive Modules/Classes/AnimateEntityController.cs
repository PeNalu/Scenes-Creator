using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateEntityController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    //Stored required properties.
    [SerializeField]
    private string interactAnimation;

    public void SetInteractAnimation(string state)
    {
        interactAnimation = state;
    }

    public void PlayInteractAnimation()
    {
        animator.Play(interactAnimation);
    }
}
