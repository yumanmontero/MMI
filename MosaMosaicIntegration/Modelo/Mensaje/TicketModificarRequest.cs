using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TicketModificarRequest
    {
        public TicketDat ticket_actual { get; set; }
        public TicketDat ticket_modif { get; set; }
    }
}
