using UnityEngine;

[System.Serializable]
public class Pedido
{
    public string nombreCliente;
    public Sprite[] ingredientes; // <-- El nombre debe ser 'ingredientes'

    public Pedido(string nombre, Sprite[] sprites)
    {
        nombreCliente = nombre;
        ingredientes = sprites;
    }
}