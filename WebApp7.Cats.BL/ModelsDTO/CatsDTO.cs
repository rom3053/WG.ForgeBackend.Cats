using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp7.Cats.BL
{
    public class CatsDTO
    {
        public CatsDTO()
        {

        }
        public CatsDTO(string name, int? tailLenght, int? whiskersLength, string color)
        {
            this.Name = name;
            this.TailLength = tailLenght;
            this.WhiskersLength = whiskersLength;
            this.Color = color;

        }
        public string Name { get; set; }
        public int? TailLength { get; set; }
        public int? WhiskersLength { get; set; }
        public string Color { get; set; }
    }
}
