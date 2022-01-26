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
        var endpoint = NetworkEndPoint.Parse(ScriptSalaEsperaCliente.serverIp, serverPort);
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

                int numJugadores = readyMsg.playerList.Count;
                
                for (int i = 0; i < numJugadores; i++)
                {
                    GameObject nuevoPlayer = Instantiate(prefabJugador);
                    jugadoresGameObject.Add(nuevoPlayer);
                    nuevoPlayer.transform.position = new Vector3(readyMsg.playerList[i].posJugador.x, 2, readyMsg.playerList[i].posJugador.z);
                }
                Debug.Log("id cliente " + idPlayer);
                jugadoresGameObject[int.Parse(idPlayer)].name = "JugadorReal";
                GameObject.Find("Main Camera").GetComponent<CameraScript>().jugadorREAL = jugadoresGameObject[int.Parse(idPlayer)];

                break;

            case Commands.MOVER_JUGADOR:
                MoverMsg moverRecMsg = JsonUtility.FromJson<MoverMsg>(recMsg);

                int idJug = int.Parse(moverRecMsg.jugador.id);
                if (int.Parse(idPlayer) != idJug)
                {
                    jugadoresGameObject[idJug].transform.position = moverRecMsg.jugador.posJugador;
                    jugadoresGameObject[idJug].transform.rotation = moverRecMsg.jugador.rotacion;
                }

                break;


            default:
                Debug.Log("Mensaje desconocido");
                break;
        }
    }

    public void movimiento(Vector3 pos, Quaternion rotacion)
    {
        MoverMsg moverMsg = new MoverMsg();
        moverMsg.jugador.id = idPlayer;
        moverMsg.jugador.posJugador = pos;
        moverMsg.jugador.rotacion = rotacion;

        SendToServer(JsonUtility.ToJson(moverMsg));
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
