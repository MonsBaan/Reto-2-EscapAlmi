using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirarSkin : MonoBehaviour
{
    public float velocidadRotacion;
    private Transform miTransform;

    // Start is called before the first frame update
    void Start()
    {
        miTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        miTransform.RotateAround(Vector3.up, velocidadRotacion);
    }
}
