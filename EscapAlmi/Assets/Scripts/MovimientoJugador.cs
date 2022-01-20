using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform miTransform;
    public int velocidad;
    void Start()
    {
        miTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {

        movimientoPersonaje();


    }


    private void movimientoPersonaje()
    {
        if (Input.GetKey(KeyCode.W))
        {
            miTransform.Translate(Vector3.forward * velocidad * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            miTransform.Translate(Vector3.back * velocidad * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            miTransform.RotateAround(Vector3.up, -velocidad * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            miTransform.RotateAround(Vector3.up, velocidad * Time.deltaTime);
        }
    }
}
