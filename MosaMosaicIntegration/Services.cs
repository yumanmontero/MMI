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
using System.IO;
using System.Reflection;
using log4net.Config;

namespace MosaMosaicIntegration
{

    public class Services
    {
        private static bool publique;
        private static string runningpathEXE = @"" + Directory.GetCurrentDirectory();
        private static string runningpathDLL = @"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        [DllExport("servicemmi", CallingConvention.Cdecl)]
        public static void servicemmi(String traza, ref long salida)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
            changeRunDirectory(runningpathDLL);
            loadLogConfig();

           ApplicationController.configuracion();
           //ApplicationController.log.Info("Lectura de traza");

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
            changeRunDirectory(runningpathEXE);

        }

        [DllExport("test", CallingConvention.Cdecl)]
        public static void test (String traza, ref string salida)
        {
            try
            {
                
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
                changeRunDirectory(runningpathDLL);
                loadLogConfig();
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
            changeRunDirectory(runningpathEXE);

        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private static void loadLogConfig()
        {
            using (FileStream fs = new FileStream("log4net.config", FileMode.Open))
            {
                XmlConfigurator.Configure(fs);
            }
        }

        private static void changeRunDirectory(string ruta)
        {
            Directory.SetCurrentDirectory(ruta);
            Environment.CurrentDirectory = ruta;
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
