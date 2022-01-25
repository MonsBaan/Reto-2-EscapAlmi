using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovimientoJugador : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform miTransform;
    public int velocidad;
    public float aceleracion;
    void Start()
    {
        if (this.gameObject.name == "JugadorReal")
        {
            miTransform = this.transform;
        }
    }

    private void FixedUpdate()
    {
        if (this.gameObject.name == "JugadorReal")
        {
            movimientoPersonaje();

        }
        if (this.gameObject.name == "JugadorReal")
        {
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
    // Update is called once per frame
    void Update()
    {

    }


    private void movimientoPersonaje()
    {
        Rigidbody myRigidbody = GetComponent<Rigidbody>();
        float move = Input.GetAxis("Vertical");

        myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, velocidad * transform.forward * Input.GetAxis("Vertical"), aceleracion * Time.deltaTime);

        

        if (Input.GetKey(KeyCode.A))
        {
            miTransform.RotateAround(Vector3.down, velocidad * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            miTransform.RotateAround(Vector3.up, velocidad * Time.deltaTime);

        }
    }
}
