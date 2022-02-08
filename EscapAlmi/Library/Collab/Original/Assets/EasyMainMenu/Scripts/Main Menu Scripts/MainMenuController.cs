using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class MainMenuController : MonoBehaviour
{

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
    public GameObject MenuRegistro;
    public GameObject btnLogin;
    public GameObject btnRegistro;
    public GameObject btnPerfil;
    public GameObject perfilNombre;
    public GameObject perfilCorreo;
    public GameObject perfilPassword;
    public GameObject perfilNombrePlaceholder;
    public GameObject perfilCorreoPlaceholder;
    public GameObject registroNombre;
    public GameObject registroCorreo;
    public GameObject registroPassword;
    public GameObject dineroTienda;
    public GameObject btnComprar;
    public GameObject btnGuardar;

    public AudioClip clickSound, HoverSound;

    public static bool esServer;
    public static string ipServer;
    public GameObject ipInput;
    public GameObject loginInput;
    public GameObject loginPassword;

    public static int idCuentaJugador = -1;
    public static string nombreCuentaJugador = "";
    public static string correoCuentaJugador = "";
    public static string passwordCuentaJugador = "";
    public static int monedasJugador = 0;
    public static List<string> arraySkinsJugador;
    public List<ResTiendaSymfony> arraySkinsTienda;
    public static int skinActual = -1;
    public static int precioSkinActual;
    public List<Material> arrayInventario;
    private int materialActInv = -1;

    public GameObject jugTienda;
    public List<Material> listaMateriales;
    private int materialActTienda = 0;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

        //new key
        PlayerPrefs.SetInt("quickSaveSlot", quickSaveSlotID);

        arraySkinsJugador = new List<string>();
        cargarSkinsTienda();
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

        cargarInventario();
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

        precioSkinActual = arraySkinsTienda[materialActTienda].precio;
        GameObject.Find("Precio").GetComponent<Text>().text = precioSkinActual + "";
        dineroTienda.GetComponent<Text>().text = "Monedas: " + monedasJugador;
        cambiarBotonCompra();
    }
    public void openLogin()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(false);
        JugarMenu.SetActive(false);
        MenuTienda.SetActive(false);
        MenuLogin.SetActive(true);
        MenuRegistro.SetActive(false);
        MenuPerfil.SetActive(false);
        MenuInventario.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }
    public void openRegistrarse()
    {
        //enable respective panel
        OpcionesAjustes.SetActive(false);
        JugarMenu.SetActive(false);
        MenuTienda.SetActive(false);
        MenuLogin.SetActive(false);
        MenuRegistro.SetActive(true);
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
        if (JugarMenuSalaEspera.activeSelf == true)
        {
            JugarMenuSalaEspera.GetComponent<ServerSalaEspera>().listaJugadores.Clear();
            JugarMenuSalaEspera.GetComponent<ScriptSalaEsperaCliente>().listaJugadores.Clear();

            for (int i = 0; i < JugarMenuSalaEspera.transform.GetChild(2).GetChild(0).GetChild(0).childCount; i++)
            {
                Destroy(JugarMenuSalaEspera.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(i).gameObject);
            }

            JugarMenuSalaEspera.GetComponent<ScriptSalaEsperaCliente>().enabled = false;
            JugarMenuSalaEspera.GetComponent<ServerSalaEspera>().enabled = false;

        }
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
        if (!this.GetComponent<AudioSource>().isPlaying)
        {
            this.GetComponent<AudioSource>().clip = HoverSound;
            this.GetComponent<AudioSource>().Play();
        }

    }

    void playClickSound()
    {
        this.GetComponent<AudioSource>().clip = clickSound;
        this.GetComponent<AudioSource>().Play();
    }


    #endregion

    public void anteriorMaterial()
    {
        if(materialActTienda == 0)
        {
            materialActTienda = listaMateriales.Count - 1;
        }
        else
        {
            materialActTienda--;
        }

        jugTienda.GetComponent<MeshRenderer>().material = listaMateriales[materialActTienda];
        jugTienda.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = listaMateriales[materialActTienda];
        precioSkinActual = arraySkinsTienda[materialActTienda].precio;
        GameObject.Find("Precio").GetComponent<Text>().text = precioSkinActual + "";
        cambiarBotonCompra();
    }

    public void sigienteMaterial()
    {
        if (materialActTienda == listaMateriales.Count - 1)
        {
            materialActTienda = 0;
        }
        else
        {
            materialActTienda++;
        }

        jugTienda.GetComponent<MeshRenderer>().material = listaMateriales[materialActTienda];
        jugTienda.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = listaMateriales[materialActTienda];
        precioSkinActual = arraySkinsTienda[materialActTienda].precio;
        GameObject.Find("Precio").GetComponent<Text>().text = precioSkinActual + "";
        cambiarBotonCompra();
    }

    public void login()
    {
        PostLoginSymfony postLogin = new PostLoginSymfony();
        postLogin.nombre = loginInput.GetComponent<Text>().text;
        postLogin.contraseña = loginPassword.GetComponent<InputField>().text;

        RestClient.Post("https://escapalmisymfony.duckdns.org/login", JsonUtility.ToJson(postLogin)).Then(res =>
        {
            if (!res.Text.Equals("[]"))
            {
                ResLoginSymfony resLogin = JsonUtility.FromJson<ResLoginSymfony>(res.Text.Substring(1, res.Text.Length - 2));

                idCuentaJugador = resLogin.id;
                nombreCuentaJugador = resLogin.nombre;
                correoCuentaJugador = resLogin.email;
                monedasJugador = int.Parse(resLogin.monedas);
                passwordCuentaJugador = loginPassword.GetComponent<InputField>().text;

                loginInput.GetComponent<Text>().text = "";
                loginPassword.GetComponent<InputField>().text = "";
                perfilNombrePlaceholder.GetComponent<Text>().text = nombreCuentaJugador;
                perfilCorreoPlaceholder.GetComponent<Text>().text = correoCuentaJugador;
                dineroTienda.GetComponent<Text>().text = "Monedas: " + monedasJugador;

                logear(true);
                back_options();

                RestClient.GetArray<ResInventarioSymfony>("https://escapalmisymfony.duckdns.org/inventario/" + idCuentaJugador).Then(res =>
                {
                    for (int i = 0; i < res.Length; i++)
                    {
                        string objeto = res[i].nombre;

                        arraySkinsJugador.Add(objeto);
                    }
                });
            }            
        });
    }

    public void logear(bool estado)
    {
        btnLogin.SetActive(!estado);
        btnRegistro.SetActive(!estado);
        btnPerfil.SetActive(estado);
    }

    public void logout()
    {
        logear(false);

        idCuentaJugador = -1;
        nombreCuentaJugador = "";
        correoCuentaJugador = "";
        monedasJugador = 0;
        arraySkinsJugador.Clear();
        skinActual = -1;

        perfilNombre.transform.parent.GetComponent<InputField>().text = "";
        perfilCorreo.transform.parent.GetComponent<InputField>().text = "";
        perfilNombre.GetComponent<Text>().text = "";
        perfilCorreo.GetComponent<Text>().text = "";
        perfilPassword.GetComponent<InputField>().text = "";
        perfilNombrePlaceholder.GetComponent<Text>().text = "";
        perfilCorreoPlaceholder.GetComponent<Text>().text = "";
        dineroTienda.GetComponent<Text>().text = "Monedas: -";

        back_options();
    }

    public void registro()
    {
        PostRegistroSymfony postRegistro = new PostRegistroSymfony();
        postRegistro.nombre = registroNombre.GetComponent<Text>().text.Trim();
        postRegistro.email = registroCorreo.GetComponent<Text>().text.Trim();
        postRegistro.contraseña = registroPassword.GetComponent<Text>().text.Trim();

        if(!postRegistro.nombre.Equals("") && !postRegistro.email.Equals("") && !postRegistro.contraseña.Equals(""))
        {
            RestClient.Post("https://escapalmisymfony.duckdns.org/addusuario", JsonUtility.ToJson(postRegistro)).Then(res =>
            {
                //Debug.Log(res);

                back_options();
            });
        }
    }

    public void compra()
    {
        if(idCuentaJugador != -1 && (monedasJugador - precioSkinActual) >= 0 && btnComprar.transform.GetChild(0).gameObject.GetComponent<Text>().text.Equals("Comprar"))
        {
            UpdateCompraSymfony updateCompraSymfony = new UpdateCompraSymfony();
            updateCompraSymfony.id = idCuentaJugador;
            updateCompraSymfony.nombre = nombreCuentaJugador;
            updateCompraSymfony.email = correoCuentaJugador;
            updateCompraSymfony.contrasena = passwordCuentaJugador;
            monedasJugador = monedasJugador - precioSkinActual;
            updateCompraSymfony.monedas = monedasJugador;

            RestClient.Put("https://escapalmisymfony.duckdns.org/update", JsonUtility.ToJson(updateCompraSymfony)).Then(res =>
            {
                
            });

            PostCompraSymfony postCompraSymfony = new PostCompraSymfony();
            postCompraSymfony.idSkin = arraySkinsTienda[materialActTienda].descripcion;
            postCompraSymfony.idUsuario = idCuentaJugador + "";

            RestClient.Post("https://escapalmisymfony.duckdns.org/postUsuarioSkin", JsonUtility.ToJson(postCompraSymfony)).Then(res =>
            {
                dineroTienda.GetComponent<Text>().text = "Monedas: " + monedasJugador;

                arraySkinsJugador.Add(arraySkinsTienda[materialActTienda].nombre);

                cambiarBotonCompra();
            });
        }
    }

    public void cargarSkinsTienda()
    {
        ResTiendaSymfony resTiendaSymfony = new ResTiendaSymfony();
        RestClient.GetArray<ResTiendaSymfony>("https://escapalmisymfony.duckdns.org/tiendaItems").Then(res =>
        {
            for (int i = 0; i < res.Length; i++)
            {
                ResTiendaSymfony objeto = new ResTiendaSymfony();
                objeto.nombre = res[i].nombre;
                objeto.descripcion = res[i].descripcion;
                objeto.precio = res[i].precio;

                arraySkinsTienda.Add(objeto);
            }
        });
    }

    public void guardarPerfil()
    {
        UpdateCompraSymfony updateCompraSymfony = new UpdateCompraSymfony();
        updateCompraSymfony.id = idCuentaJugador;
        updateCompraSymfony.nombre = perfilNombre.GetComponent<Text>().text;
        updateCompraSymfony.email = perfilCorreo.GetComponent<Text>().text;
        updateCompraSymfony.contrasena = perfilPassword.GetComponent<InputField>().text;
        updateCompraSymfony.monedas = monedasJugador;

        RestClient.Put("https://escapalmisymfony.duckdns.org/update", JsonUtility.ToJson(updateCompraSymfony)).Then(res =>
        {
            back_options();
        });
    }

    public void btnGuardarActivo()
    {
        if(!perfilNombre.GetComponent<Text>().text.Equals("") && !perfilCorreo.GetComponent<Text>().text.Equals("") && !perfilPassword.GetComponent<InputField>().text.Equals(""))
        {
            btnGuardar.SetActive(true);
        }
        else
        {
            btnGuardar.SetActive(false);
        }
    }

    public void cambiarBotonCompra()
    {
        bool tiene = false;
        for (int i = 0; i < arraySkinsJugador.Count; i++)
        {
            if (arraySkinsTienda[materialActTienda].nombre.Equals(arraySkinsJugador[i]))
            {
                tiene = true;
            }
        }

        if (tiene)
        {
            btnComprar.transform.GetChild(0).gameObject.GetComponent<Text>().text = "En inventario";
        }
        else
        {
            btnComprar.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Comprar";
        }
    }

    public void cargarInventario()
    {
        if(arraySkinsJugador.Count <= 0)
        {
            GameObject.Find("EquiparInv").transform.GetChild(0).gameObject.GetComponent<Text>().text = "No hay skins";
        }
        else
        {
            for (int i = 0; i < arraySkinsJugador.Count; i++)
            {
                for (int j = 0; j < arraySkinsTienda.Count; j++)
                {
                    if (arraySkinsJugador[i].Equals(arraySkinsTienda[j].nombre))
                    {
                        arrayInventario.Add(listaMateriales[int.Parse(arraySkinsTienda[j].descripcion) - 1]);
                    }
                }
            }

            materialActInv = 0;
            GameObject.Find("pruebasInv").GetComponent<MeshRenderer>().material = arrayInventario[materialActInv];
            cambiarBotonEquipar();
        }
    }

    public void anteriorInventario()
    {
        if (materialActInv == 0)
        {
            materialActInv = arrayInventario.Count - 1;
        }
        else
        {
            materialActInv--;
        }

        GameObject.Find("pruebasInv").GetComponent<MeshRenderer>().material = arrayInventario[materialActInv];
        cambiarBotonEquipar();
    }

    public void siguienteInventario()
    {
        if (materialActInv == arrayInventario.Count - 1)
        {
            materialActInv = 0;
        }
        else
        {
            materialActInv++;
        }

        GameObject.Find("pruebasInv").GetComponent<MeshRenderer>().material = arrayInventario[materialActInv];
        cambiarBotonEquipar();
    }

    public void cambiarBotonEquipar()
    {
        if (materialActInv == skinActual)
        {
            GameObject.Find("EquiparInv").transform.GetChild(0).gameObject.GetComponent<Text>().text = "Equipado";
        }
        else
        {
            GameObject.Find("EquiparInv").transform.GetChild(0).gameObject.GetComponent<Text>().text = "Equipar";
        }
    }

    public void btnEquipar()
    {
        skinActual = materialActInv;
        cambiarBotonEquipar();
    }
}

[System.Serializable]
public class PostLoginSymfony
{
    public string nombre;
    public string contraseña;
}

[System.Serializable]
public class ResLoginSymfony
{
    public int id;
    public string nombre;
    public string email;
    public string monedas;
    public string[] skins;
}

[System.Serializable]
public class PostRegistroSymfony
{
    public string nombre;
    public string email;
    public string contraseña;
}

[System.Serializable]
public class UpdateCompraSymfony
{
    public int id;
    public string nombre;
    public string email;
    public string contrasena;
    public int monedas;
}

[System.Serializable]
public class ResTiendaSymfony
{
    public string nombre;
    public string descripcion;
    public int precio;
}

[System.Serializable]
public class ResInventarioSymfony
{
    public string nombre;
}

[System.Serializable]
public class PostCompraSymfony
{
    public string idUsuario;
    public string idSkin;
}
