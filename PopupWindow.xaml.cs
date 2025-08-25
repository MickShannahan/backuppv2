namespace backuppv2;

public partial class PopupWindow : ContentPage
{
  public PopupWindow()
  {
    InitializeComponent();
  }

  protected override void OnAppearing()
  {
    Console.WriteLine("Popup Appearing");
    base.OnAppearing();

#if WINDOWS
    var parentWindow = this.GetParentWindow();
    if (parentWindow == null) return;
    parentWindow.SetAsTransientWindow();

#endif
  }
}