using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Win32Interop.Methods;

namespace NewGameProject;

public class WindowUtility
{
    static RandomNumberGenerator Rnd = new();
    
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

    public struct Rect {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }
    
    static List<Process> LoadProcesses()
    {
        List<Process> result = new();
        List<Process> processes = Process.GetProcesses().ToList();
        processes.Sort((p1, p2) => p1.ProcessName.CompareTo(p2.ProcessName));
        foreach (var p in processes)
        {
            try
            {
                if (p.BasePriority < 8 ||
                    p.HasExited ||
                    string.IsNullOrEmpty(p.MainWindowTitle))
                    continue;
                var t = p.StartTime; // Try access time
            }
            catch
            {
                continue;
            }
            if (result.Any((q) => p.ProcessName.Contains(q.ProcessName) || q.ProcessName.Contains(p.ProcessName)))
                continue;
            result.Add(p);
        }
        return result;
    }
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    private extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    public static IntPtr GetRandomWindow()
    {
        return GetForegroundWindow();
        
        var list = LoadProcesses();
        while (list.Count > 0)
        {
            int i = Rnd.RandiRange(0, list.Count - 1);
            var p = list[i];
            list.RemoveAt(i);
            if (p.MainWindowHandle == GetForegroundWindow())
                return p.MainWindowHandle;
        }
    } 
    
    public static Vector2 GetWindowSide(IntPtr InHandle)
    {
        Rect rect = new();
        GetWindowRect(InHandle, ref rect);
        return new (
            Rnd.Randf() > 0.5 ? rect.Left : rect.Right,
            Rnd.RandfRange(rect.Top, rect.Bottom)
        );
    } 
    
    public static Vector2 GetWindowTop(IntPtr InHandle)
    {
        Rect rect = new();
        GetWindowRect(InHandle, ref rect);
        return new (
            Rnd.RandfRange(rect.Left, rect.Right),
            rect.Top
        );
    }

    public static void PushWindow(IntPtr InHandle, Vector2I InOffset)
    {
        Rect rect = new();
        GetWindowRect(InHandle, ref rect);
        SetWindowPos(InHandle, IntPtr.Zero, rect.Left + InOffset.X, rect.Top + InOffset.Y, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
    }
}