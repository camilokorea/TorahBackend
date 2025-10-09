using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorahBackend.Application.DTO
{
    public class Torah
    {
        public List<LibroDetalle>? libros { get; set; }
        public List<string>? testamentos { get; set; }
        public GlosarioDTO? glosario { get; set; }
        public DedicatoriaDTO? dedicatoria { get; set;}
    }
}
