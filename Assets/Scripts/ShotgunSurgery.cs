﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSurgery : MonoBehaviour
{
    public void ForceStartCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void ForceStopCoroutine(IEnumerator coroutine)
    {
        StopCoroutine(coroutine);
    }
}
