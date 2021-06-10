using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApp7.Cats.DALL
{
    public partial class CatsStat
    {
        public decimal? TailLengthMean { get; set; }
        public decimal? TailLengthMedian { get; set; }
        public int[] TailLengthMode { get; set; }
        public decimal? WhiskersLengthMean { get; set; }
        public decimal? WhiskersLengthMedian { get; set; }
        public int[] WhiskersLengthMode { get; set; }
    }
}
