using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo
{
    class ApplicationConstants
    {
        public static String appName = "mmi";

        /*System Constrait*/
        public static String ACTIVO = "A";
        public static String INACTIVO = "I";
        public static String TICKET_NO_ASIGNADO_INDIC_ATEN = "O";
        public static String TICKET_NO_ASIGNADO_INDIC_ACTIVA = "A";


        /*Mensaje Cod.*/
        public static String TRANSACCION_EXITOSA = "0000";
        public static String NO_EXISTE_TICKET_EN_COLA = "0004";
        public static String NO_HAY_TAQUILLAS_CON_ESTATUS = "0003";
        public static String FECHA_INVALIDA = "0005";
        public static String FECHA_MEDICION_NO_EXISTE = "0006";
        public static String ERROR_BASE_DATOS = "0007";
        public static String DATO_INGRESADO_INVALIDO = "0008";
        public static String TICKET_NO_EXISTE = "0009";
        public static String ERROR_POR_CLIENTE_ATENDIDO = "0010";
        public static String ERROR_POR_CLIENTE_EN_ATENCION = "0011";
        public static String ERROR_DE_AUTORIZACION = "0012";
        public static String NO_REGISTROS_EN_TABLA = "0013";
        public static String TAQUILLA_NO_EXISTE = "0014";
        public static String TAQUILLA_INACTIVA = "0015";
        public static String TRANSACCION_NO_EXISTE = "0016";
        public static String LISTA_TRANSACCION_VACIA = "0017";
        public static String PETICION_SIN_DATOS = "9000";
        public static String ERROR_DE_COMUNICACION = "9999";

        /*DATE FORMAT*/
        public static String dfus = "yyyy-MM-dd";
        public static String dfustime = "yyyy-MM-dd HH:mm:ss";
        public static String dfstd = "dd/MM/yyyy";
        public static String dfform = "yyyyMMddHHmmss";
        public static String dfparm = "yyyyMMdd";
        public static String dfparmtime = "HHmmss";
        public static String dfparmtimewithsep = "HH:mm:ss";
        public static String dfparmwithsep = "MM/dd/yyyy";
        public static String dfdispdate = "dd/MM/yyyy hh:mm:ss tt";
        public static String dfdispdateval = "dd/MM/yyyy HH:mm:ss";
        public static String dfstdval = "dd/MM/yyyy";
        public static String dftimehour = "HH";
        public static String dftimemin = "mm";
        public static String dftime = "HH:mm tt";



        /*Mensaje Programador Dat Validadores*/
        public static String MP_FALTA_COD_OFICINA = "Código de oficina registrado es inválido o no existe";
        public static String MP_FALTA_FECHA_ATENCION = "Fecha no fue ingresada";
        public static String MP_INVALIDA_FECHA_ATENCION = "Fecha ingresada es inválida";
        public static String MP_FALTA_NRO_TICKET = "Número de ticket ingresado es inválido o no existe";
        public static String MP_FALTA_ID_CEDULA = "Identificador de cédula de identidad ingresado es inválido";
        public static String MP_FALTA_NRO_CEDULA = "Número de cédula de identidad ingresado es inválido";
        public static String MP_FALTA_NRO_TERMINAL = "Número del terminal registrado es inválido o no existe";
        public static String MP_FALTA_IND_ACTIVO = "Índice de Activación ingresado es inválido";
        public static String MP_FALTA_COD_TIPO_COLA = "Opción de tipo de atención no fue selecionada";
        public static String MP_INVALIDA_COD_TIPO_COLA = "Opción de tipo de atención selecionada es inválida";
        public static String MP_FALTA_IND_ATENCION = "Índice de Atención ingresado es inválido";
        public static String MP_FALTA_TRANSACCION = "Lista de transacción esta vacia";



        /*Mensaje Usuario Dat Validadores*/
        public static String MU_FALTA_COD_OFICINA = "Dato ingresado es inválido";
        public static String MU_FALTA_FECHA_ATENCION = "Favor ingresar una fecha";
        public static String MU_INVALIDA_FECHA_ATENCION = "Favor ingresar una fecha valida";
        public static String MU_FALTA_NRO_TICKET = "Favor ingresar número de ticket";
        public static String MU_FALTA_ID_CEDULA = "Seleccione la identificación de cédula valido";
        public static String MU_FALTA_NRO_CEDULA = "Favor ingresar número de cedula de identidad";
        public static String MU_FALTA_NRO_TERMINAL = "Número del terminal registrado es inválido o no existe";
        public static String MU_FALTA_IND_ACTIVO = "Dato ingresado es inválido";
        public static String MU_FALTA_COD_TIPO_COLA = "Seleccione tipo de atención";
        public static String MU_INVALIDA_COD_TIPO_COLA = "Seleccione tipo de atención valido";
        public static String MU_FALTA_IND_ATENCION = "Dato ingresado es inválido";
        public static String MU_FALTA_TRANSACCION = "Lista de transacción esta vacia";



        /*Configuracion Aplicacion*/
        public static string serviceEndpoint = "";
        public static string ticketCRUDEndpoint = "/tickets";
        public static string taquillaLogInEndpoint = "/taquilla/activar";
        public static string taquillaLogOutEndpoint = "/taquilla/desactivar";
        public static string taquillaLlamarEndpoint = "/taquilla/proximo";
        public static string testServiceEndpoint = "/validate/";
        public static string tipoColaCRUDEndpoint = "/tipo_colas";
        public static string transaccionCRUDEndpoint = "/tickets/transaccion";
        public static Boolean modtest = false;
        public static Boolean moddebug = false;
        public static string timeZone = "America/La_Paz";
        public static string codTranLogin = "login";
        public static string codTranLogout = "logout";
        public static string codTranCallNext = "callNext";

        public static int timeoutGET = 10000;
        public static int timeoutPOST = 10000;
        public static int timeoutPUT = 10000;
        public static int timeoutPATCH = 10000;
        public static int timeoutDELETE = 10000;



    }
}
