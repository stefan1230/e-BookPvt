using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_BookPvt.Models
{
public class Orders
{
    [Key]
    public int OrderId { get; set; }

    [ForeignKey("Customers")]
    [Required(ErrorMessage = "Customer is required.")]
    public int CustomerID { get; set; } // Foreign key to Customers table

    [ForeignKey("Books")]
    [Required(ErrorMessage = "Book is required.")]
    public int BookID { get; set; } // Foreign key to Books table

    public DateTime Orderdate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Total amount must be valid.")]
    public decimal TotalAmount { get; set; } // Changed to decimal for accurate calculations

    [Required]
    public string Status { get; set; } // Processing, Completed, Cancelled

    // Navigation properties
    public Customers Customer { get; set; }
    public Books Book { get; set; }
}


}
