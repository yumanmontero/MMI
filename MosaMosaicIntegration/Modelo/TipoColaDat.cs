using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class TipoColaDat
    {
        public Boolean exists;
        public Int32 codtipocola;
        public String destipocola;
        public long prioridad;
        public String codestatus;
        public DateTime? fechamodificacion;
        public String codestatusnew;
        public List<TipoServicioDat> tiposerviciolst;

        public TipoColaDat()
        {
            this.tiposerviciolst = new List<TipoServicioDat>();
        }
    }
}
