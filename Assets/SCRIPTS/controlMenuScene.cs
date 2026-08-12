
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class controlMenuScene : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (panel.activeSelf)
                DesactivarPanel();
            else
                ActivarPanel();
        }
    }

    // Para un botón de UI
    public void ActivarPanel()
    {
        panel.SetActive(true);

        CameraController.LocalCamera.StopCameraMovement();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DesactivarPanel()
    {
        panel.SetActive(false);

        CameraController.LocalCamera.ResumeCameraMovement();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Alterna entre activo e inactivo
    public void CambiarEstado(GameObject objeto)
    {
        if (objeto != null)
            objeto.SetActive(!objeto.activeSelf);
    }

    // Cerrar el juego
    public void CerrarJuego()
    {
        Debug.Log("Cerrando juego...");
        Application.Quit();
    }

    // Volver al menú
    public async void IrAlMenu()
    {
        NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();

        if (runner != null)
        {
            await runner.Shutdown();
        }

        SceneManager.LoadScene("UI");
    }
}
