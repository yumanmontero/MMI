﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    public class TicketAgregarResponse
    {
        [JsonProperty("ticket")]
        public TicketDat ticket { get; set; }
        [JsonProperty("estatus")]
        public EstatusDat estatus { get; set; }

    }
}
