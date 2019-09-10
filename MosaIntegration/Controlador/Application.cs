using log4net;
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
        private static string appName = "mmi";
        private static char sep = Path.DirectorySeparatorChar;
        private static string runDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private static readonly ILog log = LogManager.GetLogger(typeof(Application));

        public Application()
        {
            /*Inicializar configuracion*/
            configuracion();
        }

        public void configuracion()
        {
            string configFileName = @"" + runDirectory + sep + appName + ".xml";
            if (File.Exists(configFileName))
            {
                var xml = XDocument.Load(configFileName);
                    
            }
            else
            {
                log.Info("Error al Inicializar Configuración MMI");

            }
            

        }

    }
}
