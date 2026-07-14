using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class DraggableWindow : MonoBehaviour
{
    [DllImport("user32.dll")] static extern bool ReleaseCapture();
    [DllImport("user32.dll")] static extern int SendMessage(System.IntPtr hWnd, int msg, int wp, int lp);

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ReleaseCapture();
            SendMessage(Process.GetCurrentProcess().MainWindowHandle, 0xA1, 0x2, 0);
        }
    }
}
