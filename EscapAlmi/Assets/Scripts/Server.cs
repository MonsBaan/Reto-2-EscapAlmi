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

    [Header("MODELO DE JUGADOR")]
    public GameObject prefabJugador;
    public GameObject prefabNombreJugador;


    [Header("PERSONAJE QUE SE ESTA CONTROLANDO")]
    public GameObject myPlayer;

    private bool jugando = false;

    void Start()
    {
        //CREACION DE TANTOS PERSONAJES COMO JUGADORES HABIA EN LA SALA DE ESPERA
        for (int i = 0; i < ServerSalaEspera.numJugadores + 1; i++)
        {
            GameObject jugadorSimulado = Instantiate(prefabJugador);
            jugadorSimulado.GetComponent<JugadorSimuladoScript>().enabled = true;

            jugadoresSimulados.Add(jugadorSimulado);
        }
        //LE ASIGNO EL PRIMER JUGADOR AL SERVIDOR
        myPlayer = jugadoresSimulados[0];
        myPlayer.GetComponent<MovimientoJugador>().enabled = true;
        myPlayer.GetComponent<JugadorSimuladoScript>().enabled = false;


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

        if (jugadores.Count == ServerSalaEspera.numJugadores && !jugando)
        {
            ReadyMsg readyMsg = new ReadyMsg();
            readyMsg.playerList = jugadores;
            readyMsg.indexMap = ElegirMapa.indexMapa;
            readyMsg.numJugadores = m_connections.Length;

            Debug.Log("Jugadores: " + jugadores.Count + " | Conexiones: " + m_connections.Length);

            for (int i = 0; i < m_connections.Length; i++)
            {
                SendToClient(JsonUtility.ToJson(readyMsg), m_connections[i]);
            }
            jugando = true;
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
        foreach (var jugador in mantenerConexion.jugadoresReady)
        {
            jugadoresSimulados[jugador].SetActive(true);
        }
        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(mantenerConexion), m_connections[i]);
        }
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
                if (moverRecMsg.jugador.id != "0")
                {
                    GameObject personaje = jugadoresSimulados[int.Parse(moverRecMsg.jugador.id)];

                    personaje.transform.position = moverRecMsg.jugador.posJugador;
                    personaje.transform.rotation = moverRecMsg.jugador.rotacion;
                }

                for (int i = 0; i < m_connections.Length; i++)
                {
                    SendToClient(JsonUtility.ToJson(moverRecMsg), m_connections[i]);
                }

                break;
            case Commands.JUGADOR_READY:
                JugadorReady jugadorReady = JsonUtility.FromJson<JugadorReady>(recMsg);
                jugadoresReady.Add(jugadorReady.idJugador);
                jugadoresSimulados[jugadorReady.idJugador].SetActive(true);

                break;


            default:
                Debug.Log("Mensaje desconocido");
                break;
        }
    }

    #endregion

    public void movimiento(Vector3 pos, Quaternion rotacion)
    {

        MoverMsg moverMsg = new MoverMsg();
        moverMsg.jugador.id = 0 + "";
        moverMsg.jugador.posJugador = pos;
        moverMsg.jugador.rotacion = rotacion;

        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(moverMsg), m_connections[i]);
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

    private void SendToClient(string message, NetworkConnection c)
    {
        //var writer = m_Driver.BeginSend(NetworkPipeline.Null, c);
        DataStreamWriter writer;
        m_Driver.BeginSend(NetworkPipeline.Null, c, out writer);
        NativeArray<byte> bytes = new
            NativeArray<byte>(Encoding.ASCII.GetBytes(message), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }
}
