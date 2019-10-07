using MosaMosaicIntegration.Controlador;
using MosaMosaicIntegration.Modelo;
using MosaMosaicIntegration.Modelo.Mensaje;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration
{
    class Services
    {
        

        public static void servicemmi(String traza, ref long salida)
        {
            Application.configuracion();
            Application.log.Info("Lectura de traza");
            TrazaDat trazadat = new TrazaDat();
            /*Desencriptar traza*/
            trazadat = Application.decryptTrace(traza);
            /*Define la opeción*/
            if(trazadat.lstTrassaction.FirstOrDefault().codtrx.Equals(ApplicationConstants.codTranLogin))
            {
                salida = long.Parse(logIn(trazadat));

            }
            else if (trazadat.lstTrassaction.FirstOrDefault().codtrx.Equals(ApplicationConstants.codTranLogout))
            {
                salida = long.Parse(logOut(trazadat));
            }else if (trazadat.lstTrassaction.FirstOrDefault().codtrx.Equals(ApplicationConstants.codTranCallNext))
            {
                salida = 0;
            }else
            {
                //salida = long.Parse(registerTrasact(trazadat));
                salida = 0;
            }

           




        }

        public static String logIn(TrazaDat trazadat)
        {
            Application.log.Info("Inicia Operación de LogIn");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;

            /*Crear request data*/
            TaquillaActivarRequest request = new TaquillaActivarRequest
            {
                carnetatencion = Application.lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value,
                codoficina = trazadat.ticket.codoficina,
                nom_red_ofic = trazadat.ticket.nom_red_ofic
            };
            /*Realizar la peticion*/
            RestClient client = Application.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPATCH);
            RestRequest requestEntity = Application.executeRest(ApplicationConstants.taquillaLogInEndpoint, Method.PATCH, request);
            IRestResponse<TaquillaActivarResponse> response = client.Execute<TaquillaActivarResponse>(requestEntity);
            //JsonConvert.DeserializeObject()
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                Application.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                Application.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                codigo = response.Data.estatus.codigo;
            }
            else
            {
                Application.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                if (!response.IsSuccessful)
                {
                    codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    Application.log.Info("RESPONSE: "+ response.ErrorMessage);
                }
                else
                {
                    Application.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    Application.log.Error("Error en Metodo (logIn) codigo: " + response.Data.estatus.codigo);
                    codigo = response.Data.estatus.codigo;
                }      
            }
            Application.log.Info("Finaliza Operación de LogIn");
            return codigo;
        }

        public static String logOut(TrazaDat trazadat)
        {
            Application.log.Info("Inicia Operación de LogOut");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;

            /*Crear request data*/
            TaquillaDesactivarRequest request = new TaquillaDesactivarRequest
            {
                carnetatencion = Application.lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value,
                codoficina = trazadat.ticket.codoficina,
                nom_red_ofic = trazadat.ticket.nom_red_ofic
            };
            /*Realizar la peticion*/
            RestClient client = Application.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPATCH);
            RestRequest requestEntity = Application.executeRest(ApplicationConstants.taquillaLogOutEndpoint, Method.PATCH, request);
            IRestResponse<TaquillaDesactivarResponse> response = client.Execute<TaquillaDesactivarResponse>(requestEntity);
            //JsonConvert.DeserializeObject()
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                Application.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                Application.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                codigo = response.Data.estatus.codigo;
            }
            else
            {
                Application.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                if (!response.IsSuccessful)
                {
                    codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    Application.log.Info("RESPONSE: " + response.Content.ToString());
                }
                else
                {
                    Application.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    Application.log.Error("Error en Metodo (logIn) codigo: " + response.Data.estatus.codigo);
                    codigo = response.Data.estatus.codigo;
                }
            }
            Application.log.Info("Finaliza Operación de LogOut");
            return codigo;
        }

        public static String registerTrasact(TrazaDat trazadat)
        {
            Application.log.Info("Inicia Operación de Registro de Transaccion");
            string codigo = ApplicationConstants.TRANSACCION_EXITOSA;

            /*Crear request data*/
            TaquillaDesactivarRequest request = new TaquillaDesactivarRequest
            {
                carnetatencion = Application.lstCampoHeader.Where(x => x.nombre == "carnetUsuario").FirstOrDefault().value,
                codoficina = trazadat.ticket.codoficina,
                nom_red_ofic = trazadat.ticket.nom_red_ofic
            };
            /*Realizar la peticion*/
            RestClient client = Application.getClientRest(ApplicationConstants.serviceEndpoint, ApplicationConstants.timeoutPATCH);
            RestRequest requestEntity = Application.executeRest(ApplicationConstants.taquillaLogOutEndpoint, Method.PATCH, request);
            IRestResponse<TaquillaDesactivarResponse> response = client.Execute<TaquillaDesactivarResponse>(requestEntity);
            //JsonConvert.DeserializeObject()
            if (response.IsSuccessful && response.Data.estatus.codigo.Equals(ApplicationConstants.TRANSACCION_EXITOSA))
            {
                Application.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                Application.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                codigo = response.Data.estatus.codigo;
            }
            else
            {
                Application.log.Info("REQUEST: " + JsonConvert.SerializeObject(request));
                if (!response.IsSuccessful)
                {
                    codigo = ApplicationConstants.ERROR_DE_COMUNICACION;
                    Application.log.Info("RESPONSE: " + response.Content.ToString());
                }
                else
                {
                    Application.log.Info("RESPONSE: " + JsonConvert.SerializeObject(response.Data));
                    Application.log.Error("Error en Metodo (logIn) codigo: " + response.Data.estatus.codigo);
                    codigo = response.Data.estatus.codigo;
                }
            }
            Application.log.Info("Finaliza Operación de LogOut");
            return codigo;
        }





    }
}
