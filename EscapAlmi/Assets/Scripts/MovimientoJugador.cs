using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MovimientoJugador : MonoBehaviour
{
    // Start is called before the first frame update
    public int idJugadorControl;
    private Transform miTransform;
    public int velocidad;
    public float aceleracion;
    public GameObject rompemuros;

    public GameObject myText;

    float efecto1time, efecto2time, efecto3time, efecto4time;

    void Start()
    {
        miTransform = this.transform;
        rompemuros = this.gameObject.transform.GetChild(2).gameObject;
    }

    private void FixedUpdate()
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

        if (this.gameObject != null && this.gameObject.transform.position.y > 1.5)
        {
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, 1, this.gameObject.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (efecto1time > 0)
        {
            efecto1time -= Time.deltaTime;
            if (efecto1time <= 0)
            {
                GameObject.Find("Main Camera").GetComponent<CameraScript>().camaraDistancia = 10;
                GameObject.Find("Item1").GetComponent<Animator>().SetBool("isSelected", false);
            }
        }

        if (efecto2time > 0)
        {
            efecto2time -= Time.deltaTime;
            if(efecto2time <= 0)
            {
                velocidad = 5;
                GameObject.Find("Item2").GetComponent<Animator>().SetBool("isSelected", false);
            }
        }

        if (efecto3time > 0)
        {
            efecto3time -= Time.deltaTime;
            if (efecto3time <= 0)
            {
                GameObject.Find("Main Camera").GetComponent<CameraScript>().camaraDistancia = 10;
                GameObject.Find("Item3").GetComponent<Animator>().SetBool("isSelected", false);
            }
        }

        if (efecto4time > 0)
        {
            efecto4time -= Time.deltaTime;
            if (efecto4time <= 0)
            {
                GameObject.Find("Item4").GetComponent<Animator>().SetBool("isSelected", false);
            }
        }
    }


    private void movimientoPersonaje()
    {
        Rigidbody myRigidbody = GetComponent<Rigidbody>();
        float move = Input.GetAxis("Vertical");

        myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, velocidad * transform.forward * Input.GetAxis("Vertical"), aceleracion * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
        {
            miTransform.Rotate(Vector3.down, 200 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            miTransform.Rotate(Vector3.up, 200 * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && GameObject.Find("Item1").transform.GetChild(0).gameObject.GetComponent<Text>().text != "0" && efecto1time <= 0)
        {
            cambiarValoresItems(GameObject.Find("Item1"));
            if(efecto1time <= 0)
            {
                efecto1time = 2;
            }

            if (MainMenuController.esServer)
            {
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                GameObject.Find("Server").GetComponent<Server>().activarArena();
            }
            else
            {
                GameObject.Find("Cliente").GetComponent<NetworkClient>().activarArena();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && GameObject.Find("Item2").transform.GetChild(0).gameObject.GetComponent<Text>().text != "0" && efecto2time <= 0)
        {
            cambiarValoresItems(GameObject.Find("Item2"));
            velocidad = 10;
            efecto2time = 10;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && GameObject.Find("Item3").transform.GetChild(0).gameObject.GetComponent<Text>().text != "0" && efecto3time <= 0)
        {
            cambiarValoresItems(GameObject.Find("Item3"));
            GameObject.Find("Main Camera").GetComponent<CameraScript>().camaraDistancia = 20;
            efecto3time = 10;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && GameObject.Find("Item4").transform.GetChild(0).gameObject.GetComponent<Text>().text != "0" && efecto4time <= 0)
        {
            cambiarValoresItems(GameObject.Find("Item4"));
            efecto4time = 2;

            if (MainMenuController.esServer)
            {
                rompemuros.SetActive(true);
                GameObject.Find("Server").GetComponent<Server>().disparo();
            }
            else
            {
                GameObject.Find("Cliente").GetComponent<NetworkClient>().disparo();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }

    private void cambiarValoresItems(GameObject item)
    {
        item.GetComponent<Animator>().SetBool("isSelected", true);
        GameObject childText = item.transform.GetChild(0).gameObject;
        childText.GetComponent<Text>().text = int.Parse(childText.GetComponent<Text>().text) - 1 + "";
    }

    public void arena()
    {
        efecto1time = 10;
        GameObject.Find("Main Camera").GetComponent<CameraScript>().camaraDistancia = 5;
    }
}
