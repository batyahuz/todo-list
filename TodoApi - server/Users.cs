using System.ComponentModel.DataAnnotations;

namespace TodoApi;

public partial class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
