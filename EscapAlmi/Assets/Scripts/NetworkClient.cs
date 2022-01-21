using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;
using UnityEngine.UI;
using NetworkMessages;
using NetworkObject;
using System;

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    [Header("Puerto")]
    public ushort serverPort;
    public List<GameObject> jugadoresGameObject;
    public GameObject prefabJugador;

    private string idPlayer;

    void Start()
    {
        Conectar();
    }

    public void Conectar()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(MainMenuController.ipServer, serverPort);
        m_Connection = m_Driver.Connect(endpoint);
    }

    void Update()
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
                idPlayer = mensajeRecibido.player.id;

                //Genero un nuevo mensaje para enviar la infromacion al servidor
                HandshakeMsg mensajeEnviar = new HandshakeMsg();
                SendToServer(JsonUtility.ToJson(mensajeEnviar));

                break;

            case Commands.READY:
                ReadyMsg readyMsg = JsonUtility.FromJson<ReadyMsg>(recMsg);

                GameObject.Find("Mapas").GetComponent<ElegirMapa>().cargarMapa(readyMsg.indexMap);

                if(jugadoresGameObject.Count < readyMsg.playerList.Count)
                {
                    for (int i = 0; i < (readyMsg.playerList.Count - jugadoresGameObject.Count); i++)
                    {
                        jugadoresGameObject.Add(prefabJugador);
                    }
                }

                break;

            default:
                Debug.Log("Mensaje desconocido");
                break;
        }
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
}
