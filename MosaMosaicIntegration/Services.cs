using MosaMosaicIntegration.Controlador;
using MosaMosaicIntegration.Modelo;
using MosaMosaicIntegration.Modelo.Mensaje;
using Newtonsoft.Json;
using RestSharp;
using System;
using MosaMosaicIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RGiesecke.DllExport;
using log4net;

namespace MosaMosaicIntegration
{

    public class Services
    {
        private static bool publique;

        [DllExport("servicemmi", CallingConvention.Cdecl)]
        public static void servicemmi(String traza, ref long salida)
        {
            ApplicationController.configuracion();
            ApplicationController.log.Info("Lectura de traza");
            TrazaDat trazadat = new TrazaDat();
            /*Desencriptar traza*/
            trazadat = ApplicationController.decryptTrace(traza);
            /*Define la opeción*/
            if(trazadat.lstTrassaction.Count() > 0) { 
                if(trazadat.lstTrassaction.FirstOrDefault().codtrx.Equals(ApplicationConstants.codTranLogin))
                {
                    salida = long.Parse(ApplicationController.logIn(trazadat));

                }
                else if (trazadat.lstTrassaction.FirstOrDefault().codtrx.Equals(ApplicationConstants.codTranLogout))
                {
                    salida = long.Parse(ApplicationController.logOut(trazadat));
                }else if (trazadat.lstTrassaction.FirstOrDefault().codtrx.Equals(ApplicationConstants.codTranCallNext))
                {
                    salida = 0;
                }else
                {
                    salida = long.Parse(ApplicationController.registerTrasact(trazadat));
                }
            }
            else
            {
                salida = long.Parse(ApplicationController.registerTrasact(trazadat));
            }


        }

        [DllExport("test", CallingConvention.Cdecl)]
        public static void test (String traza, ref string salida)
        {
            try
            {

                Boolean asd = ApplicationController.configuracion();
                if(asd)
                {
                    salida = "" + 6;
                }
                else
                {
                    salida = "" + 4;
                }
                
            }
            catch (Exception ex)
            {
                
                salida = ex.ToString();
            }
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
