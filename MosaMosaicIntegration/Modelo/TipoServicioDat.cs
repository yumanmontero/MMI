using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class TipoServicioDat
    {
        [JsonProperty("exists")]
        public Boolean exists { get; set; }
        [JsonProperty("prioridad")]
        public int prioridad { get; set; }
        [JsonProperty("codtiposervicio")]
        public int codtiposervicio { get; set; }
        [JsonProperty("destiposervicio")]
        public String destiposervicio { get; set; }
        [JsonProperty("fechamodificacion")]
        public DateTime? fechamodificacion { get; set; }
        [JsonProperty("codestatus")]
        public String codestatus { get; set; }
    }
}
