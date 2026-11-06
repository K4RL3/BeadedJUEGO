using System.Collections.Generic;
using UnityEngine;

public static class PedidoMemoria
{
    // Guarda todos los pedidos de la partida
    public static List<Sprite[]> PedidosActivos = new List<Sprite[]>();

    // Guarda los nombres de los NPCs que ya pidieron
    public static List<string> NpcsAtendidos = new List<string>();

    // Variables para la posición del NPC en la Estación 1
    public static Vector3 PosicionNPCGuardada;
    public static bool NPCLlegoALaCaja = false;
}