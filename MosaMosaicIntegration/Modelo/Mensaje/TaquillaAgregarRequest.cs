using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TaquillaAgregarRequest
    {
        public String nom_red_ofic { get; set; }
        public long codoficina { get; set; }
        public String carnetatencion { get; set; }
        public int nrotaq { get; set; }
        public string estado { get; set; }

       
    }
}
