using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator AnimatorCompo { get; private set; }

    [Range(0, 1f)] public float startAnimTime = 0.3f;
    [Range(0, 1f)] public float stopAnimTime = 0.15f;
    public float allowPlayerAnimation = 0.1f;

    private readonly int _xHash = Animator.StringToHash("X");
    private readonly int _yHash = Animator.StringToHash("Y");
    private readonly int _shootingHash = Animator.StringToHash("shooting");
    private readonly int _blendHash = Animator.StringToHash("blend");

    private void Awake()
    {
        AnimatorCompo = GetComponent<Animator>();
    }

    public void SetXY(Vector2 dir)
    {
        float value = dir.sqrMagnitude;
        if(value > allowPlayerAnimation)
        {
            AnimatorCompo.SetFloat(_xHash, dir.x, startAnimTime * 0.33f, Time.deltaTime);
            AnimatorCompo.SetFloat(_yHash, dir.y, startAnimTime * 0.33f, Time.deltaTime);
        }
        else
        {
            //��ü ������ ����
            AnimatorCompo.SetFloat(_xHash, dir.x, stopAnimTime * 0.33f, Time.deltaTime);
            AnimatorCompo.SetFloat(_yHash, dir.y, stopAnimTime * 0.33f, Time.deltaTime);
        }
    }

    public void SetShooting(bool value)
    {
        AnimatorCompo.SetBool(_shootingHash, value);
    }

    public void SetBlendValue(float blend)
    {
        if(blend > allowPlayerAnimation)
        {
            AnimatorCompo.SetFloat(_blendHash, blend, startAnimTime, Time.deltaTime);
        }
        else
        {
            AnimatorCompo.SetFloat(_blendHash, blend, stopAnimTime, Time.deltaTime);
        }
    }
}
