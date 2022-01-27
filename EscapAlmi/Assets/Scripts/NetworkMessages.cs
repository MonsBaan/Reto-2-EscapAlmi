using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Text;
using Unity.Networking.Transport;

namespace NetworkObject
{

    namespace NetworkObject
    {
        [System.Serializable]
        public class NetworkObject
        {
            public string id;
        }

        [System.Serializable]
        public class Jugador : NetworkObject
        {
            public string nombre;
            public Vector3 posJugador;
            public Quaternion rotacion;
        }
    }

    namespace NetworkMessages
    {
        public enum Commands
        {
            HANDSHAKE, HANDSHAKE_SALAESPERA, MANTENER_CONEXION_SALAESPERA, MANTENER_CONEXION, READY, JUGADOR_READY, MOVER_JUGADOR, CAMBIO_ESCENA
        }

        [System.Serializable]
        public class NetworkHeader
        {
            public Commands command;
        }

        [System.Serializable]
        public class HandshakeMsg : NetworkHeader
        {
            public NetworkObject.Jugador player;
            public int numJugadores;
            public HandshakeMsg()
            {
                command = Commands.HANDSHAKE;
                player = new NetworkObject.Jugador();
                numJugadores = -1;
            }
        }

        [System.Serializable]
        public class HandshakeSalaEspera : NetworkHeader
        {
            public NetworkObject.Jugador player;
            public HandshakeSalaEspera()
            {
                command = Commands.HANDSHAKE_SALAESPERA;
                player = new NetworkObject.Jugador();
            }
        }
        [System.Serializable]
        public class MantenerConexionSalaEspera : NetworkHeader
        {
            public List<NetworkObject.Jugador> listaJugadores;
            public MantenerConexionSalaEspera()
            {
                command = Commands.MANTENER_CONEXION_SALAESPERA;

            }
        }
        [System.Serializable]
        public class ReadyMsg : NetworkHeader
        {
            public int idJugador;
            public List<NetworkObject.Jugador> playerList;
            public int indexMap;
            public int numJugadores;
            public ReadyMsg()
            {
                command = Commands.READY;
                playerList = new List<NetworkObject.Jugador>();
                indexMap = -1;
                numJugadores = -1;
            }
        }
        [System.Serializable]
        public class MoverMsg : NetworkHeader
        {
            public NetworkObject.Jugador jugador;
            public MoverMsg()
            {
                command = Commands.MOVER_JUGADOR;
                jugador = new NetworkObject.Jugador();
            }
        }

        [System.Serializable]
        public class CambiarEscena : NetworkHeader
        {
            public NetworkObject.Jugador jugador;
            public CambiarEscena()
            {
                command = Commands.CAMBIO_ESCENA;
            }
        }

        [System.Serializable]
        public class JugadorReady: NetworkHeader
        {
            public int idJugador;
            public JugadorReady()
            {
                command = Commands.JUGADOR_READY;
                idJugador = -1;
            }
        }

        [System.Serializable]
        public class MantenerConexion : NetworkHeader
        {
            public List<int> jugadoresReady;
            public MantenerConexion()
            {
                command = Commands.MANTENER_CONEXION;
                jugadoresReady = new List<int>();
            }
        }

    }

}