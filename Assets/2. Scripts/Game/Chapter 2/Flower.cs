using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField]
    private Animator[] petalAnimators;

    public void Hit(int i)
    {
        if (i > petalAnimators.Length) return;

        petalAnimators[i - 1].SetTrigger("drop");
    }
}
