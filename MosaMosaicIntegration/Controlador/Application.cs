﻿using log4net;
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
using System.Runtime.InteropServices;

namespace MosaMosaicIntegration.Controlador
{
    class Application
    {
        private static char sep = Path.DirectorySeparatorChar;
        private static string runDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public static readonly ILog log = LogManager.GetLogger(typeof(Application));
        public static List<Campo> lstCampoHeader =  new List<Campo>();
        public static List<Campo> lstCampoBody = new List<Campo>();
        


        public Application()
        {
            
        }

        public static Boolean configuracion()
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
                ApplicationConstants.codTranCallNext = param.Where(x => x.key.Equals("codTranCallNext")).FirstOrDefault().value;

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
                log.Info("Configuración inicializada.");
            }
            else
            {
                log.Error("Error al Inicializar Configuración MMI");
                log.Warn("No Existe: " + configFileName);
                validator = false;
            }

            return validator;
            

        }

        /*SEVICIOS*/
        public static TicketDat cosltTicket(TrazaDat traza)
        {
            TicketDat ticket = new TicketDat();
            if(traza.ticket.isValid)
            {
                /*Crear request data*/
                TicketConsultarRequest requestconsltticket = new TicketConsultarRequest
                {
                    codoficina=traza.ticket.codoficina,
                    fechaatencion = traza.ticket.fechaatencion,
                    idcedula = traza.ticket.idcedula,
                    nrocedula = traza.ticket.nrocedula,
                    codtipocola = traza.ticket.codtipocola,
                    indactivo = traza.ticket.indactivo,
                    indatencion = traza.ticket.indatencion,
                    nroticket = traza.ticket.nroticket
                };
                /*Realizar la peticion*/
                 RestClient client = Application.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutGET);
                 RestRequest requestEntity = Application.executeRest(ApplicationConstants.ticketCRUDEndpoint, Method.GET, requestconsltticket);
                 IRestResponse<TicketConsultarResponse> response = client.Execute<TicketConsultarResponse>(requestEntity);
                //JsonConvert.DeserializeObject()
                /*Tratamiento de respuesta*/
                Application.log.Info("REQUEST: " + JsonConvert.SerializeObject(requestEntity));
                if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
                {
                    /*Existe ticket*/
                    Application.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    ticket = response.Data.ticket;
                    ticket.isValid = true;
                    ticket.exists = true;
                    
                }
                else if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TICKET_NO_EXISTE))
                {
                    /*Ticket no existe*/
                    ticket = traza.ticket;
                    ticket.exists = false;
                }
            }
            else
            {
                /*ticket no valido - no existe*/
                ticket = traza.ticket;
                ticket.exists = false;
            }


            return ticket;
        }

        
        
        /*FUNCIONES*/
        public static TrazaDat decryptTrace(String traza)
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
                trazadat.tipoOficina = int.Parse(lstCampoHeader.Where(x => x.nombre == "tipoOficina").FirstOrDefault().value);
            }catch { }

            /*validate*/
            /*foreach (var pam in lstCampoHeader)
            {
                Debug.WriteLine(pam.nombre + " : " + pam.value);
            }*/
            /*Obtiene transacciones*/
            int topbody = lstCampoBody.Count();
            for (int i= lstCampoHeader.Count; i < arrTransaccion.Count(); i = i + topbody)
            {
                TransactionDat trx = new TransactionDat();
                trx = getTransaction(i, topbody, ticket.fechaatencionD.Value, arrTransaccion);
                if (trx.codtrx != null) { 
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
            }



            return trazadat;
        }
      
        private static TicketDat getTicket()
        {
            TicketDat tk = new TicketDat();
            try
            {
                tk.codoficina = long.Parse(lstCampoHeader.Where(x => x.nombre == "oficina").FirstOrDefault().value);
            }
            catch{ tk.isValid = false;}
            try
            {
                tk.fechaatencion = lstCampoHeader.Where(x => x.nombre == "fecha").FirstOrDefault().value;
              
                tk.fechaatencionD = DateTime.ParseExact(tk.fechaatencion, ApplicationConstants.dfparm, null);
                

            }
            catch { tk.isValid = false; 
            }
            try
            {
                var nom_taq = lstCampoHeader.Where(x => x.nombre == "nomredofic").FirstOrDefault().value;
                if (nom_taq != null && nom_taq.Count() > 0)
                {
                    tk.nom_red_ofic = nom_taq;
                }
                else
                {
                    tk.nom_red_ofic = System.Net.Dns.GetHostName();
                }
            }
            catch
            {
                tk.isValid = false;
                tk.nom_red_ofic = "";
            }


            try
            {
                tk.nroterminal = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroTerminal").FirstOrDefault().value);
            }
            catch { tk.isValid = false; }
            try
            {
                var carnet = lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value;
                if (carnet.StartsWith("b") || carnet.StartsWith("c"))
                {
                    tk.carnetatencion = long.Parse(lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value.Substring(1));
                }else
                {
                    tk.carnetatencion = long.Parse(lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value);
                }
                tk.carnetactivacion = tk.carnetatencion;
            }
            catch { tk.isValid = false; }
            try
            {
                tk.nroticket = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroFicha").FirstOrDefault().value);
            }
            catch { tk.nroticket = 0; }
            try
            {
                tk.idcedula = lstCampoHeader.Where(x => x.nombre == "nacionalidadCl").FirstOrDefault().value;
            }
            catch { tk.idcedula = "N"; }
            try
            {
                tk.nrocedula = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroCIcl").FirstOrDefault().value);
            }
            catch { tk.isValid = false; }
            try
            {
                var datehour = tk.fechaatencion + lstCampoHeader.Where(x => x.nombre == "hhiOperacion").FirstOrDefault().value;
                tk.horainiatencionD = DateTime.ParseExact(datehour, ApplicationConstants.dfform, null);
                tk.horainiatencion = tk.horainiatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            catch { tk.isValid = false; }
            try
            {
                var datehour = tk.fechaatencion + lstCampoHeader.Where(x => x.nombre == "hhfOperacion").FirstOrDefault().value;
                tk.horafinatencionD = DateTime.ParseExact(datehour, ApplicationConstants.dfform, null);
                tk.horafinatencion = tk.horafinatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            catch { tk.isValid = false; }
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
                    tk.isValid = false;
                    tk.statustransaccion = "S";
                }
            }
            else
            {
                tk.statustransaccion = "S";
            }
            tk.exists = false;

            return tk;
        }

        private static TransactionDat getTransaction(int ntransac,int  ncampos,DateTime fechaatencion,string[] arrtrx)
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
            catch {  }
                ntransac++;
            try
            {   sdate = fechaatencion.ToString(ApplicationConstants.dfparm) + arrtrx[ntransac];
                trx.horafintrx = DateTime.ParseExact(sdate, ApplicationConstants.dfform, null);
            }
            catch { trx.exists = false; }
            return trx;
        }

        public static RestClient getClientRest(string endpoint, int timeout)
        {
            RestClient client = new RestClient(endpoint);
            client.Timeout = timeout;
            return client;
        }

        public static RestRequest executeRest(string service, RestSharp.Method method, Object requestdat) 
        {
            RestRequest request = new RestRequest(service, method);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            if (method.Equals(Method.GET))
            {
                request.AddObject(requestdat);
            }else
            {
               
                request.AddJsonBody(requestdat);
            }
            
            return request;
        }


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
      int dwDesiredAccess,
      bool bInheritHandle,
      int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(
          int hProcess,
          int lpBaseAddress,
          byte[] lpBuffer,
          int dwSize,
          ref int lpNumberOfBytesRead);
    }
}
