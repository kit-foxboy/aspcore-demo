using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
  public int Id { get; set; }
  public required string Url { get; set; }
  public bool IsMain { get; set; }
  public bool IsNSFW { get; set; }
  public required string PublicId { get; set; }

  // Navigation properties
  public int AppUserId { get; set; }
  public AppUser AppUser { get; set; } = null!;
}