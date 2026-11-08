using System.Collections.Generic;
using UnityEngine;

public static class PedidoMemoria
{
    public static List<GameObject> TicketsActivos = new List<GameObject>();
    public static Sprite[] IngredientesActuales;

    // Aquí guardamos el nombre del NPC (ej. "Cliente1")
    public static List<string> NpcsAtendidos = new List<string>();

    public static Vector3 PosicionNPCGuardada;
    public static bool NPCLlegoALaCaja = false;
    public static bool PedidoYaTomado = false;
}