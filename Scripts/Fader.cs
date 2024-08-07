using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Fader : MonoBehaviour
{
    public float FadeSeconds = 3f;

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void FadeOut()
    {
        m_animator.SetTrigger(Constants.AnimParam_FadeOut);
    }

    public void FadeIn()
    {
        m_animator.SetTrigger(Constants.AnimParam_FadeIn);
    }
}
