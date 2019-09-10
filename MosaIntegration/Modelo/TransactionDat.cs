using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaIntegration.Modelo
{
    class TransactionDat
    {
        public Boolean exists;
        public long nroticket;
        public String fechaatencion;
        public String horallegadaofic;
        public DateTime? horallegadaoficD;
        public DateTime? fechaatencionD;
        public String idcedula;
        public long nrocedula;
        public DateTime? horafintrx;
        public long montotrx;
        public long idtrx;
        public String codtrx;
        public long codoficina;

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
