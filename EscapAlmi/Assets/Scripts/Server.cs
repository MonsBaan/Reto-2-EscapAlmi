using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;
using NetworkObject.NetworkMessages;
using NetworkObject;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public ushort serverPort;
    public NativeList<NetworkConnection> m_connections;

    [Header("LISTA DE JUGADORES")]
    public List<NetworkObject.NetworkObject.Jugador> jugadores;
    [Header("LISTA DE JUGADORES READY")]
    public List<int> jugadoresReady;
    [Header("LISTA DE PERSONAJES")]
    public List<GameObject> jugadoresSimulados;
    public List<GameObject> jugadoresNombres;

    [Header("MODELO DE JUGADOR")]
    public GameObject prefabJugador;
    public GameObject prefabNombreJugador;


    [Header("PERSONAJE QUE SE ESTA CONTROLANDO")]
    public GameObject myPlayer;
    public GameObject myText;

    private bool jugando = false;
    private bool jugadoresActivados = false;
    private int numJugadoresFin = 0;

    private float tempDescon = 2;
    private bool finjuego = false;

    private bool spawneado = false;
    void Start()
    {
        //CREACION DE TANTOS PERSONAJES COMO JUGADORES HABIA EN LA SALA DE ESPERA
        for (int i = 0; i < ServerSalaEspera.numJugadores + 1; i++)
        {
            GameObject jugadorSimulado = Instantiate(prefabJugador);
            jugadorSimulado.GetComponent<JugadorSimuladoScript>().enabled = true;
            jugadorSimulado.GetComponent<JugadorSimuladoScript>().jugadorID = i;
            jugadoresSimulados.Add(jugadorSimulado);

            GameObject textoJugador = Instantiate(prefabNombreJugador);
            textoJugador.GetComponent<TextMesh>().text = "";
            jugadoresNombres.Add(textoJugador);
        }
        //LE ASIGNO EL PRIMER JUGADOR AL SERVIDOR
        myPlayer = jugadoresSimulados[0];
        myPlayer.GetComponent<MovimientoJugador>().enabled = true;
        myPlayer.GetComponent<JugadorSimuladoScript>().enabled = false;
        myText = jugadoresNombres[0];
        myText.SetActive(true);
        myText.GetComponent<ScriptTexto>().jugador = myPlayer;
        myText.GetComponent<TextMesh>().text = MainMenuController.nombreCuentaJugador;


        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = serverPort;
        if (m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port: " + serverPort);
        }
        else
        {
            m_Driver.Listen();
        }
        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        jugadores = new List<NetworkObject.NetworkObject.Jugador>();
        jugadoresReady = new List<int>();
        jugadoresReady.Add(0);


    }

    // Update is called once per frame
    void Update()
    {
        connectionStuff();
        mantenerConexion();

        if (!spawneado && myPlayer != null)
        {
            GameObject spawnpoint = GameObject.FindGameObjectWithTag("SpawnPoint");
            float numRandom1 = UnityEngine.Random.Range(-5, +5);
            float numRandom2 = UnityEngine.Random.Range(-5, +5);

            myPlayer.transform.position = new Vector3(spawnpoint.transform.position.x + numRandom1, spawnpoint.transform.position.y, spawnpoint.transform.position.z + numRandom2);
            spawneado = true;
        }

        if (finjuego)
        {
            tempDescon -= Time.deltaTime;
            if(tempDescon <= 0)
                SceneManager.LoadScene("Menu");

        }

        if (jugadores.Count == ServerSalaEspera.numJugadores && !jugando)
        {
            ReadyMsg readyMsg = new ReadyMsg();
            readyMsg.playerList = jugadores;
            readyMsg.indexMap = ElegirMapa.indexMapa;
            readyMsg.numJugadores = m_connections.Length;


            for (int i = 0; i < m_connections.Length; i++)
            {
                SendToClient(JsonUtility.ToJson(readyMsg), m_connections[i]);
            }
            jugando = true;
        }

        if (numJugadoresFin == jugadoresSimulados.Count)
        {
            terminarPartida();
        }

    }

    #region ConnectionStuff
    void connectionStuff()
    {
        m_Driver.ScheduleUpdate().Complete();

        for (int i = 0; i < m_connections.Length; i++)
        {
            if (!m_connections[i].IsCreated)
            {
                m_connections.RemoveAtSwapBack(i);
                i--;
            }
        }

        //aceptamos las conexiones
        NetworkConnection c = m_Driver.Accept();
        while (c != default(NetworkConnection))
        {
            OnConnect(c);
            c = m_Driver.Accept();
        }

        //leer mensajes
        DataStreamReader stream;
        for (int i = 0; i < m_connections.Length; i++)
        {
            Assert.IsTrue(m_connections[i].IsCreated);
            NetworkEvent.Type cmd;
            cmd = m_Driver.PopEventForConnection(m_connections[i], out stream);
            while (cmd != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream, i);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    OnDisconnect(i);
                }
                //pasamos al siguiente mensaje
                cmd = m_Driver.PopEventForConnection(m_connections[i], out stream);
            }
        }
    }
    void OnConnect(NetworkConnection c)
    {
        m_connections.Add(c);
        Debug.Log("Accepted connection");

        HandshakeMsg m = new HandshakeMsg();
        m.player.id = m_connections.Length + ""; //EL ID ES LA POSICION DEL ARRAY "JugadoresSimulados"
        m.numJugadores = ServerSalaEspera.numJugadores + 1; //SE LE AÑADE AQUI EL USUARIO EXTRA QUE ES EL SERVIDOR
        jugadores.Add(m.player);

        SendToClient(JsonUtility.ToJson(m), c);
    }

    void mantenerConexion()
    {
        MantenerConexion mantenerConexion = new MantenerConexion();
        mantenerConexion.jugadoresReady = jugadoresReady;

        if (!jugadoresActivados)
        {
            foreach (var jugador in mantenerConexion.jugadoresReady)
            {
                jugadoresSimulados[jugador].SetActive(true);
            }

            jugadoresActivados = true;
        }

        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(mantenerConexion), m_connections[i]);
        }
    }

    private void OnDisconnect(int i)
    {
        m_connections[i] = default(NetworkConnection);
    }

    public void OnDestroy()
    {
        m_connections.Dispose();
        m_Driver.Dispose();
    }

    public void SendToClient(string message, NetworkConnection c)
    {
        //var writer = m_Driver.BeginSend(NetworkPipeline.Null, c);
        DataStreamWriter writer;
        m_Driver.BeginSend(NetworkPipeline.Null, c, out writer);
        NativeArray<byte> bytes = new
            NativeArray<byte>(Encoding.ASCII.GetBytes(message), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }
    #endregion


    #region Recibo de Datos
    private void OnData(DataStreamReader stream, int numJugador)
    {
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch (header.command)
        {
            case Commands.READY:
                ReadyMsg readyMsg = JsonUtility.FromJson<ReadyMsg>(recMsg);


                break;

            case Commands.MOVER_JUGADOR:
                MoverMsg moverRecMsg = JsonUtility.FromJson<MoverMsg>(recMsg);
                
                moverJugadorData(moverRecMsg);
                break;

            case Commands.JUGADOR_READY:
                JugadorReady jugadorReady = JsonUtility.FromJson<JugadorReady>(recMsg);
                jugadoresReady.Add(jugadorReady.idJugador);

                jugadoresSimulados[jugadorReady.idJugador].SetActive(true);
                jugadoresNombres[jugadorReady.idJugador].SetActive(true);
                
                jugadoresNombres[jugadorReady.idJugador].GetComponent<ScriptTexto>().jugador = jugadoresSimulados[jugadorReady.idJugador];
                jugadoresNombres[jugadorReady.idJugador].GetComponent<TextMesh>().text = jugadorReady.nombre;

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

                itemGet(itemMsg.indexItem, itemMsg.tipo);

                break;

            case Commands.DISPARO:
                DisparoMsg disparoMsg = JsonUtility.FromJson<DisparoMsg>(recMsg);
                
                jugadoresSimulados[disparoMsg.idPlayer].transform.GetChild(2).gameObject.SetActive(true);

                DisparoMsg disparoEnviarMsg = new DisparoMsg();
                disparoEnviarMsg.idPlayer = disparoMsg.idPlayer;
                for (int i = 0; i < m_connections.Length; i++)
                {
                    SendToClient(JsonUtility.ToJson(disparoEnviarMsg), m_connections[i]);
                }

                break;

            case Commands.ACTIVAR_ARENA:
                ActivarArenaMsg activarArenaMsg = JsonUtility.FromJson<ActivarArenaMsg>(recMsg);

                jugadoresSimulados[activarArenaMsg.idPlayer].transform.GetChild(1).gameObject.SetActive(true);

                ActivarArenaMsg activarClienterMsg = new ActivarArenaMsg();
                activarClienterMsg.idPlayer = activarArenaMsg.idPlayer;
                for (int i = 0; i < m_connections.Length; i++)
                {
                    SendToClient(JsonUtility.ToJson(activarClienterMsg), m_connections[i]);
                }

                break;

            case Commands.ARENA:
                ArenaMsg arenaMsg = JsonUtility.FromJson<ArenaMsg>(recMsg);

                if(arenaMsg.idJugador == 0)
                {
                    myPlayer.GetComponent<MovimientoJugador>().arena();
                }
                else
                {
                    ArenaMsg arenaClienteMsg = new ArenaMsg();
                    for (int i = 0; i < m_connections.Length; i++)
                    {
                        SendToClient(JsonUtility.ToJson(arenaClienteMsg), m_connections[i]);
                    }
                }

                break;

            case Commands.FIN_PLAYER:
                FinPlayerMsg finPlayerMsg = JsonUtility.FromJson<FinPlayerMsg>(recMsg);
                Debug.Log("Jugador " + finPlayerMsg.idJugador + " fin");

                jugadoresSimulados[finPlayerMsg.idJugador].SetActive(false);
                jugadoresNombres[finPlayerMsg.idJugador].SetActive(false);
                numJugadoresFin++;

                for (int i = 0; i < m_connections.Length; i++)
                {
                    SendToClient(JsonUtility.ToJson(finPlayerMsg), m_connections[i]);
                }

                break;

            default:
                Debug.Log("Mensaje desconocido");
                break;
        }
    }

    async void moverJugadorData(MoverMsg moverRecMsg)
    {
        if (moverRecMsg.skin != -1)
            jugadoresSimulados[int.Parse(moverRecMsg.jugador.id)].GetComponent<MeshRenderer>().material = MainMenuController.materialesStatic[moverRecMsg.skin + 1];

        if (moverRecMsg.jugador.id != "0")
        {
            GameObject personaje = jugadoresSimulados[int.Parse(moverRecMsg.jugador.id)];

            personaje.transform.position = moverRecMsg.jugador.posJugador;
            personaje.transform.rotation = moverRecMsg.jugador.rotacion;

            GameObject texto = jugadoresNombres[int.Parse(moverRecMsg.jugador.id)];
            texto.transform.position = moverRecMsg.posTextJugador;
        }

        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(moverRecMsg), m_connections[i]);
        }
    }
    #endregion

    public void movimiento(Vector3 pos, Quaternion rotacion)
    {
        MoverMsg moverMsg = new MoverMsg();
        moverMsg.jugador.id = 0 + "";
        moverMsg.jugador.posJugador = pos;
        moverMsg.jugador.rotacion = rotacion;
        moverMsg.jugador.nombre = MainMenuController.nombreCuentaJugador;
        moverMsg.skin = MainMenuController.skinActual;

        moverMsg.posTextJugador = myText.transform.position;


        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(moverMsg), m_connections[i]);
        }
    }

    public void itemGet(int index, string tipo)
    {
        ItemMsg itemMsg = new ItemMsg();
        itemMsg.indexItem = index;
        itemMsg.tipo = tipo;

        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(itemMsg), m_connections[i]);
        }
    }

    public void disparo()
    {
        DisparoMsg disparoMsg = new DisparoMsg();
        disparoMsg.idPlayer = 0;
        
        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(disparoMsg), m_connections[i]);
        }
    }

    public void activarArena()
    {
        ActivarArenaMsg activarArenaMsg = new ActivarArenaMsg();
        activarArenaMsg.idPlayer = 0;

        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(activarArenaMsg), m_connections[i]);
        }
    }

    public void efectoArena(int idJugador)
    {
        ArenaMsg arenaMsg = new ArenaMsg();
        arenaMsg.idJugador = idJugador;
        Debug.Log(idJugador);

        //SendToClient(JsonUtility.ToJson(arenaMsg), m_connections[arenaMsg.idJugador]);
        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(arenaMsg), m_connections[i]);
        }
    }

    public void finPlayer()
    {
        FinPlayerMsg finPlayerMsg = new FinPlayerMsg();
        finPlayerMsg.idJugador = 0;

        myPlayer.SetActive(false);
        myText.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<CameraScript>().cambioFin();
        numJugadoresFin++;

        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(finPlayerMsg), m_connections[i]);
        }
    }

    public void terminarPartida()
    {
        jugando = false;
        CambiarEscena cambiarEscena = new CambiarEscena();

        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(cambiarEscena), m_connections[i]);
        }
        finjuego = true;

    }
}
