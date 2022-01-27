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

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    [Header("Puerto")]
    public ushort serverPort;

    [Header("LISTA DE PERSONAJES")]
    public List<GameObject> jugadoresSimulados;

    [Header("MODELO DE JUGADOR")]
    public GameObject prefabJugador;
    public GameObject prefabNombreJugador;

    public string idPlayer;

    [Header("PERSONAJE QUE SE ESTA CONTROLANDO")]
    public GameObject myPlayer;

    [Header("LISTA DE JUGADORES READY")]
    public List<int> jugadoresReady;

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
        foreach (var jugador in jugadoresReady)
        {
            jugadoresSimulados[jugador].SetActive(true);
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
                    
                }
                //LE ASIGNO EL PERSONAJE AL CLIENTE
                myPlayer = jugadoresSimulados[int.Parse(mensajeRecibido.player.id)];
                myPlayer.GetComponent<MovimientoJugador>().enabled = true;
                myPlayer.GetComponent<JugadorSimuladoScript>().enabled = false;

                JugadorReady jugadorReady = new JugadorReady();
                jugadorReady.idJugador = int.Parse(idPlayer);
                SendToServer(JsonUtility.ToJson(jugadorReady));
                break;

            case Commands.READY:
                ReadyMsg readyMsg = JsonUtility.FromJson<ReadyMsg>(recMsg);
                GameObject.Find("Mapas").GetComponent<ElegirMapa>().cargarMapa(readyMsg.indexMap);

                break;

            case Commands.MOVER_JUGADOR:
                MoverMsg moverRecMsg = JsonUtility.FromJson<MoverMsg>(recMsg);
                Debug.Log(moverRecMsg.jugador.id + " | " + moverRecMsg.jugador.posJugador);
                if(moverRecMsg.jugador.id != idPlayer)
                {
                    GameObject personaje = jugadoresSimulados[int.Parse(moverRecMsg.jugador.id)];

                    personaje.transform.position = moverRecMsg.jugador.posJugador;
                    personaje.transform.rotation = moverRecMsg.jugador.rotacion;
                }

                break;

            case Commands.MANTENER_CONEXION:
                MantenerConexion mantenerConexion = JsonUtility.FromJson<MantenerConexion>(recMsg);
                jugadoresReady = mantenerConexion.jugadoresReady;



                break;
            default:
                Debug.Log(header.command + " No disponible");
                break;
        }
    }


    #endregion
    public void movimiento(Vector3 pos, Quaternion rotacion)
    {
        MoverMsg moverMsg = new MoverMsg();
        moverMsg.jugador.id = idPlayer;
        moverMsg.jugador.posJugador = pos;
        moverMsg.jugador.rotacion = rotacion;

        SendToServer(JsonUtility.ToJson(moverMsg));
    }


}
