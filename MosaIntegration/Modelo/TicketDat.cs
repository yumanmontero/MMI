using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaIntegration.Modelo
{
    class TicketDat
    {
        public Int32 nroticket;
        public Boolean exists;
        public long codoficina;
        public Int32 codtipocola;
        public String destipocola;
        public long carnetactivacion;
        public String idcedula;
        public Int32 nrocedula;
        public long nroterminal;
        public String indatencion;
        public String indactivo;
        public DateTime? fechaatencionD;
        public DateTime? horallegadaoficD;
        public DateTime? horainiatencionD;
        public DateTime? horafinatencionD;
        public String fechaatencion;
        public String horallegadaofic;
        public String horainiatencion;
        public String horafinatencion;
        public long carnetatencion;
        public Int32 canttransacciones;
        public String codsigla;
        public String descindatencion;
        public String descindactivo;
        public Boolean existstrx;
        public List<TransactionDat> listtrx;

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
