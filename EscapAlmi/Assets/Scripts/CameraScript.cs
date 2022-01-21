using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject camara, jugadorREAL;

    private Transform camaraTransform;
    void Start()
    {
        camaraTransform = this.camara.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posJugador = jugadorREAL.transform.position;
        posJugador.y = posJugador.y + 20;
        camaraTransform.position = posJugador;
    }
}
