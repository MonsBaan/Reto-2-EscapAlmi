using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    Animator anim;

    public string newGameSceneName;
    public int quickSaveSlotID;

    [Header("Options Panel")]
    public GameObject OpcionesAjustes;
    public GameObject JugarMenu;
    public GameObject OpcionesAjustesGame;
    public GameObject OpcionesAjustesControles;
    public GameObject OpcionesAjustesGraficos;
    public GameObject JugarMenuConectarse;
    public GameObject JugarMenuSalaEspera;
    public GameObject MenuInventario;
    public GameObject MenuTienda;
    public GameObject MenuLogin;
    public GameObject MenuPerfil;

    public static bool esServer;
    public static string ipServer;
    public GameObject ipInput;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();

        //new key
        PlayerPrefs.SetInt("quickSaveSlot", quickSaveSlotID);
    }

    #region Botones Main Menu
    public void openStartGameOptions()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(false);
        JugarMenu.SetActive(true);
        MenuTienda.SetActive(false);
        MenuLogin.SetActive(false);
        MenuPerfil.SetActive(false);
        MenuInventario.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }
    public void openContinue_Load()
    {
        //enable respective panel
        OpcionesAjustesGame.SetActive(false);
        OpcionesAjustesControles.SetActive(false);
        OpcionesAjustesGraficos.SetActive(false);
        JugarMenuConectarse.SetActive(true);
        JugarMenuSalaEspera.SetActive(false);


        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }

    public void openSalaEsperaHostear()
    {
        //enable respective panel
        OpcionesAjustesGame.SetActive(false);
        OpcionesAjustesControles.SetActive(false);
        OpcionesAjustesGraficos.SetActive(false);
        JugarMenuConectarse.SetActive(false);
        JugarMenuSalaEspera.SetActive(true);
        esServer = true;

        JugarMenuSalaEspera.GetComponent<ScriptSalaEsperaCliente>().enabled = false;
        JugarMenuSalaEspera.GetComponent<ServerSalaEspera>().enabled = true;
        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }

    public void openSalaEsperaUnirse()
    {

        string ip = GameObject.Find("UnirseJuego").transform.GetChild(1).GetChild(2).GetComponent<Text>().text;
        if (ip.Equals(""))
        {
            ip = "127.0.0.1";
        }
        ScriptSalaEsperaCliente.serverIp = ip;

        //enable respective panel
        OpcionesAjustesGame.SetActive(false);
        OpcionesAjustesControles.SetActive(false);
        OpcionesAjustesGraficos.SetActive(false);
        JugarMenuConectarse.SetActive(false);
        JugarMenuSalaEspera.SetActive(true);
        esServer = false;


        JugarMenuSalaEspera.GetComponent<ScriptSalaEsperaCliente>().enabled = true;
        JugarMenuSalaEspera.GetComponent<ServerSalaEspera>().enabled = false;
        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }

    public void openInventario()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(false);
        JugarMenu.SetActive(false);
        MenuTienda.SetActive(false);
        MenuLogin.SetActive(false);
        MenuPerfil.SetActive(false);
        MenuInventario.SetActive(true);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }
    public void openTienda()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(false);
        JugarMenu.SetActive(false);
        MenuTienda.SetActive(true);
        MenuLogin.SetActive(false);
        MenuPerfil.SetActive(false);
        MenuInventario.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }
    public void openLogin()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(false);
        JugarMenu.SetActive(false);
        MenuTienda.SetActive(false);
        MenuLogin.SetActive(true);
        MenuPerfil.SetActive(false);
        MenuInventario.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }
    public void openPerfil()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(false);
        JugarMenu.SetActive(false);
        MenuTienda.SetActive(false);
        MenuLogin.SetActive(false);
        MenuPerfil.SetActive(true);
        MenuInventario.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }

    public void newGameClient()
    {
        if (!string.IsNullOrEmpty("InGame"))
        {
            SceneManager.LoadScene("InGame");
            esServer = false;
            ipServer = ipInput.GetComponent<Text>().text;
        }
        else
            Debug.Log("Please write a scene name in the 'newGameSceneName' field of the Main Menu Script and don't forget to " +
                "add that scene in the Build Settings!");
    }

    #endregion

    #region Ajustes
    public void openOptions()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(true);
        JugarMenu.SetActive(false);
        MenuTienda.SetActive(false);
        MenuLogin.SetActive(false);
        MenuPerfil.SetActive(false);
        MenuInventario.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }

    public void openOptions_Game()
    {
        //enable respective panel
        OpcionesAjustesGame.SetActive(true);
        OpcionesAjustesControles.SetActive(false);
        OpcionesAjustesGraficos.SetActive(false);
        JugarMenuConectarse.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }
    public void openOptions_Controls()
    {
        //enable respective panel
        OpcionesAjustesGame.SetActive(false);
        OpcionesAjustesControles.SetActive(true);
        OpcionesAjustesGraficos.SetActive(false);
        JugarMenuConectarse.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }
    public void openOptions_Gfx()
    {
        //enable respective panel
        OpcionesAjustesGame.SetActive(false);
        OpcionesAjustesControles.SetActive(false);
        OpcionesAjustesGraficos.SetActive(true);
        JugarMenuConectarse.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }
    #endregion

    #region Back Buttons

    public void back_options()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("buttonTweenAnims_off");

        //disable BLUR
       // Camera.main.GetComponent<Animator>().Play("BlurOff");

        //play click sfx
        playClickSound();
    }

    public void back_options_panels()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("OptTweenAnim_off");
        
        //play click sfx
        playClickSound();

    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Sounds
    public void playHoverClip()
    {
       
    }

    void playClickSound() {

    }


    #endregion
}
