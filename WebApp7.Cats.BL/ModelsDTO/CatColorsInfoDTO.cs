using System;
using System.Collections.Generic;
using System.Text;
using WebApp7.Cats.DALL;

namespace WebApp7.Cats.BL.ModelsDTO
{
    public class CatColorsInfoDTO
    {
        public CatColorsInfoDTO()
        {
                
        }
        public CatColorsInfoDTO(string color, int? count)
        {
            this.Count = count;
            this.Color = color;

        }
        public string Color { get; set; }
        public int? Count { get; set; }
    }
}
