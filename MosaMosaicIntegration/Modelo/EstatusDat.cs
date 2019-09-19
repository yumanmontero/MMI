using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class EstatusDat
    {
        public String codigo;
        public List<String> mensajeProgramador;
        public List<String> mensajeUsuario;


        public EstatusDat()
        {
            //Init
            mensajeProgramador = new List<String>();
            mensajeUsuario = new List<String>();
        }

        public String getCodigo()
        {
            return codigo;
        }
        public List<String> getMensajeProgramador()
        {
            return mensajeProgramador;
        }
        public List<String> getMensajeUsuario()
        {
            return mensajeUsuario;
        }
        public void setCodigo(String codigo)
        {
            this.codigo = codigo;
        }
        public void setMensajeProgramador(List<String> mensajeProgramador)
        {
            this.mensajeProgramador = mensajeProgramador;
        }
        public void setMensajeUsuario(List<String> mensajeUsuario)
        {
            this.mensajeUsuario = mensajeUsuario;
        }
        public void addMensajeProgramador(String mensajeProgramador)
        {
            this.mensajeProgramador.Add(mensajeProgramador);
        }
        public void addMensajeUsuario(String mensajeUsuario)
        {
            this.mensajeUsuario.Add(mensajeUsuario);
        }
    }
}
