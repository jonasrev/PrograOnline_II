using System.Collections.Generic;
using UnityEngine;

public class EstadisticasManager : MonoBehaviour
{
    public GameObject panelEstadisticas;
    public GameObject prefabFila;
    private Transform contenido;

    private List<DatosJugador> jugadores = new List<DatosJugador>();

    public void RecibirDatosJugador(DatosJugador datos)
    {
        // Si ya existe, actualiza sus datos
        DatosJugador existente = jugadores.Find(x => x.player == datos.player);

        if (existente != null)
        {
            existente.puntos = datos.puntos;
            existente.kills = datos.kills;
            existente.team = datos.team;
            existente.nombre = datos.nombre;
        }
        else
        {
            jugadores.Add(datos);
        }
    }

    public void MostrarPanel()
    {
        panelEstadisticas.SetActive(true);

        contenido = panelEstadisticas.transform;

        foreach (DatosJugador jugador in jugadores)
        {
            GameObject fila = Instantiate(prefabFila, contenido);

            TMPro.TMP_Text[] textos = fila.GetComponentsInChildren<TMPro.TMP_Text>();

            textos[0].text = jugador.nombre.ToString();
            textos[1].text = $"Puntos: {jugador.puntos}";
            textos[2].text = $"Eliminaciones: {jugador.kills}";
            textos[3].text = jugador.team.ToString(); // luego pondrás el team tú
        }
    }

    public void OcultarPanel()
    {
        // Elimina todas las filas creadas
        foreach (Transform hijo in panelEstadisticas.transform)
        {
            Destroy(hijo.gameObject);
        }

        // Limpia la lista para la siguiente partida
        jugadores.Clear();

        // Oculta el panel
        panelEstadisticas.SetActive(false);
    }
}
