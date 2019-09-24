using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class TipoColaDat
    {
        [JsonProperty("exists")]
        public Boolean exists { get; set; }
        [JsonProperty("codtipocola")]
        public Int32 codtipocola { get; set; }
        [JsonProperty("destipocola")]
        public String destipocola { get; set; }
        [JsonProperty("prioridad")]
        public long prioridad { get; set; }
        [JsonProperty("codestatus")]
        public String codestatus { get; set; }
        [JsonProperty("fechamodificacion")]
        public DateTime? fechamodificacion { get; set; }
        [JsonProperty("codestatusnew")]
        public String codestatusnew { get; set; }
        [JsonProperty("tiposerviciolst")]
        public List<TipoServicioDat> tiposerviciolst { get; set; }

        public TipoColaDat()
        {
            this.tiposerviciolst = new List<TipoServicioDat>();
        }
    }
}
