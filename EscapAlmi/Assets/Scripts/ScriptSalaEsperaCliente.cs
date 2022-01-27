using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;
using NetworkObject.NetworkMessages;
using UnityEngine.SceneManagement;

public class ScriptSalaEsperaCliente : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public static string serverIp;
    public ushort serverPort;
    private GameObject server;
    public bool conectado;

    public GameObject btnEmpezar;

    public List<NetworkObject.NetworkObject.Jugador> listaJugadores;
    public int idJugador;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(serverIp);

        btnEmpezar.SetActive(false);

        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(serverIp, serverPort);
        m_Connection = m_Driver.Connect(endpoint);
        conectado = true;

    }

    void Update()
    {
        if (conectado)
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

            cmd = m_Connection.PopEvent(m_Driver, out stream);
        }
    }

    private void OnData(DataStreamReader stream)
    {
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch (header.command)
        {
            case Commands.HANDSHAKE_SALAESPERA:
                HandshakeSalaEspera handshake = JsonUtility.FromJson<HandshakeSalaEspera>(recMsg);
                idJugador = int.Parse(handshake.player.id);
                break;
            case Commands.MANTENER_CONEXION_SALAESPERA:
                MantenerConexionSalaEspera mantenerConexion = JsonUtility.FromJson<MantenerConexionSalaEspera>(recMsg);
                listaJugadores = mantenerConexion.listaJugadores;
                break;
            case Commands.CAMBIO_ESCENA:
                CambiarEscena cambiarEscena = JsonUtility.FromJson<CambiarEscena>(recMsg);
                SceneManager.LoadScene("InGame");
                OnDisconnect();

                break;
            default:
                Debug.Log("Mensaje desconocido");
                break;
        }
    }




    private void OnConnect()
    {
        Debug.Log("Conexion con el Servidor");
        HandshakeSalaEspera handshake = new HandshakeSalaEspera();
        SendToServer(JsonUtility.ToJson(handshake));

    }
    public void OnDisconnect()
    {
        Debug.Log("Desconectado");
        m_Connection.Disconnect(m_Driver);
    }

    public void OnDestroy()
    {
        m_Connection = default(NetworkConnection);
        m_Driver.Dispose();
    }

    public void SendToServer(string v)
    {
        DataStreamWriter writer;
        m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out writer);
        NativeArray<byte> bytes = new
            NativeArray<byte>(Encoding.ASCII.GetBytes(v), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }




}