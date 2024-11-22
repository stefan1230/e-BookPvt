using System.ComponentModel.DataAnnotations;

namespace e_BookPvt.Models
{
    public class Customers
    {
        [Key]
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ContactNo { get; set; }
        public string NICNo { get; set; }

    }
}
