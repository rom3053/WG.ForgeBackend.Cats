using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp7.Cats.DALL.Helper
{
    public static class ConvertEnumToString
    {
        public static CatColor ConvertToEnum(string c) => c switch
        {
            "black" => CatColor.Black,
            "red" => CatColor.Red,
            "white" => CatColor.White,
            "black & white" => CatColor.Black | CatColor.White,
            "red & white" => CatColor.Red | CatColor.White,
            "red & black & white" => CatColor.Red | CatColor.Black | CatColor.White,
            _ => CatColor.None,
        };
    }
}
