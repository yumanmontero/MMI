using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TaquillaDesactivarResponse
    {
        [JsonProperty("taquilla")]
        public TaquillaDat taquilla { set; get; }
        [JsonProperty("estatus")]
        public EstatusDat estatus { set; get; }


    }
}
