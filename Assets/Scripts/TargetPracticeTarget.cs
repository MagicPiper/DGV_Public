using System;
using Assets.Scripts;
using UnityEngine;

public class TargetPracticeTarget : MonoBehaviour
{

    public TargetPracticeSceneManager manager;
    public Renderer material;
    public Color hitColor;
    public Color startColor;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        manager.TargetHit();
        material.material.color = hitColor;
    }

    internal void ResetColor()
    {
        material.material.color = startColor;

    }
}
