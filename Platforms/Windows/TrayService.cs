using Hardcodet.Wpf.TaskbarNotification.Interop;
using Microsoft.UI.Xaml;
using backuppv2.Services;

namespace backuppv2.WinUI;

public class TrayService : ITrayService
{
  WindowsTrayIcon tray;

  public Action LeftClickHandler { get; set; }
  public Action RightClickHandler { get; set; }

  public void Initialize()
  {
    tray = new WindowsTrayIcon("Resources/AppIcon/ArbysGundam1.ico");
    tray.LeftClick = () =>
    {
      WindowExtensions.BringToFront();
      LeftClickHandler?.Invoke();
    };
    tray.RightClick = () =>
    {
      WindowExtensions.BringToFront();
      RightClickHandler?.Invoke();
    };
  }
}