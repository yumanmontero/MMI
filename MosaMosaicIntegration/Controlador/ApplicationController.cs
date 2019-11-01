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
            lstCampoHeader = new List<Campo>();
            lstCampoBody = new List<Campo>();
            lstCampoHeader.Clear();
            lstCampoBody.Clear();
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
                if (ApplicationConstants.pathFailed != null && ApplicationConstants.pathFailed != "") { } else
                {
                    ApplicationConstants.pathFailed = runDirectory + sep + "Mosa_To_Batch";
                }

                /*TIMEOUTS*/
                ApplicationConstants.timeoutGET = int.Parse(param.Where(x => x.key.Equals("timeoutGET")).FirstOrDefault().value);
                ApplicationConstants.timeoutPOST = int.Parse(param.Where(x => x.key.Equals("timeoutPOST")).FirstOrDefault().value);
                ApplicationConstants.timeoutPATCH = int.Parse(param.Where(x => x.key.Equals("timeoutPATCH")).FirstOrDefault().value);
                ApplicationConstants.timeoutPUT = int.Parse(param.Where(x => x.key.Equals("timeoutPUT")).FirstOrDefault().value);
                ApplicationConstants.timeoutDELETE = int.Parse(param.Where(x => x.key.Equals("timeoutDELETE")).FirstOrDefault().value);
                ApplicationConstants.tcoladeafult = int.Parse(param.Where(x => x.key.Equals("tcolaoficdefault")).FirstOrDefault().value);
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

            
            try
            {
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
                    if (Convert.ToInt16((int)response.StatusCode).Equals(200))
                    {
                    if (ApplicationConstants.moddebug)
                    {
                        ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                        ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    }
                        codigo = response.Data.estatus.codigo;
                        /*Si no existe la taquilla la registra*/
                        if (codigo.Equals(ApplicationConstants.TAQUILLA_NO_EXISTE))
                        {
                            codigo = addTaquilla(trazadat).codigo;
                        }
                    }
                    else
                    {
                        
                        codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    if (ApplicationConstants.moddebug) {
                        ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                        ApplicationController.log.Info("RESPONSE: " + response.ErrorMessage); }
                    }
               }catch(Exception ex)
            {
                codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                ApplicationController.log.Info("Error [Exception] (login): " + ex);
            }
            ApplicationController.log.Info("Finaliza Operación de LogIn");
            return codigo;
        }

        public static String logOut(TrazaDat trazadat)
        {
            ApplicationController.log.Info("Inicia Operación de LogOut");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;

            try
            { 

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
            if (Convert.ToInt16((int)response.StatusCode).Equals(200) && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                    if (ApplicationConstants.moddebug)
                    {
                        ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                        ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    }
                codigo = response.Data.estatus.codigo;
            }
            else
            {
                    if (ApplicationConstants.moddebug)
                    { ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request)); }
                if (!Convert.ToInt16((int)response.StatusCode).Equals(200))
                {
                    codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + response.Content.ToString()); }
                }
                else
                {
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                    ApplicationController.log.Error("Error en Metodo (logIn) codigo: " + response.Data.estatus.codigo);
                    codigo = response.Data.estatus.codigo;
                }
            }
            }
            catch (Exception ex)
            {
                codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                ApplicationController.log.Info("Error [Exception] (logout): " + ex);
            }
            ApplicationController.log.Info("Finaliza Operación de LogOut");
            return codigo;
        }

        public static String registerTrasact(TrazaDat trazadat)
        {
            ApplicationController.log.Info("Inicia Operación de Registro de Transaccion");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;
            Boolean registerLater = false;
            Boolean redyTicket = false;
            Boolean redyTransact = false;
            EstatusDat estatusDat = new EstatusDat();
            estatusDat.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
            TicketDat ticketOri = new TicketDat();
            TicketModificarResponse typemodf = new TicketModificarResponse();
            TicketAgregarResponse typeadd = new TicketAgregarResponse();
    
            //ApplicationController.log.Info("Ticket Antes: " + JsonConvert.SerializeObject(trazadat.ticket));
            /*Validar si el ticket existe*/
            /*Si no viene un nro de ticket en la rafaga significa que estamos en presencia o de un ticket registrado por oficina tipo 2 o de un ticket proveniente de una oficina tipo 1*/
            if (trazadat.ticket.nroticket == 0)
            {
                trazadat.ticket.indactivo = ApplicationConstants.TICKET_NO_ASIGNADO_INDIC_ACTIVA;
                int tcolaactual = trazadat.ticket.codtipocola.Value;
                trazadat.ticket.codtipocola = ApplicationConstants.tcoladeafult;
                ticketOri = cosltTicket(trazadat, true); /*Buscar ticket de oficina tipo 2*/
                if (ticketOri.isValid)
                {
                    trazadat.ticket.codtipocola = tcolaactual;
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
                        ticketOri = normalizeTimeTicket(ticketOri);
                        if(ticketOri.horallegadaofic != null)
                        {
                            trazadat.ticket.horallegadaofic = null;
                            trazadat.ticket.horallegadaoficD = null;
                        }

                        typemodf = updateTicket(ticketOri, trazadat.ticket);
                        estatusDat = typemodf.estatus;
                        redyTicket = true;
                    }
                    else
                    {/*Busca si el ticket oficina tipo 1 ya no fue procesado*/
                        trazadat.ticket.indactivo = null;
                        String dateaten = trazadat.ticket.fechaatencion;
                        trazadat.ticket.fechaatencion = trazadat.ticket.horallegadaofic;
                        ticketOri = cosltTicket(trazadat, false);
                        if (ticketOri.isValid)
                        {
                            trazadat.ticket.fechaatencion = dateaten;
                            if (!ticketOri.exists) /*Esto implica que el ticket es de oficina tipo 1 y no ha sido registrado*/
                            {/*Crea*/
                                trazadat.ticket.nroticket = trazadat.ticket.nroticketcal;
                                trazadat.ticket.indactivo ="F";
                                trazadat.ticket.indatencion = trazadat.ticket.statustransaccion;
                                typeadd = addTicket(trazadat.ticket);
                                estatusDat = typeadd.estatus;
                                ticketOri = typeadd.ticket;
                                ticketOri = normalizeTimeTicket(ticketOri);
                                ticketOri.isValid = true;
                                redyTicket = true;
                            }
                            else
                            {
                                estatusDat.codigo = ApplicationConstants.TRANSACCION_EXITOSA;
                                redyTicket = true;
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
                        ticketOri = normalizeTimeTicket(ticketOri);
                        trazadat.ticket.horallegadaofic = null;
                        trazadat.ticket.horallegadaoficD = null;
                        typemodf = updateTicket(ticketOri, trazadat.ticket);
                        estatusDat = typemodf.estatus;
                        redyTicket = true;
                        foreach(String text in estatusDat.mensajeProgramador){
                            if (text.ToUpper().Contains("CLIENTE YA FUE ATENDIDO"))
                            {
                                if (ApplicationConstants.moddebug)
                                {
                                    ApplicationController.log.Info(text);
                                }
                                estatusDat.codigo = ApplicationConstants.TRANSACCION_EXITOSA;
                            }
                        }

                    }
                    else
                    {
                        /*Preguntar si nos encontramos en presencia de un ticket activado por mosaweb*/
                        trazadat.ticket.indactivo = ApplicationConstants.TICKET_NO_ASIGNADO_INDIC_ACTIVA;
                        int tcolaactual = trazadat.ticket.codtipocola.Value;
                        int nrocedula = trazadat.ticket.nrocedula.Value;
                        String idcedula = trazadat.ticket.idcedula;
                        trazadat.ticket.codtipocola = null;
                        trazadat.ticket.nrocedula = 0;
                        trazadat.ticket.idcedula = "*";
                        ticketOri = cosltTicket(trazadat, true);
                        if (ticketOri.isValid)
                        {
                            trazadat.ticket.codtipocola = tcolaactual;
                            trazadat.ticket.nrocedula = nrocedula;
                            trazadat.ticket.idcedula = idcedula;
                            /*Actualiza*/
                            if (ticketOri.exists)
                            {
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
                                ticketOri = normalizeTimeTicket(ticketOri);
                                trazadat.ticket.horallegadaofic = null;
                                trazadat.ticket.horallegadaoficD = null;
                                typemodf = updateTicket(ticketOri, trazadat.ticket);
                                estatusDat = typemodf.estatus;
                                redyTicket = true;
                                foreach (String text in estatusDat.mensajeProgramador)
                                {
                                    if (text.ToUpper().Contains("CLIENTE YA FUE ATENDIDO"))
                                    {
                                        if (ApplicationConstants.moddebug)
                                        {
                                            ApplicationController.log.Info(text);
                                        }
                                        estatusDat.codigo = ApplicationConstants.TRANSACCION_EXITOSA;
                                    }
                                }
                            }/*Crea*/
                            else
                            {
                                trazadat.ticket.indactivo = "F";
                                trazadat.ticket.indatencion = trazadat.ticket.statustransaccion;
                                typeadd = addTicket(trazadat.ticket);
                                ticketOri = typeadd.ticket;
                                ticketOri = normalizeTimeTicket(ticketOri);
                                estatusDat = typeadd.estatus;
                                ticketOri.isValid = true;
                                redyTicket = true;
                            }
                        }

                     
                        
                    }
                }
            }




            // ApplicationController.log.Info("Ticket D: "+ JsonConvert.SerializeObject(ticketOri));
            if (ApplicationConstants.moddebug)
            {
                ApplicationController.log.Info("TicketValid: "+ ticketOri.isValid.ToString() + " Codigo: "+ estatusDat.codigo+ " CantTransacciones: "+ trazadat.lstTrassaction.Count());
                foreach (TransactionDat trx in trazadat.lstTrassaction)
                {
                    ApplicationController.log.Info("TrxID: "+trx.codtrx+ " TrxMonto: "+trx.montotrx+ " Fechafin "+trx.horafintrxD);
                }
                
            }


            /*Ticket valido, Request anterior exitoso y transacciones a registrar*/
            if (ticketOri.isValid && estatusDat.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA) && trazadat.lstTrassaction.Count() > 0)
            {
                foreach(TransactionDat trx in trazadat.lstTrassaction)
                {
                    trx.nroticket = ticketOri.nroticket;
                }
                estatusDat = addTransact(trazadat.lstTrassaction);
                redyTransact = true;
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

            if (ApplicationConstants.moddebug)
            {
                ApplicationController.log.Info("Ticket: " + redyTicket.ToString() + " Transact " + redyTransact.ToString());
            }

            ApplicationController.log.Info("Finaliza Operación de Registro de Transaccion");
            return codigo;
        }


        /*SEVICIOS*/
        public static TicketDat cosltTicket(TrazaDat traza, Boolean alldayP)
        {
            TicketDat ticket = new TicketDat();
            if(traza.ticket.isValid)
            {
                try {
                
                /*Crear request data*/
                TicketConsultarRequest requestconsltticket = new TicketConsultarRequest
                {
                    codoficina=traza.ticket.codoficina,
                    fechaatencion = traza.ticket.fechaatencion,
                    idcedula = traza.ticket.idcedula,
                    nrocedula = traza.ticket.nrocedula,
                    codtipocola = traza.ticket.codtipocola.Value,
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
                    if (ApplicationConstants.moddebug)
                    {
                        ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(requestconsltticket));
                    }
                //TicketConsultarResponse deserializedProduct = JsonConvert.DeserializeObject<TicketConsultarResponse>(response.Content);


               if (Convert.ToInt16((int)response.StatusCode).Equals(200) && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
                {
                        /*Existe ticket*/
                        if (ApplicationConstants.moddebug)
                        {ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));}
                    ticket = response.Data.ticket;
                    ticket.isValid = true;
                    ticket.exists = true;
                    
                }
                else if (Convert.ToInt16((int)response.StatusCode).Equals(200) && response.Data.estatus.codigo.Equals(ApplicationConstants.TICKET_NO_EXISTE))
                {
                        /*Ticket no existe*/
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                    ticket = traza.ticket;
                    ticket.exists = false;
                }else if (!Convert.ToInt16((int)response.StatusCode).Equals(200))
                {
                    ticket = traza.ticket;
                    ticket.isValid = false;
                    ticket.exists = false;
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + response.Content.ToString()); }
                }
                else
                {
                    ticket = traza.ticket;
                    ticket.isValid = false;
                    ticket.exists = false;
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                }
                }
                catch (Exception ex)
                {
                    ticket = traza.ticket;
                    ticket.isValid = false;
                    ticket.exists = false;
                    ApplicationController.log.Error("Error [Exception] (cosltTicket): " + ex);
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
        
        public static TicketModificarResponse updateTicket(TicketDat ticketOri, TicketDat ticketModf)
        {
            EstatusDat estatusd = new EstatusDat();
            TicketModificarResponse responsedat = new TicketModificarResponse();
            try
            {
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
                if (ApplicationConstants.moddebug)
                { ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request)); }
                if (Convert.ToInt16((int)response.StatusCode).Equals(200) && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
                {
                    if (ApplicationConstants.moddebug)
                    { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                    estatusd = response.Data.estatus;
                    responsedat.ticket_actual = response.Data.ticket_actual;
                    responsedat.ticket_modif = response.Data.ticket_modif;
                }
                else
                {
                    if (!Convert.ToInt16((int)response.StatusCode).Equals(200))
                    {
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + response.Content.ToString()); }
                        estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    }
                    else
                    {
                        ApplicationController.log.Error("Error en Metodo (updateTicket) codigo: " + response.Data.estatus.codigo);
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                        estatusd = response.Data.estatus;
                    }

                }
            } catch (Exception ex)
            {
                estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                ApplicationController.log.Info("Error [Exception] (updateTicket): " + ex);
            }


            responsedat.estatus = estatusd;

           return responsedat;
        }

        public static TicketAgregarResponse addTicket(TicketDat ticketNuevo)
        {
            EstatusDat estatusd = new EstatusDat();
            TicketAgregarResponse responset = new TicketAgregarResponse();
            try { 
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
                if (ApplicationConstants.moddebug)
                { ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request)); }

            if (Convert.ToInt16((int)response.StatusCode).Equals(200) && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                    if (ApplicationConstants.moddebug)
                    { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                estatusd = response.Data.estatus;
                responset.ticket = response.Data.ticket;
            }
            else
            {
                if (!Convert.ToInt16((int)response.StatusCode).Equals(200))
                {
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + response.Content.ToString()); }
                    estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                }
                else
                {
                    ApplicationController.log.Error("Error en Metodo (addTicket) codigo: " + response.Data.estatus.codigo);
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                    estatusd = response.Data.estatus;
                }

            }
            }catch(Exception ex)
            {
                estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                ApplicationController.log.Info("Error [Exception] (addTicket): " + ex);
            }
            responset.estatus = estatusd;
            return responset;
        }

        public static EstatusDat addTransact(List<TransactionDat> lstTransaction)
        {
            EstatusDat estatusd = new EstatusDat();

            try
            {
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
                if (ApplicationConstants.moddebug)
                { ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request)); }
                if (Convert.ToInt16((int)response.StatusCode).Equals(200) && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
                {
                    if (ApplicationConstants.moddebug)
                    { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                    estatusd = response.Data.estatus;
                }
                else
                {
                    if (!Convert.ToInt16((int)response.StatusCode).Equals(200))
                    {
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + response.Content.ToString()); }
                        estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    }
                    else
                    {
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                        ApplicationController.log.Error("Error en Metodo (addTransact) codigo: " + response.Data.estatus.codigo);
                        estatusd = response.Data.estatus;
                    }

                }
            }catch(Exception ex)
            {
                estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                ApplicationController.log.Error("Error [Exception] (addTransact): " + ex);
            }

            return estatusd;
        }
        
        public static EstatusDat addTaquilla(TrazaDat trazadat)
        {
            ApplicationController.log.Info("Inicia Operación de Registro de Taquilla");
            EstatusDat estatusd = new EstatusDat();
            int nrotaqdat = 0;
            try
            {
                /*Crear request data*/
                try
                {
                    nrotaqdat = Convert.ToInt16(trazadat.ticket.nom_red_ofic.LastOrDefault());
                }
                catch
                {
                    nrotaqdat = 0;
                }
                TaquillaAgregarRequest request = new TaquillaAgregarRequest
                {
                    carnetatencion = ApplicationController.lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value,
                    codoficina = trazadat.ticket.codoficina,
                    nom_red_ofic = trazadat.ticket.nom_red_ofic,
                    nrotaq = nrotaqdat,
                    estado = ApplicationConstants.ACTIVO

                };
                /*Realizar la peticion*/
                RestClient client = ApplicationController.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPUT);
                RestRequest requestEntity = ApplicationController.executeRest(ApplicationConstants.taquillaCRUDEndpoint, Method.PUT, request);
                IRestResponse<TaquillaAgregarResponse> response = client.Execute<TaquillaAgregarResponse>(requestEntity);
                /*Deserializar respuesta*/
                if (ApplicationConstants.moddebug)
                { ApplicationController.log.Info("REQUEST: " + JsonConvert.SerializeObject(request)); }
                if (Convert.ToInt16((int)response.StatusCode).Equals(200) && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
                {
                    if (ApplicationConstants.moddebug)
                    { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                    estatusd = response.Data.estatus;
                }
                else
                {
                    if (!Convert.ToInt16((int)response.StatusCode).Equals(200))
                    {
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + response.Content.ToString()); }
                        estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    }
                    else
                    {
                        ApplicationController.log.Error("Error en Metodo (addTaquilla) codigo: " + response.Data.estatus.codigo);
                        if (ApplicationConstants.moddebug)
                        { ApplicationController.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data)); }
                        estatusd = response.Data.estatus;
                    }

                }
            }
            catch (Exception ex)
            {
                estatusd.codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                ApplicationController.log.Info("Error [Exception] (addTaquilla): " + ex);
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
                    if (trx.codtrx != null && trx.exists)
                    {
                        trx.codoficina = ticket.codoficina;
                        trx.fechaatencion = ticket.fechaatencion;
                        trx.fechaatencionD = ticket.fechaatencionD;
                        trx.horallegadaofic = ticket.horallegadaofic;
                        trx.horallegadaoficD = ticket.horallegadaoficD;
                        trx.idcedula = ticket.idcedula;
                        trx.nrocedula = ticket.nrocedula.Value;
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
                tk.nroticketcal = (tk.codtipocola.Value*100000)+ randomNumber;
            }

            if (tk.isValid)
            {
                ApplicationController.log.Info("Ticket: Registro Valido");
            }
            else
            {
                ApplicationController.log.Error("Ticket: Registro Invalido");
            }

            return tk;
        }

        private static TransactionDat getTransaction(int ntransac,int  ncampos,DateTime fechaatencion,string[] arrtrx)
        {
            string sdate = "";
            TransactionDat trx = new TransactionDat();
            trx.exists = true;
            
            try
            {
                trx.codtrx = arrtrx[ntransac];
                if(trx.codtrx ==null)
                {
                    trx.exists = false;
                }
                else
                {
                    if(trx.codtrx.Length == 0)
                    {
                        trx.exists = false;
                    }
                }
            }
            catch { trx.exists = false; }
            ntransac++;
            try
            {
                trx.montotrx = Convert.ToDecimal(arrtrx[ntransac]);
            }
            catch { trx.montotrx = 0; }
                ntransac++;
            try
            {   sdate = fechaatencion.ToString(ApplicationConstants.dfparm) + arrtrx[ntransac];
                trx.horafintrxD = DateTime.ParseExact(sdate, ApplicationConstants.dfform, null);
                trx.horafintrx = trx.horafintrxD.Value.ToString(ApplicationConstants.dfdispdateval);
            }
            catch { trx.exists = false; }

            if (trx.exists)
            {
                ApplicationController.log.Info("Transacción: Registro Valido");
            }
            else
            {
                ApplicationController.log.Error("Transacción: Registro Invalido, CodTrx: "+ trx.codtrx);
            }

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

        private static TicketDat normalizeTimeTicket(TicketDat ticket)
        {
            if(ticket.fechaatencionD !=null)
            {
                
                ticket.fechaatencion = ticket.fechaatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
                ticket.fechaatencionD = null;
            }
            if(ticket.horallegadaoficD != null)
            {
               
                ticket.horallegadaofic = ticket.horallegadaoficD.Value.ToString(ApplicationConstants.dfdispdateval);
                ticket.horallegadaoficD = null;
            }
            if (ticket.horainiatencionD != null)
            {
                ticket.horainiatencion = ticket.horainiatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
                ticket.horainiatencionD = null;
            }
            if (ticket.horafinatencionD != null)
            {
                ticket.horafinatencion = ticket.horafinatencionD.Value.ToString(ApplicationConstants.dfdispdateval);
                ticket.horafinatencionD = null;
            }

            return ticket;
        }

        public static RestClient getClientRest(string endpoint, int timeout)
        {
            RestClient client = new RestClient(endpoint);
            // Override with Newtonsoft JSON Handler
            client.AddHandler("application/json",  NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript",  NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json",  NewtonsoftJsonSerializer.Default);
            client.Timeout = timeout;
            return client;
        }

        public static RestRequest executeRest(string service, RestSharp.Method method, Object requestdat) 
        {
            RestRequest request = new RestRequest(service, method);
            //request.RequestFormat = DataFormat.Json;
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
