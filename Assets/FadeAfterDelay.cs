using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAfterDelay : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        var renderer = GetComponent<SpriteRenderer>();
        var col = renderer.color;

        while (col.a > 0.0001) {
            col.a = Mathf.MoveTowards(col.a, 0f, Time.deltaTime * 0.5f);
            renderer.color = col;
            yield return null;
        }

        renderer.enabled = false;
    }
}
