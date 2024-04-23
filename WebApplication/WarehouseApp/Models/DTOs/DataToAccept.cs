using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.DTOs;

public class DataToAccept
{
    [Required]
    [Range(1, int.MaxValue) ]
    public int IdProduct { get;set; }
    [Required]
    [Range(1, int.MaxValue) ]
    public int IdWarehouse { get; set; }
    [Required]
    [Range(1, int.MaxValue) ]
    public int Amount { get; set; }
    [Required]
    public DateTime Date { get; set; }
}