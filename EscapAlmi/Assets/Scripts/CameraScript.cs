using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject camara, jugadorREAL;
    public int camaraDistancia;
    private Transform camaraTransform;
    void Start()
    {
        camaraTransform = this.camara.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posJugador = jugadorREAL.transform.position;
        posJugador.y = posJugador.y + camaraDistancia;
        camaraTransform.position = posJugador;
    }
}
