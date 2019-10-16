using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MosaMosaicIntegration.Modelo
{
    public class TicketDat
    {
        [JsonProperty("nroticket")]
        public Int32 nroticket { get; set; }
        [JsonProperty("exists")]
        public Boolean exists { get; set; }
        [JsonProperty("codoficina")]
        public long codoficina { get; set; }
        [JsonProperty("codtipocola")]
        public Int32 codtipocola { get; set; }
        [JsonProperty("destipocola")]
        public String destipocola { get; set; }
        [JsonProperty("carnetactivacion")]
        public long carnetactivacion { get; set; }
        [JsonProperty("idcedula")]
        public String idcedula { get; set; }
        [JsonProperty("nrocedula")]
        public Int32 nrocedula { get; set; }
        [JsonProperty("nroterminal")]
        public long nroterminal { get; set; }
        [JsonProperty("indatencion")]
        public String indatencion { get; set; }
        [JsonProperty("indactivo")]
        public String indactivo { get; set; }
        [JsonProperty("fechaatencionD")]
        public DateTime? fechaatencionD { get; set; }
        [JsonProperty("horallegadaoficD")]
        public DateTime? horallegadaoficD { get; set; }
        [JsonProperty("horainiatencionD")]
        public DateTime? horainiatencionD { get; set; }
        [JsonProperty("horafinatencionD")]
        public DateTime? horafinatencionD { get; set; }
        [JsonProperty("fechaatencion")]
        public String fechaatencion { get; set; }
        [JsonProperty("horallegadaofic")]
        public String horallegadaofic { get; set; }
        [JsonProperty("horainiatencion")]
        public String horainiatencion { get; set; }
        [JsonProperty("horafinatencion")]
        public String horafinatencion { get; set; }
        [JsonProperty("carnetatencion")]
        public long carnetatencion { get; set; }
        [JsonProperty("canttransacciones")]
        public Int32 canttransacciones { get; set; }
        [JsonProperty("codsigla")]
        public String codsigla { get; set; }
        [JsonProperty("descindatencion")]
        public String descindatencion { get; set; }
        [JsonProperty("descindactivo")]
        public String descindactivo { get; set; }
        [JsonProperty("existstrx")]
        public Boolean existstrx { get; set; }
        [JsonProperty("statustransaccion")]
        public String statustransaccion { get; set; }
        [JsonProperty("listtrx")]
        public List<TransactionDat> listtrx { get; set; }
        [JsonProperty("nom_red_ofic")]
        public String nom_red_ofic { get; set; }

        public Boolean isValid { get; set; }
        public Int32 nroticketcal { get; set; }



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
            //hhinit
            try
            {
                this.horainiatencionD = DateTime.ParseExact(horainiatencion, ApplicationConstants.dfdispdateval, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception i)
            {
                try
                {
                    this.horainiatencionD = new DateTime(long.Parse(horainiatencion));
                }
                catch (Exception e)
                {
                    this.horainiatencionD = null;
                }
            }
            //hhfin
            try
            {
                this.horafinatencionD = DateTime.ParseExact(horafinatencion, ApplicationConstants.dfdispdateval, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception i)
            {
                try
                {
                    this.horafinatencionD = new DateTime(long.Parse(horafinatencion));
                }
                catch (Exception e)
                {
                    this.horafinatencionD = null;
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
