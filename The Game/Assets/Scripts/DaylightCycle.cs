using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    [Min(0), SerializeField] private float speed = 1;

    private const float defaultSpeed = 0.001f;
    private Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    private void Start()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    private void Update()
    {
        CycleDay();
    }

    private void CycleDay()
    {
        transform.Rotate(new Vector3(defaultSpeed * speed, 0, 0));
    }
}
