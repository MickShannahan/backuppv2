namespace backuppv2.Models;

public class Todo
{
  public int? Id { get; set; }
  public string? Name { get; set; }
  public bool Complete { get; set; } = false;
}