﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MosaMosaicIntegration.Modelo.Mensaje
{
    class TaquillaActivarResponse
    {
        [JsonProperty("taquilla")]
        public TaquillaDat taquilla { set; get; }
        [JsonProperty("estatus")]
        public EstatusDat estatus { set; get; }


    }
}
