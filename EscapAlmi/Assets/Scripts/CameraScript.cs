using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject camara, jugadorREAL;
    public int camaraDistancia;
    private Transform camaraTransform;

    public GameObject server, cliente;
    void Start()
    {
        camaraTransform = this.camara.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (jugadorREAL == null)
        {
            if (server.activeSelf)
            {
                jugadorREAL = server.GetComponent<Server>().myPlayer;
            }
            else if (cliente.activeSelf)
            {
                jugadorREAL = cliente.GetComponent<NetworkClient>().jugadoresSimulados[int.Parse(cliente.GetComponent<NetworkClient>().idPlayer)];
            }
        }
        else
        {
            Vector3 posJugador = jugadorREAL.transform.position;
            posJugador.y = posJugador.y + camaraDistancia;
            camaraTransform.position = posJugador;
        }

    }
}
