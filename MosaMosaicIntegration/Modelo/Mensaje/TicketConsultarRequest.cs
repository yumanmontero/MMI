using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TicketConsultarRequest
    {
        public int nroticket { get; set; }
        public String idcedula { get; set; }
        public int nrocedula { get; set; }

        public long codoficina { get; set; }
        public String fechaatencion { get; set; }

        public String indatencion { get; set; }
        public String indactivo { get; set; }
        public int codtipocola { get; set; }
        public Boolean allday { get; set; }
    }
}
