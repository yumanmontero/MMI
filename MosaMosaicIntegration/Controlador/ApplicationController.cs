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
using System.Runtime.InteropServices;

namespace MosaMosaicIntegration.Controlador
{
    public class ApplicationController
    {
        public static char sep = Path.DirectorySeparatorChar;
        public static string runDirectory = "";
        public static readonly ILog log = LogManager.GetLogger(typeof(ApplicationController));
        public static List<Campo> lstCampoHeader =  new List<Campo>();
        public static List<Campo> lstCampoBody = new List<Campo>();
        
        

        public static Boolean configuracion()
        {
            runDirectory = Directory.GetCurrentDirectory();            
            Console.WriteLine("The current directory is {0}", runDirectory);
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
                ApplicationConstants.pathFailed = param.Where(x => x.key.Equals("pathFailed")).FirstOrDefault().value;
                if(ApplicationConstants.pathFailed != null && ApplicationConstants.pathFailed != "") { } else
                {
                    ApplicationConstants.pathFailed = runDirectory + sep + "Mosa_To_Batch";
                }

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

        /*PUBLICADO*/
        public static String logIn(TrazaDat trazadat)
        {
            ApplicationController.log.Info("Inicia Operación de LogIn");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;

            /*Crear request data*/
            TaquillaActivarRequest request = new TaquillaActivarRequest
            {
                carnetatencion = ApplicationController.lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value,
                codoficina = trazadat.ticket.codoficina,
                nom_red_ofic = trazadat.ticket.nom_red_ofic
            };
            /*Realizar la peticion*/
            RestClient client = ApplicationController.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPATCH);
            RestRequest requestEntity = ApplicationController.executeRest(ApplicationConstants.taquillaLogInEndpoint, Method.PATCH, request);
            IRestResponse<TaquillaActivarResponse> response = client.Execute<TaquillaActivarResponse>(requestEntity);
            //JsonConvert.DeserializeObject()
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                codigo = response.Data.estatus.codigo;
            }
            else
            {
                ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                if (!response.IsSuccessful)
                {
                    codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    ApplicationController.log.Info("RESPONSE: " + response.ErrorMessage);
                }
                else
                {
                    ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    ApplicationController.log.Error("Error en Metodo (logIn) codigo: " + response.Data.estatus.codigo);
                    codigo = response.Data.estatus.codigo;
                }
            }
            ApplicationController.log.Info("Finaliza Operación de LogIn");
            return codigo;
        }

        public static String logOut(TrazaDat trazadat)
        {
            ApplicationController.log.Info("Inicia Operación de LogOut");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;

            /*Crear request data*/
            TaquillaDesactivarRequest request = new TaquillaDesactivarRequest
            {
                carnetatencion = ApplicationController.lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value,
                codoficina = trazadat.ticket.codoficina,
                nom_red_ofic = trazadat.ticket.nom_red_ofic
            };
            /*Realizar la peticion*/
            RestClient client = ApplicationController.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPATCH);
            RestRequest requestEntity = ApplicationController.executeRest(ApplicationConstants.taquillaLogOutEndpoint, Method.PATCH, request);
            IRestResponse<TaquillaDesactivarResponse> response = client.Execute<TaquillaDesactivarResponse>(requestEntity);
            //JsonConvert.DeserializeObject()
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                codigo = response.Data.estatus.codigo;
            }
            else
            {
                ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                if (!response.IsSuccessful)
                {
                    codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    ApplicationController.log.Info("RESPONSE: " + response.Content.ToString());
                }
                else
                {
                    ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    ApplicationController.log.Error("Error en Metodo (logIn) codigo: " + response.Data.estatus.codigo);
                    codigo = response.Data.estatus.codigo;
                }
            }
            ApplicationController.log.Info("Finaliza Operación de LogOut");
            return codigo;
        }

        public static String registerTrasact(TrazaDat trazadat)
        {
            ApplicationController.log.Info("Inicia Operación de Registro de Transaccion");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;
            Boolean registerLater = false;
            EstatusDat estatusDat = new EstatusDat();
            estatusDat.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
            TicketDat ticketOri = new TicketDat();
            ApplicationController.log.Info("Ticket Antes: " + JsonConvert.SerializeObject(trazadat.ticket));
            /*Validar si el ticket existe*/
            /*Si no viene un nro de ticket en la rafaga significa que estamos en presencia o de un ticket registrado por oficina tipo 2 o de un ticket proveniente de una oficina tipo 1*/
            if (trazadat.ticket.nroticket == 0)
            {
                trazadat.ticket.indactivo = ApplicationConstants.TICKET_NO_ASIGNADO_INDIC_ACTIVA;
                ticketOri = cosltTicket(trazadat, true); /*Buscar ticket de oficina tipo 2*/
                if (ticketOri.isValid)
                {
                    if (ticketOri.exists)
                    {/*Actualizar*/
                        trazadat.ticket.indatencion = trazadat.ticket.statustransaccion;
                        if (ticketOri.indactivo.Equals("K"))
                        {
                            trazadat.ticket.indactivo = "P";
                        }
                        else if (ticketOri.indactivo.Equals("B"))
                        {
                            trazadat.ticket.indactivo = "B";
                        }
                        else
                        {
                            trazadat.ticket.indactivo = "C";
                        }
                        estatusDat = updateTicket(ticketOri, trazadat.ticket);
                    }
                    else
                    {/*Busca si el ticket oficina tipo 1 ya no fue procesado*/
                        trazadat.ticket.indactivo = null;
                        ticketOri = cosltTicket(trazadat, false);
                        if (ticketOri.isValid)
                        {
                            if (!ticketOri.exists) /*Esto implica que el ticket es de oficina tipo 1 y no ha sido registrado*/
                            {/*Crea*/
                                trazadat.ticket.nroticket = trazadat.ticket.nroticketcal;
                                trazadat.ticket.indactivo ="F";
                                trazadat.ticket.indatencion = trazadat.ticket.statustransaccion;
                                estatusDat = addTicket(trazadat.ticket);
                            }
                        }
                    }

                }
            }else
            {
                ticketOri = cosltTicket(trazadat, true);
                /*Agrega o Actualiza el ticket*/
                if (ticketOri.isValid)
                {
                    if (ticketOri.exists)
                    {/*Actualizar*/
                        trazadat.ticket.indatencion = trazadat.ticket.statustransaccion;
                        if (ticketOri.indactivo.Equals("K"))
                        {
                            trazadat.ticket.indactivo = "P";
                        }
                        else if (ticketOri.indactivo.Equals("B"))
                        {
                            trazadat.ticket.indactivo = "B";
                        }
                        else
                        {
                            trazadat.ticket.indactivo = "C";
                        }
                        estatusDat = updateTicket(ticketOri, trazadat.ticket);
                    }
                    else
                    {/*Crea*/
                        trazadat.ticket.indactivo = "F";
                        trazadat.ticket.indatencion = trazadat.ticket.statustransaccion;
                        estatusDat = addTicket(trazadat.ticket);
                    }
                }
            }
            
         


            ApplicationController.log.Info("Ticket D: "+ JsonConvert.SerializeObject(ticketOri));
            
            /*Ticket valido, Request anterior exitoso y transacciones a registrar*/
            if (ticketOri.isValid && estatusDat.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA) && trazadat.lstTrassaction.Count() > 0)
            {
                estatusDat = addTransact(trazadat.lstTrassaction);
            }
            else
            {
                if(!ticketOri.isValid)
                {
                    estatusDat.codigo = ApplicationConstants.REGISTRO_INVALIDO;
                    registerLater = true;
                }
                else if (!estatusDat.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
                {
                    registerLater = true;
                }
                
            }

            if(registerLater)
            {
                registerLaterTrace(trazadat.traza);
            }
            codigo = estatusDat.codigo;
            ApplicationController.log.Info("Finaliza Operación de Registro de Transaccion");
            return codigo;
        }


        /*SEVICIOS*/
        public static TicketDat cosltTicket(TrazaDat traza, Boolean alldayP)
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
                    nroticket = traza.ticket.nroticket,
                    allday = alldayP
                };
                /*Realizar la peticion*/
                 RestClient client = ApplicationController.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutGET);
                 RestRequest requestEntity = ApplicationController.executeRest(ApplicationConstants.ticketCRUDEndpoint, Method.GET, requestconsltticket);
                 IRestResponse<TicketConsultarResponse> response = client.Execute<TicketConsultarResponse>(requestEntity);
                //JsonConvert.DeserializeObject()
                /*Tratamiento de respuesta*/
                ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(requestEntity));
                if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
                {
                    /*Existe ticket*/
                    ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    ticket = response.Data.ticket;
                    ticket.isValid = true;
                    ticket.exists = true;
                    
                }
                else if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TICKET_NO_EXISTE))
                {
                    /*Ticket no existe*/
                    ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    ticket = traza.ticket;
                    ticket.exists = false;
                }else if (!response.IsSuccessful)
                {
                    ticket = traza.ticket;
                    ticket.isValid = false;
                    ticket.exists = false;
                    ApplicationController.log.Info("RESPONSE: " + response.Content.ToString());
                }
                else
                {
                    ticket = traza.ticket;
                    ticket.isValid = false;
                    ticket.exists = false;
                    ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
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
        
        public static EstatusDat updateTicket(TicketDat ticketOri, TicketDat ticketModf)
        {
            EstatusDat estatusd = new EstatusDat();
       
            /*Crear request data*/
            TicketModificarRequest request = new TicketModificarRequest
            {
                ticket_actual = ticketOri,
                ticket_modif = ticketModf
            };
            /*Realizar la peticion*/
            RestClient client = ApplicationController.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPATCH);
            RestRequest requestEntity = ApplicationController.executeRest(ApplicationConstants.ticketCRUDEndpoint, Method.PATCH, request);
            IRestResponse<TicketModificarResponse> response = client.Execute<TicketModificarResponse>(requestEntity);
            /*Deserializar respuesta*/
            ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                estatusd = response.Data.estatus;
            }
            else
            {
                if (!response.IsSuccessful)
                {
                    ApplicationController.log.Info("RESPONSE: " + response.Content.ToString());
                    estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                }
                else
                {
                    ApplicationController.log.Error("Error en Metodo (updateTicket) codigo: " + response.Data.estatus.codigo);
                    estatusd = response.Data.estatus;
                }

            }

                return estatusd;
        }

        public static EstatusDat addTicket(TicketDat ticketNuevo)
        {
            EstatusDat estatusd = new EstatusDat();

            /*Crear request data*/
            TicketAgregarRequest request = new TicketAgregarRequest
            {
                ticket = ticketNuevo
            };
            /*Realizar la peticion*/
            RestClient client = ApplicationController.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPUT);
            RestRequest requestEntity = ApplicationController.executeRest(ApplicationConstants.ticketCRUDEndpoint, Method.PUT, request);
            IRestResponse<TicketAgregarResponse> response = client.Execute<TicketAgregarResponse>(requestEntity);
            /*Deserializar respuesta*/
            ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                estatusd = response.Data.estatus;
            }
            else
            {
                if (!response.IsSuccessful)
                {
                    ApplicationController.log.Info("RESPONSE: " + response.Content.ToString());
                    estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                }
                else
                {
                    ApplicationController.log.Error("Error en Metodo (addTicket) codigo: " + response.Data.estatus.codigo);
                    ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    estatusd = response.Data.estatus;
                }

            }

            return estatusd;
        }

        public static EstatusDat addTransact(List<TransactionDat> lstTransaction)
        {
            EstatusDat estatusd = new EstatusDat();

            /*Crear request data*/
            TransaccionAgregarRequest request = new TransaccionAgregarRequest
            {
                transacciones = lstTransaction
            };
            /*Realizar la peticion*/
            RestClient client = ApplicationController.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPUT);
            RestRequest requestEntity = ApplicationController.executeRest(ApplicationConstants.transaccionCRUDEndpoint, Method.PUT, request);
            IRestResponse<TransaccionAgregarResponse> response = client.Execute<TransaccionAgregarResponse>(requestEntity);
            /*Deserializar respuesta*/
           ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                estatusd = response.Data.estatus;
            }
            else
            {
                if (!response.IsSuccessful)
                {
                    ApplicationController.log.Info("RESPONSE: " + response.Content.ToString());
                    estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                }
                else
                {
                    ApplicationController.log.Error("Error en Metodo (addTransact) codigo: " + response.Data.estatus.codigo);
                    estatusd = response.Data.estatus;
                }

            }

            return estatusd;
        }
        
        /*FUNCIONES*/
        public static TrazaDat decryptTrace(String traza)
        {
            TrazaDat trazadat = new TrazaDat();
            trazadat.traza = traza;
            var arrTransaccion = traza.Split('|');
            
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
            if (arrTransaccion.Count() > lstCampoHeader.Count())
            {
                for (int i = lstCampoHeader.Count; i < arrTransaccion.Count(); i = i + topbody)
                {
                    TransactionDat trx = new TransactionDat();
                    trx = getTransaction(i, topbody, ticket.fechaatencionD.Value, arrTransaccion);
                    if (trx.codtrx != null)
                    {
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
            }
            else
            {
                trazadat.wasCompleted = false;
            }


            return trazadat;
        }
      
        private static TicketDat getTicket()
        {
            TicketDat tk = new TicketDat();
            tk.isValid = true;
            try
            {
                tk.codoficina = long.Parse(lstCampoHeader.Where(x => x.nombre == "oficina").FirstOrDefault().value);
            }
            catch{ tk.isValid = false;}
            //log.Info("codoficina: " + tk.isValid);
            try
            {
                tk.fechaatencion = lstCampoHeader.Where(x => x.nombre == "fecha").FirstOrDefault().value;
              
                tk.fechaatencionD = DateTime.ParseExact(tk.fechaatencion, ApplicationConstants.dfparm, null);
                

            }
            catch { tk.isValid = false; 
            }
            //log.Info("fechaatencion: " + tk.isValid);
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
            //log.Info("nom_taq: " + tk.isValid);

            try
            {
                tk.nroterminal = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroTerminal").FirstOrDefault().value);
            }
            catch { tk.isValid = false; }
            //log.Info("nroterminal: " + tk.isValid);
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
            //log.Info("carnetatencion: " + tk.isValid);
            try
            {
                tk.nroticket = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroFicha").FirstOrDefault().value);
            }
            catch { tk.nroticket = 0; }
            //log.Info("nroticket: " + tk.isValid);
            try
            {
                tk.idcedula = lstCampoHeader.Where(x => x.nombre == "nacionalidadCl").FirstOrDefault().value;
            }
            catch { tk.idcedula = "N"; }
            //log.Info("idcedula: " + tk.isValid);
            try
            {
                tk.nrocedula = int.Parse(lstCampoHeader.Where(x => x.nombre == "nroCIcl").FirstOrDefault().value);
            }
            catch { tk.isValid = false; }
            //log.Info("nrocedula: " + tk.isValid);
            try
            {
                var datehour = tk.fechaatencion + lstCampoHeader.Where(x => x.nombre == "hhiOperacion").FirstOrDefault().value;
                tk.horainiatencionD = DateTime.ParseExact(datehour, ApplicationConstants.dfform, null);
                tk.horainiatencion = tk.horainiatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            catch { tk.isValid = false; }
            //log.Info("horainiatencion: " + tk.isValid);
            try
            {
                var datehour = tk.fechaatencion + lstCampoHeader.Where(x => x.nombre == "hhfOperacion").FirstOrDefault().value;
                tk.horafinatencionD = DateTime.ParseExact(datehour, ApplicationConstants.dfform, null);
                tk.horafinatencion = tk.horafinatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            catch { tk.isValid = false; }
            //log.Info("horafinatencion: " + tk.isValid);
            try
            {
                tk.codtipocola = int.Parse(lstCampoHeader.Where(x => x.nombre == "tipoCola").FirstOrDefault().value);
            }
            catch { tk.isValid = false; }
            //log.Info("codtipocola: " + tk.isValid);
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

            if(tk.fechaatencionD != null) { 
                 tk.fechaatencion = tk.fechaatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            if (tk.horainiatencionD != null)
            {
                tk.horainiatencion = tk.horainiatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
                tk.horallegadaofic = tk.horainiatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
                tk.horallegadaoficD = tk.horainiatencionD.Value;
            }
            if (tk.horafinatencionD != null)
            {
                tk.horafinatencion = tk.horafinatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
           
            tk.exists = false;

            if(tk.isValid && tk.nroticket == 0)
            {
                Random random = new Random();
                int randomNumber = random.Next(10000, 30000);
                tk.nroticketcal = (tk.codtipocola*100000)+ randomNumber;
            }

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

        public static void registerLaterTrace(String traza)
        {
            string path = @"" + ApplicationConstants.pathFailed;
            string prefix = "MOSA-";
            string dater = DateTime.Now.ToString(ApplicationConstants.dfparm);
            string ext = ".txt";
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);

                }catch
                {}
            }
            if(!File.Exists(path+sep+ prefix + dater + ext))
            {
                using (StreamWriter sw = File.CreateText(path + sep + prefix + dater + ext))
                {
                    sw.WriteLine(traza);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path + sep + prefix + dater + ext))
                {
                    sw.WriteLine(traza);
                }
            }


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


    }
}
