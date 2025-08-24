namespace backuppv2;

public partial class CustomWindow : Window
{
  public CustomWindow()
  {
    InitializeComponent();
    Page = new ContentPage()
    {
      Content = new VerticalStackLayout
      {
        Children = {
          new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
          }
        }
      }
    };
  }
}