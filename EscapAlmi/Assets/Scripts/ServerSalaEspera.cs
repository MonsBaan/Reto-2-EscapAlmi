using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;
using System.Text;
using System;
using Random = UnityEngine.Random;
using NetworkObject.NetworkMessages;
using UnityEngine.SceneManagement;

public class ServerSalaEspera : MonoBehaviour
{
    public NetworkDriver m_Driver;
    [Header("Puerto de Conexion")]
    public ushort serverPort;

    public NativeList<NetworkConnection> m_connections;

    public bool juegoEmpezado = false;
    public GameObject btnEmpezar;
    public List<NetworkObject.NetworkObject.Jugador> listaJugadores;

    public static int numJugadores;


    void Start()
    {
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = serverPort;
        if (m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("Fallo enlazando el puerto: " + serverPort);
            //Posiblemente el puerto este siendo utilizado si salta este error
        }
        else
        {
            m_Driver.Listen();
            GetComponent<ScriptSalaEspera>().añadirJugador("Server");
            NetworkObject.NetworkObject.Jugador jugadorNuevo = new NetworkObject.NetworkObject.Jugador();
            jugadorNuevo.nombre = "Servidor"; //CAMBIAR ESTO CUANDO TENGAMOS LOGIN DE USER
            listaJugadores.Add(jugadorNuevo);
            btnEmpezar.SetActive(true);
            Debug.Log("Servidor Abierto: " + serverPort);

        }
        m_connections = new NativeList<NetworkConnection>(20, Allocator.Persistent);

    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();
        //Limpiar las conexiones
        limpiarConexiones();
        //MANTENER CONEXIONES ABIERTAS
        refrescarConexion();
        //Aceptar nuevas conexiones
        aceptarConexiones();
        //LEER MENSAJES QUE LLEGAN AL SERVIDOR
        messageListener();

    }

    private void OnData(DataStreamReader stream, int numJugador)
    {
        //SE DECODIFICA EL MENSAJE RECIBIDO
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string msgRecibido = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(msgRecibido);

        //SELECCION DE COMANDO
        switch (header.command)
        {
            case Commands.HANDSHAKE_SALAESPERA:
                HandshakeSalaEspera handshake = JsonUtility.FromJson<HandshakeSalaEspera>(msgRecibido);
                if (handshake.player.nombre.Equals(""))
                {
                    handshake.player.nombre = "Invitado"+listaJugadores.Count;
                }
                listaJugadores.Add(handshake.player);
                break;
        }


    }
    //  PARSEAR DATOS PARA ENVIAR AL CLIENTE
    private void SendToClient(string message, NetworkConnection c)
    {
        DataStreamWriter writer;
        m_Driver.BeginSend(NetworkPipeline.Null, c, out writer);
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(message), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    private void OnDisconnect(int i)
    {
        m_connections[i] = default(NetworkConnection);
        m_connections.RemoveAtSwapBack(i);
    }

    public void OnDestroy()
    {
        m_connections.Dispose();
        m_Driver.Dispose();
    }

    private void OnConnect(NetworkConnection connection)
    {
        m_connections.Add(connection);
        Debug.Log("Conexion Aceptada - Numero de Jugadores: " + m_connections.Length);

        HandshakeSalaEspera handshake = new HandshakeSalaEspera();
        handshake.player.id = connection.InternalId.ToString();
        SendToClient(JsonUtility.ToJson(handshake), connection);

    }

    //FUNCIONES DE CONEXION
    private void refrescarConexion()
    {
        int numJugadores = m_connections.Length;
        for (int i = 0; i < numJugadores; i++)
        {
            MantenerConexionSalaEspera mantenerConexion = new MantenerConexionSalaEspera();
            mantenerConexion.listaJugadores = listaJugadores;
            SendToClient(JsonUtility.ToJson(mantenerConexion), m_connections[i]);
        }
    }
    private void aceptarConexiones()
    {
        NetworkConnection c = m_Driver.Accept();
        while (c != default(NetworkConnection))
        {
            OnConnect(c);
            c = m_Driver.Accept();
        }
    }
    private void limpiarConexiones()
    {
        for (int i = 0; i < m_connections.Length; i++)
        {
            if (!m_connections[i].IsCreated)
            {
                m_connections.RemoveAtSwapBack(i);
                i--;
            }
        }
    }
    private void messageListener()
    {
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
                    Debug.Log("Jugador " + i+1 + " desconectado");
                    listaJugadores.RemoveAt(i+1);

                }
                //pasamos al siguiente mensaje
                cmd = m_Driver.PopEventForConnection(m_connections[i], out stream);
            }
        }

    }
    public void empezarJuego()
    {

        numJugadores = m_connections.Length;
        SceneManager.LoadScene("InGame");

        CambiarEscena cambiarEscena = new CambiarEscena();
        for (int i = 0; i < m_connections.Length; i++)
        {
            SendToClient(JsonUtility.ToJson(cambiarEscena), m_connections[i]);
        }


    }
}
