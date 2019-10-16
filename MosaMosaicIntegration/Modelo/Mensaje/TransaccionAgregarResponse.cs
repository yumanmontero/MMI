using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TransaccionAgregarResponse
    {
        [JsonProperty("estatus")]
        public EstatusDat estatus { get; set; }
    }
}
