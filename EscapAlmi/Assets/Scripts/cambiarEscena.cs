using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cambiarEscena : MonoBehaviour
{
    public string nombreEscena;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void cambioEscena()
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void cerrar()
    {

    }
}
