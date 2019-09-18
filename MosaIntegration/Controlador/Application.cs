using log4net;
using MosaIntegration.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MosaIntegration.Controlador
{
    class Application
    {
        private static char sep = Path.DirectorySeparatorChar;
        private static string runDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private static readonly ILog log = LogManager.GetLogger(typeof(Application));

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
                var param = from r in xml.Descendants("entry") select new { key = r.Attribute("key").Value, value = r.Value.Trim() };
                ApplicationConstants.serviceEndpoint = param.Where(x => x.key.Equals("serviceEndPoint")).FirstOrDefault().value;
                try
                { 
                    ApplicationConstants.modtest = Boolean.Parse(param.Where(x => x.key.Equals("modetest")).FirstOrDefault().value);
                    ApplicationConstants.moddebug = Boolean.Parse(param.Where(x => x.key.Equals("debug")).FirstOrDefault().value);
                }
                catch
                {
                    ApplicationConstants.modtest = false;
                    ApplicationConstants.moddebug = false;
                }
                ApplicationConstants.timeZone = param.Where(x => x.key.Equals("timezone")).FirstOrDefault().value;
                validator = true;
            }
            else
            {
                log.Info("Error al Inicializar Configuración MMI");
                validator = false;
            }

            return validator;
            

        }

        public void logIn(String traza)
        {
            
        }

    }
}
