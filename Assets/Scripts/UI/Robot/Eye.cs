using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{

    public Transform reference;
    public float factor = 0.25f;
    public float limit = 0.08f;

    private Vector3 center;
    private Vector3 referencePoint;

    void Start()
    {
        referencePoint = reference.transform.position;
        center = RectTransformUtility.WorldToScreenPoint(null, transform.position);
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 dir = (mousePos - center) * factor;
        dir = Vector3.ClampMagnitude(dir, limit);
        Vector3 tt = referencePoint + dir;
        transform.position = tt;
    }
}
