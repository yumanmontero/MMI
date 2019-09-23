using log4net;
using MosaMosaicIntegration.Modelo;
using MosaMosaicIntegration.Modelo.Mensaje;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json;

namespace MosaMosaicIntegration.Controlador
{
    class Application
    {
        private static char sep = Path.DirectorySeparatorChar;
        private static string runDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private static readonly ILog log = LogManager.GetLogger(typeof(Application));
        private static List<Campo> lstCampoHeader =  new List<Campo>();
        private static List<Campo> lstCampoBody = new List<Campo>();
        


        public Application()
        {
            /*Inicializar configuracion*/
            configuracion();
        }

        public Boolean configuracion()
        {
            string configFileName = @"" + runDirectory + sep + ApplicationConstants.appName + ".xml";
            Boolean validator = false;
            if (File.Exists(configFileName))
            {
                var xml = XDocument.Load(configFileName);
                var param = from r in xml.Descendants("entry") select new { key = r.Attribute("key").Value, value = r.Value };
                ApplicationConstants.serviceEndpoint = param.Where(x => x.key.Equals("serviceEndPoint")).FirstOrDefault().value.Trim();
                try
                { 
                    ApplicationConstants.modtest = Boolean.Parse(param.Where(x => x.key.Equals("modetest")).FirstOrDefault().value.Trim());
                    ApplicationConstants.moddebug = Boolean.Parse(param.Where(x => x.key.Equals("debug")).FirstOrDefault().value.Trim());
                }
                catch
                {
                    ApplicationConstants.modtest = false;
                    ApplicationConstants.moddebug = false;
                }
                ApplicationConstants.timeZone = param.Where(x => x.key.Equals("timezone")).FirstOrDefault().value;
                ApplicationConstants.codTranLogin = param.Where(x => x.key.Equals("codtranLogin")).FirstOrDefault().value;
                ApplicationConstants.codTranLogout = param.Where(x => x.key.Equals("codtranLogout")).FirstOrDefault().value;

                /*TIMEOUTS*/
                ApplicationConstants.timeoutGET=  int.Parse(param.Where(x => x.key.Equals("timeoutGET")).FirstOrDefault().value);
                ApplicationConstants.timeoutPOST = int.Parse(param.Where(x => x.key.Equals("timeoutPOST")).FirstOrDefault().value);
                ApplicationConstants.timeoutPATCH = int.Parse(param.Where(x => x.key.Equals("timeoutPATCH")).FirstOrDefault().value);
                ApplicationConstants.timeoutPUT = int.Parse(param.Where(x => x.key.Equals("timeoutPUT")).FirstOrDefault().value);
                ApplicationConstants.timeoutDELETE = int.Parse(param.Where(x => x.key.Equals("timeoutDELETE")).FirstOrDefault().value);


                /*Validaciones*/
                /*Debug.WriteLine("Parametro [timezone] " + ApplicationConstants.timeZone);
                Debug.WriteLine("Parametro [serviceEndpoint] " + ApplicationConstants.serviceEndpoint);
                Debug.WriteLine("Parametro [modetest] " + param.Where(x => x.key.Equals("modetest")).FirstOrDefault().value.Trim());
                Debug.WriteLine("Parametro [debug] " + param.Where(x => x.key.Equals("debug")).FirstOrDefault().value.Trim());*/
                /*Cargar Header de campos*/
                var campoheader = xml.Descendants("entry").Where(x => x.Attribute("key").Value.Equals("camposHeader")).FirstOrDefault().Descendants("campo").Select(r => new { campo = r.Attribute("key").Value, tipo = r.Attribute("type").Value, tam = r.Value });
                var campobody = xml.Descendants("entry").Where(x => x.Attribute("key").Value.Equals("camposBody")).FirstOrDefault().Descendants("campo").Select(r => new { campo = r.Attribute("key").Value, tipo = r.Attribute("type").Value, tam = r.Value });
                foreach (var pa in campoheader)
                {
                    Campo cap = new Campo
                    {
                        dimension = Int32.Parse(pa.tam),
                        nombre = pa.campo,
                        tipo = pa.tipo
                    };
                    lstCampoHeader.Add(cap);
                }
                foreach (var pa in campobody)
                {
                    Campo cap = new Campo
                    {
                        dimension = Int32.Parse(pa.tam),
                        nombre = pa.campo,
                        tipo = pa.tipo
                    };
                    lstCampoBody.Add(cap);
                }

                /*val*/
                /*foreach(var pa in lstCampoHeader)
                {
                    Debug.WriteLine(pa.nombre+" " + pa.tipo + " "+ pa.dimension);
                }
                foreach (var pa in lstCampoBody)
                {
                    Debug.WriteLine(pa.nombre + " " + pa.tipo + " " + pa.dimension);
                }*/
                validator = true;
            }
            else
            {
                log.Info("Error al Inicializar Configuración MMI");
                log.Warn("No Existe: " + configFileName);
                validator = false;
            }

            return validator;
            

        }

        public String logIn(String traza)
        {
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;
            TrazaDat trazadat = new TrazaDat();

            /*Desencriptar traza*/
            trazadat = decryptTrace(traza);
            /*Crear request data*/
            TaquillaActivarRequest request = new TaquillaActivarRequest {
            carnetatencion= trazadat.ticket.carnetatencion.ToString(),
            codoficina = trazadat.ticket.codoficina,
            nroterminal = unchecked((int)trazadat.ticket.nroterminal)
            };
            /*Realizar la peticion*/
            RestClient client = getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPATCH);
            RestRequest requestEntity = executeRest(ApplicationConstants.taquillaLogInEndpoint, Method.PATCH, request);
            IRestResponse<TaquillaActivarResponse> response = client.Execute<TaquillaActivarResponse>(requestEntity);

            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
            }
            else
            {
                log.Error("Error en Metodo (logIn) codigo: " + response.Data.estatus.codigo);
                log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
            }

            return response.Data.estatus.codigo;
        }






        public TrazaDat decryptTrace(String traza)
        {
            var arrTransaccion = traza.Split('|');
            TrazaDat trazadat = new TrazaDat();
            for(int i =0; i < lstCampoHeader.Count; i++)
            {
                try { 
                     lstCampoHeader[i].value = arrTransaccion[i];
                }catch
                {
                    lstCampoHeader[i].value = null;
                }
            }
            
            
            /*Obtiene todo sobre el ticket*/
            TicketDat ticket = new TicketDat();
            ticket = getTicket();
            trazadat.ticket = ticket;

            /*Obtiene datos particulares*/
            try
            {
                trazadat.tipoOficina = int.Parse(lstCampoHeader.Where(x => x.nombre == "tipoEstacion").FirstOrDefault().value);
            }catch { }

            /*validate*/
            /*foreach (var pam in lstCampoHeader)
            {
                Debug.WriteLine(pam.nombre + " : " + pam.value);
            }*/
            /*Obtiene transacciones*/
            int topbody = lstCampoBody.Count();
            for (int i= lstCampoHeader.Count-1; i < arrTransaccion.Count(); i = i + topbody)
            {
                TransactionDat trx = new TransactionDat();
                trx = getTransaction(i, topbody, ticket.fechaatencionD.Value, arrTransaccion);
                trx.codoficina = ticket.codoficina;
                trx.fechaatencion = ticket.fechaatencion;
                trx.fechaatencionD = ticket.fechaatencionD;
                trx.horallegadaofic = ticket.horallegadaofic;
                trx.horallegadaoficD = ticket.horallegadaoficD;
                trx.idcedula = ticket.idcedula;
                trx.nrocedula = ticket.nrocedula;
                trx.nroticket = ticket.nroticket;
                trazadat.lstTrassaction.Add(trx);
            }



            return trazadat;
        }

        public TicketDat getTicket()
        {
            TicketDat tk = new TicketDat();
            try
            {
                tk.codoficina = long.Parse(lstCampoHeader.Where(x => x.nombre == "oficina").FirstOrDefault().value);
            }
            catch{ tk.exists = false;}
            try
            {
                tk.fechaatencion = lstCampoHeader.Where(x => x.nombre == "fecha").FirstOrDefault().value;
                tk.fechaatencionD = DateTime.ParseExact(tk.fechaatencion, ApplicationConstants.dfparm, null); 
            }
            catch { tk.exists = false; }
            try
            {
                tk.nroterminal = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroTerminal").FirstOrDefault().value);
            }
            catch { tk.exists = false; }
            try
            {
                var carnet = lstCampoHeader.Where(x => x.nombre == "nroFicha").FirstOrDefault().value;
                if (carnet.StartsWith("b") || carnet.StartsWith("c"))
                {
                    tk.carnetatencion = long.Parse(lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value.Substring(1));
                }else
                {
                    tk.carnetatencion = long.Parse(lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value);
                }
                tk.carnetactivacion = tk.carnetatencion;
            }
            catch { tk.exists = false; }
            try
            {
                tk.nroticket = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroFicha").FirstOrDefault().value);
            }
            catch { tk.nroticket = 0; }
            try
            {
                tk.idcedula = lstCampoHeader.Where(x => x.nombre == "nacionalidadCl").FirstOrDefault().value;
            }
            catch { tk.nroticket = 0; }
            try
            {
                tk.nrocedula = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroCIcl").FirstOrDefault().value);
            }
            catch { tk.exists = false; }
            try
            {
                var datehour = tk.fechaatencion + lstCampoHeader.Where(x => x.nombre == "hhiOperacion").FirstOrDefault().value;
                tk.horainiatencionD = DateTime.ParseExact(datehour, ApplicationConstants.dfform, null);
                tk.horainiatencion = tk.horainiatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            catch { tk.exists = false; }
            try
            {
                var datehour = tk.fechaatencion + lstCampoHeader.Where(x => x.nombre == "hhfOperacion").FirstOrDefault().value;
                tk.horafinatencionD = DateTime.ParseExact(datehour, ApplicationConstants.dfform, null);
                tk.horafinatencion = tk.horafinatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            catch { tk.exists = false; }
            try
            {
                tk.codtipocola = int.Parse(lstCampoHeader.Where(x => x.nombre == "tipoCola").FirstOrDefault().value);
            }
            catch { }

            var tipoCierre = lstCampoHeader.Where(x => x.nombre == "tipoCierre").FirstOrDefault().value;
            if(tipoCierre != null)
            {
                if(tipoCierre.Length > 0)
                {
                    tk.statustransaccion = tipoCierre;
                }
                else
                {
                    tk.exists = false;
                    tk.statustransaccion = "S";
                }
            }
            else
            {
                tk.statustransaccion = "S";
            }


            return tk;
        }

        public TransactionDat getTransaction(int ntransac,int  ncampos,DateTime fechaatencion,string[] arrtrx)
        {
            string sdate = "";
            TransactionDat trx = new TransactionDat();
            try
            {
                trx.codtrx = arrtrx[ntransac]; 
            }
            catch { trx.exists = false; }
            ntransac++;
            try
            {
                trx.montotrx = long.Parse(arrtrx[ntransac]);
            }
            catch { trx.exists = false; }
                ntransac++;
            try
            {   sdate = fechaatencion.ToString(ApplicationConstants.dfparm) + arrtrx[ntransac];
                trx.horafintrx = DateTime.ParseExact(sdate, ApplicationConstants.dfform, null);
            }
            catch { trx.exists = false; }
            return trx;
        }

        public RestClient getClientRest(string endpoint, int timeout)
        {
            RestClient client = new RestClient();
            client.Timeout = timeout;
            return client;
        }

        public RestRequest executeRest(string service, RestSharp.Method method, Object requestdat) 
        {
            RestRequest request = new RestRequest(service, method);
            request.AddJsonBody(requestdat);
            return request;
        }
    }
}
