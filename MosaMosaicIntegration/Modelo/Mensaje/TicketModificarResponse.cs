using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TicketModificarResponse
    {
        [JsonProperty("ticket_actual")]
        public TicketDat ticket_actual { get; set; }
        [JsonProperty("ticket_modif")]
        public TicketDat ticket_modif { get; set; }
        [JsonProperty("estatus")]
        public EstatusDat estatus { get; set; }
    }
}
