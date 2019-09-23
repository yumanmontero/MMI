using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class TrazaDat
    {
        public Boolean wasCompleted;
        public string traza;
        public string operacion;
        public int tipoOficina;
        public TicketDat ticket;
        public List<TransactionDat> lstTrassaction;

        public TrazaDat()
        {
            ticket = new TicketDat();
            lstTrassaction = new List<TransactionDat>();
            wasCompleted = false;
        }
    }
}
