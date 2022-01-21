using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptLuz : MonoBehaviour
{
    public GameObject jugador;
    // Start is called before the first frame update
    private Transform miTransform;
    void Start()
    {
        miTransform = this.transform;
        jugador = GameObject.Find("Jugador");
    }

    // Update is called once per frame
    void Update()
    {
        miTransform.LookAt(jugador.transform);
    }
}
