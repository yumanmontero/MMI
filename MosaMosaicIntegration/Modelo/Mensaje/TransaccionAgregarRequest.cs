using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TransaccionAgregarRequest
    {
        public List<TransactionDat> transacciones = new List<TransactionDat>();

        internal List<TransactionDat> Transacciones
        {
            get
            {
                return transacciones;
            }

            set
            {
                transacciones = value;
            }
        }
    }
}
