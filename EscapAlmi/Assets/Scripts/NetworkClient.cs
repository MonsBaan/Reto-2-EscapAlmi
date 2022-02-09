using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;
using UnityEngine.UI;
using NetworkObject.NetworkMessages;
using NetworkObject;
using System;
using UnityEngine.SceneManagement;
using Proyecto26;

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    [Header("Puerto")]
    public ushort serverPort;

    [Header("LISTA DE PERSONAJES")]
    public List<GameObject> jugadoresSimulados;
    public List<GameObject> jugadoresNombres;

    [Header("MODELO DE JUGADOR")]
    public GameObject prefabJugador;
    public GameObject prefabNombreJugador;

    public string idPlayer;

    [Header("PERSONAJE QUE SE ESTA CONTROLANDO")]
    public GameObject myPlayer;
    public GameObject myText;

    [Header("LISTA DE JUGADORES READY")]
    public List<int> jugadoresReady;

    private bool jugadoresActivados = false;

    private bool spawneado = false;

    private bool dineros = false;
    void Start()
    {
        Conectar();
    }

    public void Conectar()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(ScriptSalaEsperaCliente.serverIp, serverPort);
        m_Connection = m_Driver.Connect(endpoint);
    }

    void Update()
    {
        connectionStuff();

        if (!spawneado && myPlayer != null)
        {
            GameObject spawnpoint = GameObject.FindGameObjectWithTag("SpawnPoint");
            float numRandom1 = UnityEngine.Random.Range(-5, +5);
            float numRandom2 = UnityEngine.Random.Range(-5, +5);

            myPlayer.transform.position = new Vector3(spawnpoint.transform.position.x + numRandom1, spawnpoint.transform.position.y, spawnpoint.transform.position.z + numRandom2);
            spawneado = true;
        }

        foreach (var jugador in jugadoresReady)
        {
            if (!jugadoresSimulados[jugador].tag.Equals("PlayerFin"))
            {
                jugadoresSimulados[jugador].SetActive(true);
            }
        }
    }
    #region ConnectionStuff
    void connectionStuff()
    {
        m_Driver.ScheduleUpdate().Complete();
        if (!m_Connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd = m_Connection.PopEvent(m_Driver, out stream);

        while (cmd != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                OnConnect();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                OnDisconnect();
            }
            cmd = m_Connection.PopEvent(m_Driver, out stream);
        }
    }
    private void SendToServer(string v)
    {
        DataStreamWriter writer;
        m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);
        NativeArray<byte> bytes = new
            NativeArray<byte>(Encoding.ASCII.GetBytes(v), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    private void OnConnect()
    {
        Debug.Log("Conectado Correctamente");

    }

    private void OnDisconnect()
    {
        m_Connection = default(NetworkConnection);
    }

    public void OnDestroy()
    {
        m_Connection.Disconnect(m_Driver);
        m_Driver.Dispose();
    }
    #endregion

    #region Recibo de Datos
    private void OnData(DataStreamReader stream)
    {

        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch (header.command)
        {
            case Commands.HANDSHAKE:
                HandshakeMsg mensajeRecibido = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                //asigno la id de la conexion en cliente, para despues enviar mensajes
                Debug.Log("Id de Jugador: " + mensajeRecibido.player.id);
                idPlayer = mensajeRecibido.player.id;

                //CREACION DE TANTOS PERSONAJES COMO JUGADORES HABIA EN LA SALA DE ESPERA
                for (int i = 0; i < mensajeRecibido.numJugadores; i++)
                {
                    GameObject jugadorSimulado = Instantiate(prefabJugador);
                    jugadorSimulado.GetComponent<JugadorSimuladoScript>().enabled = true;
                    jugadorSimulado.GetComponent<JugadorSimuladoScript>().jugadorID = i;
                    jugadoresSimulados.Add(jugadorSimulado);

                    GameObject textoJugador = Instantiate(prefabNombreJugador);
                    textoJugador.GetComponent<TextMesh>().text = "";
                    jugadoresNombres.Add(textoJugador);
                }

                //LE ASIGNO EL PERSONAJE AL CLIENTE
                myPlayer = jugadoresSimulados[int.Parse(mensajeRecibido.player.id)];
                myText = jugadoresNombres[int.Parse(mensajeRecibido.player.id)];
                myText.GetComponent<TextMesh>().text = MainMenuController.nombreCuentaJugador;
                myText.GetComponent<ScriptTexto>().jugador = myPlayer;
                myText.SetActive(true);

                myPlayer.GetComponent<MovimientoJugador>().enabled = true;
                myPlayer.GetComponent<JugadorSimuladoScript>().enabled = false;

                JugadorReady jugadorReady = new JugadorReady();
                jugadorReady.idJugador = int.Parse(idPlayer);
                jugadorReady.nombre = MainMenuController.nombreCuentaJugador;

                SendToServer(JsonUtility.ToJson(jugadorReady));
                break;

            case Commands.READY:
                ReadyMsg readyMsg = JsonUtility.FromJson<ReadyMsg>(recMsg);
                GameObject.Find("Mapas").GetComponent<ElegirMapa>().cargarMapa(readyMsg.indexMap);

                break;

            case Commands.MOVER_JUGADOR:
                MoverMsg moverRecMsg = JsonUtility.FromJson<MoverMsg>(recMsg);
                moveJugadorMsg(moverRecMsg);


                break;

            case Commands.MANTENER_CONEXION:
                MantenerConexion mantenerConexion = JsonUtility.FromJson<MantenerConexion>(recMsg);
                jugadoresReady = mantenerConexion.jugadoresReady;

                break;

            case Commands.ITEM_GET:
                ItemMsg itemMsg = JsonUtility.FromJson<ItemMsg>(recMsg);

                if (itemMsg.tipo.Equals("moneda"))
                {
                    GameObject.Find("MapaActual(Clone)").GetComponent<ScriptLaberintoOK>().poolCoins[itemMsg.indexItem].SetActive(false);
                }
                else if (itemMsg.tipo.Equals("powerup"))
                {
                    GameObject.Find("MapaActual(Clone)").GetComponent<ScriptLaberintoOK>().poolItems[itemMsg.indexItem].SetActive(false);
                }

                break;

            case Commands.DISPARO:
                DisparoMsg disparoMsg = JsonUtility.FromJson<DisparoMsg>(recMsg);

                jugadoresSimulados[disparoMsg.idPlayer].transform.GetChild(2).gameObject.SetActive(true);

                break;

            case Commands.ACTIVAR_ARENA:
                ActivarArenaMsg activarArenaMsg = JsonUtility.FromJson<ActivarArenaMsg>(recMsg);

                jugadoresSimulados[activarArenaMsg.idPlayer].transform.GetChild(1).gameObject.SetActive(true);

                break;

            case Commands.ARENA:
                ArenaMsg arenaMsg = JsonUtility.FromJson<ArenaMsg>(recMsg);

                if (arenaMsg.idJugador == int.Parse(idPlayer))
                {
                    myPlayer.GetComponent<MovimientoJugador>().arena();
                }

                break;

            case Commands.FIN_PLAYER:
                FinPlayerMsg finPlayerMsg = JsonUtility.FromJson<FinPlayerMsg>(recMsg);
                jugadoresSimulados[finPlayerMsg.idJugador].tag = "PlayerFin";
                jugadoresSimulados[finPlayerMsg.idJugador].SetActive(false);
                jugadoresNombres[finPlayerMsg.idJugador].SetActive(false);


                break;

            case Commands.CAMBIO_ESCENA:
                CambiarEscena cambiarEscena = JsonUtility.FromJson<CambiarEscena>(recMsg);

                MainMenuController.monedasJugador += int.Parse(GameObject.Find("Monedas").GetComponent<Text>().text);
                if (!dineros)
                {
                    guardarPerfil(MainMenuController.monedasJugador);
                    dineros = true;
                }


                break;
            case Commands.TIEMPO:
                TiempoMsg tiempo = JsonUtility.FromJson<TiempoMsg>(recMsg);
                GameObject.Find("Mapas").GetComponent<ElegirMapa>().changeTiempo(tiempo.min, tiempo.sec);



                break;

            default:
                Debug.Log(header.command + " No disponible");
                break;
        }
    }


    #endregion

    void moveJugadorMsg(MoverMsg moverRecMsg)
    {

        if (moverRecMsg.jugador.id != idPlayer)
        {
            GameObject personaje = jugadoresSimulados[int.Parse(moverRecMsg.jugador.id)];

            personaje.transform.position = moverRecMsg.jugador.posJugador;
            personaje.transform.rotation = moverRecMsg.jugador.rotacion;

            GameObject texto = jugadoresNombres[int.Parse(moverRecMsg.jugador.id)];
            texto.GetComponent<TextMesh>().text = moverRecMsg.jugador.nombre;
            texto.GetComponent<ScriptTexto>().enabled = false;

            if (!texto.activeSelf)
                texto.SetActive(true);

            texto.transform.position = moverRecMsg.posTextJugador;
        }

        foreach (var material in MainMenuController.materialesStatic)
        {
            if (material.name == moverRecMsg.nombreSkin)
            {
                jugadoresSimulados[int.Parse(moverRecMsg.jugador.id)].GetComponent<MeshRenderer>().material = material;

            }
        }
    }


    public void movimiento(Vector3 pos, Quaternion rotacion)
    {
        MoverMsg moverMsg = new MoverMsg();
        moverMsg.jugador.id = idPlayer;
        moverMsg.jugador.nombre = MainMenuController.nombreCuentaJugador;
        moverMsg.jugador.posJugador = pos;
        moverMsg.jugador.rotacion = rotacion;
        moverMsg.nombreSkin = MainMenuController.skinActualName;

        moverMsg.posTextJugador = myText.transform.position;

        SendToServer(JsonUtility.ToJson(moverMsg));
    }

    public void itemGet(int index, string tipo)
    {
        ItemMsg itemMsg = new ItemMsg();
        itemMsg.indexItem = index;
        itemMsg.tipo = tipo;

        SendToServer(JsonUtility.ToJson(itemMsg));
    }

    public void disparo()
    {
        DisparoMsg disparoMsg = new DisparoMsg();
        disparoMsg.idPlayer = int.Parse(idPlayer);

        SendToServer(JsonUtility.ToJson(disparoMsg));
    }

    public void activarArena()
    {
        ActivarArenaMsg activarArenaMsg = new ActivarArenaMsg();
        activarArenaMsg.idPlayer = int.Parse(idPlayer);

        SendToServer(JsonUtility.ToJson(activarArenaMsg));
    }

    public void efectoArena(int idJugador)
    {
        ArenaMsg arenaMsg = new ArenaMsg();
        arenaMsg.idJugador = idJugador;

        SendToServer(JsonUtility.ToJson(arenaMsg));
    }

    public void finPlayer()
    {
        FinPlayerMsg finPlayerMsg = new FinPlayerMsg();
        finPlayerMsg.idJugador = int.Parse(idPlayer);

        SendToServer(JsonUtility.ToJson(finPlayerMsg));

        GameObject.Find("Main Camera").GetComponent<CameraScript>().cambioFin();
    }
    public void guardarPerfil(int monedas)
    {
        UpdateCompraSymfony updateCompraSymfony = new UpdateCompraSymfony();
        updateCompraSymfony.id = MainMenuController.idCuentaJugador;
        updateCompraSymfony.monedas = monedas;

        RestClient.Put("https://escapalmisymfony.duckdns.org/update", JsonUtility.ToJson(updateCompraSymfony)).Then(res =>
        {
            SceneManager.LoadScene("Menu");
            OnDisconnect();
        });
    }
}
