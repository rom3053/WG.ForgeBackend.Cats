using System;
using System.Collections.Generic;
using NpgsqlTypes;
// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace WebApp7.Cats.DALL
{
    public partial class Cats
    {
        public Cats()
        {

        }
        public string Name { get; set; }
        public int? TailLength { get; set; }
        public int? WhiskersLength { get; set; }
        public CatColor Color { get; set; }

        public override string ToString()
        {
            return $"{Name} {TailLength} {WhiskersLength} {Color}";
        }
    }
    //old variant
    [Flags]
    public enum Color
    {
        None = 0,
        Black = 1,
        White = 2,
        Red = 4,
    }

    public enum CatColor
    {
        None = 0,
        [PgName("black")]
        Black = 1,
        [PgName("white")]
        White = 2,
        [PgName("red")]
        Red = 3,
        [PgName("black & white")]
        Black_N_White = 4,
        [PgName("red & white")]
        Red_N_White = 5,
        [PgName("red & black & white")]
        Red_N_Black_N_White = 6
        
    }
}
