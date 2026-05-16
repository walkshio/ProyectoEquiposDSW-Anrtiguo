using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa.Entidades
{
    public class TipoCambio
    {
        public string result { get; set; }
        public double conversion_rate { get; set; }
        public string base_code { get; set; }
        public string target_code { get; set; }
    }

    public class TipTecnico
    {
        public Slip slip { get; set; }
    }

    public class Slip
    {
        public int id { get; set; }
        public string advice { get; set; }
    }
}
