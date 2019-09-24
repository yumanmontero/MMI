using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class TaquillaDat
    {
        [JsonProperty("exists")]
        public Boolean exists { set; get; }

        [JsonProperty("nrotaquilla")]
        public int nrotaquilla { set; get; }

        [JsonProperty("nroterminal")]
        public int nroterminal { set; get; }

        [JsonProperty("codoficina")]
        public long codoficina { set; get; }

        [JsonProperty("codestatus")]
        public String codestatus { set; get; }

        [JsonProperty("fechamodificacion")]
        public DateTime? fechamodificacion { set; get; }

        [JsonProperty("carnetaten")]
        public long carnetaten { set; get; }

        [JsonProperty("username")]
        public List<TipoColaDat> tipocola { set; get; }


        public TaquillaDat()
        {
            this.tipocola = new List<TipoColaDat>();
        }
    }
}
