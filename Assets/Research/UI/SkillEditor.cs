using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillEditor : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private GameObject objectToSpawn;

    [SerializeField]
    private Vector3 start;

    [Range(1, 100)]
    [SerializeField]
    private float curveLength;

    [Range(2, 100)]
    [SerializeField]
    private int spawnCount;

    [Range(1, 100)]
    [SerializeField]
    private float heightMultiplier;

    private void Start()
    {
    }
}
