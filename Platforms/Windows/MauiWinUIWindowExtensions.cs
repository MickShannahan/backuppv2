
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using MauiApp = Microsoft.Maui.Controls.Application;
using UIWindow = Microsoft.UI.Xaml.Window;
using MauiWindow = Microsoft.Maui.Controls.Window;

namespace backuppv2;

public static class WindowExtensions
{
  public static IntPtr Hwnd { get; set; }

  public static void SetIcon(string iconFilename)
  {
    if (Hwnd == IntPtr.Zero)
      return;

    var hIcon = PInvoke.User32.LoadImage(IntPtr.Zero, iconFilename,
       PInvoke.User32.ImageType.IMAGE_ICON, 16, 16, PInvoke.User32.LoadImageFlags.LR_LOADFROMFILE);

    PInvoke.User32.SendMessage(Hwnd, PInvoke.User32.WindowMessage.WM_SETICON, (IntPtr)0, hIcon);
  }

  public static void BringToFront()
  {
    PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_SHOW);
    PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_RESTORE);

    _ = PInvoke.User32.SetForegroundWindow(Hwnd);
  }

  public static void MinimizeToTray()
  {
    PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_MINIMIZE);
    PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_HIDE);
  }

  public static void SetAsTransientWindow(this MauiWindow window)
  {
    var appWindow = GetAppWindow(window);

    if (appWindow.Presenter is OverlappedPresenter presenter)
    {
      // Make the window always on top
      presenter.IsAlwaysOnTop = true;

      // Subscribe to the window's "focus lost" event to close it
      window.Deactivated += (sender, args) =>
      {
        MauiApp.Current?.CloseWindow(MauiApp.Current.Windows[0]);
        // // Ensure the window is not the main one before closing
        // if (Application.Current?.Windows.Count > 1)
        // {
        //   Application.Current.CloseWindow(sender as Window);
        // }
      };
    }
  }

  private static AppWindow GetAppWindow(MauiWindow window)
  {
    Console.WriteLine("Getting Window", window.Id);
    var handle = WindowNative.GetWindowHandle(window);
    var windowId = Win32Interop.GetWindowIdFromWindow(handle);
    return AppWindow.GetFromWindowId(windowId);
  }
}
