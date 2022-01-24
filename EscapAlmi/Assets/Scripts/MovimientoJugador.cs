using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovimientoJugador : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform miTransform;
    public int velocidad;

    void Start()
    {
        if (this.gameObject.name == "JugadorReal")
        {
            miTransform = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.name == "JugadorReal")
        {
            movimientoPersonaje();

            if (MainMenuController.esServer)
            {
                GameObject.Find("Server").GetComponent<Server>().movimiento(this.gameObject.transform.position, this.gameObject.transform.rotation);
            }
            else
            {
                GameObject.Find("Cliente").GetComponent<NetworkClient>().movimiento(this.gameObject.transform.position, this.gameObject.transform.rotation);
            }
        }
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
