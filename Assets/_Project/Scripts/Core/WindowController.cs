using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    [DllImport("user32.dll")] static extern int  GetWindowLong(System.IntPtr h, int n);
    [DllImport("user32.dll")] static extern int  SetWindowLong(System.IntPtr h, int n, int v);
    [DllImport("user32.dll")] static extern bool SetLayeredWindowAttributes(System.IntPtr h, uint key, byte alpha, uint flags);
    [DllImport("user32.dll")] static extern bool SetWindowPos(System.IntPtr h, System.IntPtr insert, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] static extern bool ReleaseCapture();
    [DllImport("user32.dll")] static extern int  SendMessage(System.IntPtr h, int msg, int wp, int lp);
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumWindowsProc proc, System.IntPtr lp);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(System.IntPtr h, out uint pid);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(System.IntPtr h);

    delegate bool EnumWindowsProc(System.IntPtr hWnd, System.IntPtr lParam);

    const int  GWL_STYLE     = -16;
    const int  GWL_EXSTYLE   = -20;
    const int  WS_BORDER     = 0x00800000;
    const int  WS_DLGFRAME   = 0x00400000;
    const int  WS_THICKFRAME = 0x00040000;
    const int  WS_CAPTION    = WS_BORDER | WS_DLGFRAME;
    const int  WS_EX_LAYERED = 0x00080000;
    const uint LWA_COLORKEY  = 0x00000001;
    const uint SWP_NOMOVE    = 0x0002;
    const uint SWP_NOSIZE    = 0x0001;
    const uint SWP_FRAMECHANGED = 0x0020;
    static readonly System.IntPtr HWND_TOPMOST = new System.IntPtr(-1);

    // COLORREF 0x00BBGGRR — magenta (R=255,G=0,B=255) = 0x00FF00FF
    // Camera background must be set to #FF00FF
    const uint MAGENTA_KEY = 0x00FF00FF;

    System.IntPtr hwnd;
    uint myPid;

    void Start()
    {
#if !UNITY_EDITOR
        myPid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
        StartCoroutine(ApplyWithRetry());
#endif
    }

    IEnumerator ApplyWithRetry()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.1f);
            hwnd = FindOwnWindow();
            if (hwnd != System.IntPtr.Zero)
            {
                Apply();
                yield break;
            }
        }
        Debug.LogError("[WindowController] window handle not found after 2s");
    }

    System.IntPtr FindOwnWindow()
    {
        System.IntPtr found = System.IntPtr.Zero;
        EnumWindows((h, _) =>
        {
            if (!IsWindowVisible(h)) return true;
            uint pid;
            GetWindowThreadProcessId(h, out pid);
            if (pid == myPid) { found = h; return false; }
            return true;
        }, System.IntPtr.Zero);
        return found;
    }

    void Apply()
    {
        // Remove title bar and border
        int style = GetWindowLong(hwnd, GWL_STYLE);
        style &= ~(WS_CAPTION | WS_THICKFRAME);
        SetWindowLong(hwnd, GWL_STYLE, style);

        // Set WS_EX_LAYERED
        int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        exStyle |= WS_EX_LAYERED;
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);

        // Make magenta pixels transparent
        SetLayeredWindowAttributes(hwnd, MAGENTA_KEY, 255, LWA_COLORKEY);

        // Force redraw + always on top
        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
    }

    void Update()
    {
#if !UNITY_EDITOR
        if (hwnd == System.IntPtr.Zero) return;
        if (Input.GetMouseButtonDown(0))
        {
            ReleaseCapture();
            SendMessage(hwnd, 0xA1, 0x2, 0);
        }
#endif
    }
}
