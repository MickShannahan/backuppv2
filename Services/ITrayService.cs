namespace backuppv2.Services;

public interface ITrayService
{
  void Initialize();
  Action LeftClickHandler { get; set; }
  Action RightClickHandler { get; set; }
}