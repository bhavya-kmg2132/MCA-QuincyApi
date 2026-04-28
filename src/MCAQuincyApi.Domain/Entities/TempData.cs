using System;
using System.ComponentModel.DataAnnotations;
namespace MCAQuincyApi.Domain.Entities;
public class TempData {
    [Key] public int Id { get; set; }
    [Required] public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public DateTime LastRefreshed { get; set; }
}