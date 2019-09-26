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

        [JsonProperty("nom_red_ofic")]
        public String nom_red_ofic { get; set; }



        public TaquillaDat()
        {
            this.tipocola = new List<TipoColaDat>();
        }
    }
}
