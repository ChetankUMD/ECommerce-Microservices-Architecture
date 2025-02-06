using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs
{
    // Dominr typr objects
    public record ProductDTO
        (
        int Id,
        [Required] string Name,
        [Required, DataType(DataType.Currency)] decimal Price,
        [Required, Range(1, int.MaxValue)] int Quantity
        );
}
