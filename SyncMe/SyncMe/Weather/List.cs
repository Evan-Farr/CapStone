using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyncMe.Weather
{
    public class List
    {
        public Temp temp { get; set; }
        public int humidity { get; set; }
        public List<Weather> weather { get; set; }
    }
}