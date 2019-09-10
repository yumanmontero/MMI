using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MosaIntegration.Modelo;

namespace MosaIntegration.Controlador
{
    class ApplicationProperties
    {
	    public static Dictionary<String, String> codigoadmin = new Dictionary<String, String>();
        public static Dictionary<String, String> codigousuario = new Dictionary<String, String>();
        public static Dictionary<String, String> hmactivo = new Dictionary<String, String>();
        public static Dictionary<String, String> hmatencion = new Dictionary<String, String>();

        public ApplicationProperties()
        {

            SetUpMensaje();
            SetUpCodAtencion();

        }



        public void SetUpMensaje()
        {
            //MensajeAdministrador
            codigoadmin[ApplicationConstants.TRANSACCION_EXITOSA]="TRANSACCION EXITOSA";
            codigoadmin[ApplicationConstants.NO_EXISTE_TICKET_EN_COLA]="TRANSACCION EXITOSA";
            codigoadmin[ApplicationConstants.FECHA_INVALIDA]="Fecha invalida o error de acceso a la bd";
            codigoadmin[ApplicationConstants.FECHA_MEDICION_NO_EXISTE]="Fecha de medicion no existe";
            codigoadmin[ApplicationConstants.ERROR_BASE_DATOS]="Error al procesar la operacion en la base de datos";
            codigoadmin[ApplicationConstants.DATO_INGRESADO_INVALIDO]="Datos ingresados son invalidos";
            codigoadmin[ApplicationConstants.TICKET_NO_EXISTE]="Ticket no existe";
            codigoadmin[ApplicationConstants.ERROR_POR_CLIENTE_ATENDIDO]="No se puede ? ticket debido a que el cliente ya fue atendido";
            codigoadmin[ApplicationConstants.ERROR_POR_CLIENTE_EN_ATENCION]="No se puede ? ticket debido a que el cliente esta siendo atendido";
            codigoadmin[ApplicationConstants.ERROR_DE_AUTORIZACION]="No cuenta con la autorizacion para ? un ticket activo luego de que paso su fecha de atencion";
            codigoadmin[ApplicationConstants.NO_REGISTROS_EN_TABLA]="No existen registros en ?";
            codigoadmin[ApplicationConstants.TAQUILLA_NO_EXISTE]="La taquilla no existe";
            codigoadmin[ApplicationConstants.NO_HAY_TAQUILLAS_CON_ESTATUS]="No existen taquillas disponibles con el estatus definido ?";
            codigoadmin[ApplicationConstants.PETICION_SIN_DATOS]="Peticion del servicio no contiene datos.";
            codigoadmin[ApplicationConstants.TAQUILLA_INACTIVA]="La taquilla se encuentra inactiva.";
            codigoadmin[ApplicationConstants.ERROR_DE_COMUNICACION]="Existe una falla de comunicación con el servidor: ?";


            //MensajeUsuario
            codigousuario[ApplicationConstants.TRANSACCION_EXITOSA]="TRANSACCION EXITOSA";
            codigousuario[ApplicationConstants.NO_EXISTE_TICKET_EN_COLA]="No existen tickets pendientes o cerrados para la fecha de atencion";
            codigousuario[ApplicationConstants.FECHA_INVALIDA]="Fecha invalida o error de acceso a la bd";
            codigousuario[ApplicationConstants.FECHA_MEDICION_NO_EXISTE]="Fecha de medicion no existe";
            codigousuario[ApplicationConstants.ERROR_BASE_DATOS]="Error de acceso a la BD";
            codigousuario[ApplicationConstants.DATO_INGRESADO_INVALIDO]="Datos ingresados son invalidos";
            codigousuario[ApplicationConstants.TICKET_NO_EXISTE]="Ticket no encontrado";
            codigousuario[ApplicationConstants.ERROR_POR_CLIENTE_ATENDIDO]="No se puede ? ticket debido a que el cliente ya fue atendido";
            codigousuario[ApplicationConstants.ERROR_POR_CLIENTE_EN_ATENCION]="No se puede ? ticket debido a que el cliente esta siendo atendido";
            codigousuario[ApplicationConstants.ERROR_DE_AUTORIZACION]="No cuenta con la autorizacion para ? un ticket activo luego de que paso su fecha de atencion";
            codigousuario[ApplicationConstants.NO_REGISTROS_EN_TABLA]="No existen registros en ?";
            codigousuario[ApplicationConstants.TAQUILLA_NO_EXISTE]="La taquilla no existe";
            codigousuario[ApplicationConstants.NO_HAY_TAQUILLAS_CON_ESTATUS]="No existen taquillas disponibles con el estatus definido ?";
            codigousuario[ApplicationConstants.PETICION_SIN_DATOS]="En estos momentos no se puede ejecutar la transaccion";
            codigousuario[ApplicationConstants.TAQUILLA_INACTIVA]="La taquilla se encuentra inactiva. Por favor inicie session.";
            codigousuario[ApplicationConstants.ERROR_DE_COMUNICACION]="En estos momentos, no se puede procesar su solicitud.";



        }

        public void SetUpCodAtencion()
        {
            hmactivo["A"]="Cliente en espera";
            hmactivo["C"]="Cliente atendido con ticket";
            hmactivo["S"]="Sin Clientes";
            hmactivo["F"]="Cliente atendido sin ticket";
            hmactivo["B"]="Cliente atendido (turno perdido)";
            hmactivo["BO"]="Abandono";


            hmactivo["K"]="Activo (via kiosko)";
            hmactivo["P"]="Procesado por mosaic(kiosko)";
            hmactivo["L"]="Cliente LLamado (via kiosko)"; /*new*/
            hmactivo["T"]="Cliente en Taquilla (via kiosko"; /*new*/
            hmactivo["Y"]="LLamando Cliente (via kiosko"; /*new*/


            hmatencion["TO"]="Cliente siendo atendido"; /*new*/

            hmatencion["O"]="No se ha atendido";
            hmatencion["SO"]="Sin cliente";
            hmatencion["A"]="Atencion (Cierre abrupto)";
            hmatencion["N"]="Atencion (Cierre normal)";
            hmatencion["S"]="Atencion (Cierre por parametro)";

        }


    }
}
