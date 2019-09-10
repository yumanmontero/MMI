using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaIntegration.Modelo
{
    class TaquillaDat
    {
        public Boolean exists;
        public int nrotaquilla;
        public int nroterminal;
        public long codoficina;
        public String codestatus;
        public DateTime? fechamodificacion;
        public long carnetaten;

        public List<TipoColaDat> tipocola;


        public TaquillaDat()
        {
            this.tipocola = new List<TipoColaDat>();
        }
    }
}
