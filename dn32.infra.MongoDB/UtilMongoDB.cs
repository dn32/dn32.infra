using Newtonsoft.Json;
using System;

namespace dn32.infra
{
    public static class UtilMongoDB
    {
        public static DateTime LimparDataParaMongo(this DateTime data)
        { // Um cuidado para que o BD não use sua propria data
            return JsonConvert.DeserializeObject<DateTime>(JsonConvert.SerializeObject(data));
        }

        public static DateTime DataAtual => DateTime.Now.LimparDataParaMongo();
    }
}
