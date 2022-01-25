using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptItem : MonoBehaviour
{
    public float velocidadFlotar;
    public float velocidadRotacion;
    private Transform miTransform;
    private Vector3 direccion = Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        miTransform = this.transform;
    }

    private void FixedUpdate()
    {
        if(transform.position.y >= 1.5)
        {
            direccion = Vector3.down;
        }
        else if(transform.position.y <= 0.5)
        {
            direccion = Vector3.up;
        }

        miTransform.Translate(direccion * velocidadFlotar * Time.deltaTime);
        miTransform.RotateAround(Vector3.up, velocidadRotacion);

    }
}
