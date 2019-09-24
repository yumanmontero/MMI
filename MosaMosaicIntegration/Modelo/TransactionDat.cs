using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class TransactionDat
    {
        [JsonProperty("exists")]
        public Boolean exists { get; set; }
        [JsonProperty("nroticket")]
        public long nroticket { get; set; }
        [JsonProperty("fechaatencion")]
        public String fechaatencion { get; set; }
        [JsonProperty("horallegadaofic")]
        public String horallegadaofic { get; set; }
        [JsonProperty("horallegadaoficD")]
        public DateTime? horallegadaoficD { get; set; }
        [JsonProperty("fechaatencionD")]
        public DateTime? fechaatencionD { get; set; }
        [JsonProperty("idcedula")]
        public String idcedula { get; set; }
        [JsonProperty("nrocedula")]
        public long nrocedula { get; set; }
        [JsonProperty("horafintrx")]
        public DateTime? horafintrx { get; set; }
        [JsonProperty("montotrx")]
        public long montotrx { get; set; }
        [JsonProperty("idtrx")]
        public long idtrx { get; set; }
        [JsonProperty("codtrx")]
        public String codtrx { get; set; }
        [JsonProperty("codoficina")]
        public long codoficina { get; set; }

        public void configFechaHora()
        {
            try
            {
                this.fechaatencionD = DateTime.ParseExact(fechaatencion, ApplicationConstants.dfdispdateval, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception i)
            {
                try
                {
                    this.fechaatencionD = new DateTime(long.Parse(fechaatencion));
                }
                catch (Exception e)
                {
                    this.fechaatencionD = null;
                }
            }
            //hhlegada
            try
            {
                this.horallegadaoficD = DateTime.ParseExact(horallegadaofic, ApplicationConstants.dfdispdateval, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception i)
            {
                try
                {
                    this.horallegadaoficD = new DateTime(long.Parse(horallegadaofic));
                }
                catch (Exception e)
                {
                    this.horallegadaoficD = null;
                }
            }


        }
    }
}
