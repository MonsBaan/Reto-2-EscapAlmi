using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTexto : MonoBehaviour
{
    public GameObject jugador;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        seguirJugador();
    }

    void seguirJugador()
    {
        Vector3 posJugador = jugador.transform.position;
        this.transform.position = new Vector3(posJugador.x, posJugador.y + 2, posJugador.z - 1);
    }
}

