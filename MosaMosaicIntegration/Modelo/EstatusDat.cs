using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MosaMosaicIntegration.Modelo
{
    public class EstatusDat
    {
        [JsonProperty("codigo")]
        public String codigo { get; set; }
        [JsonProperty("mensajeProgramador")]
        public List<String> mensajeProgramador { get; set; }
        [JsonProperty("mensajeUsuario")]
        public List<String> mensajeUsuario { get; set; }


        public EstatusDat()
        {
            //Init
            mensajeProgramador = new List<String>();
            mensajeUsuario = new List<String>();
        }

    }
}
