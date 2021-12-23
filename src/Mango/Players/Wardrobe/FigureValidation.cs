using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Wardrobe
{
    static class FigureValidation
    {
        public static bool Validate(string Figure)
        {
            if (Figure.Length < 18 || Figure.Length > 150) // 138 = max length with all clothes on i use 150
            {
                return false;
            }

            string[] Sets = Figure.Split('.');

            if (Sets.Length > 13 || Sets.Length < 2)
            {
                return false;
            }

            bool HasHD = false; // hd = head
            bool HasLG = false; // lg = leg clothing

            bool CheckOthers = (Sets.Length > 2);
            bool OthersOK = false;

            //Console.WriteLine();
            //Console.WriteLine("SETS LENGTH: " + Sets.Length);

            foreach (string Set in Sets)
            {
                string[] Parts = Set.Split('-');

                if (Parts.Length < 3 || Parts.Length > 4)
                {
                    return false;
                }

                if (Parts[0].Length != 2) // names like hd, lg, wa, cc etc all length 2
                {
                    return false;
                }

                //Console.WriteLine(Parts[1]);
                //Console.WriteLine(Parts[2]);

                int Type = 0; // i think types
                int Colour = 0; // i think colour

                if (int.TryParse(Parts[1], out Type))
                {
                    if (Type < 1) // make sure a type is present
                    {
                        return false;
                    }

                    // dno how to validate this properly yet
                }

                if (int.TryParse(Parts[2], out Colour))
                {
                    if (Colour < 1)
                    {
                        return false;
                    }
                }

                foreach (string Part in Parts)
                {
                    //Console.WriteLine();
                    //Console.WriteLine("PART: " + Part);

                    if (Part == "hd")
                    {
                        HasHD = true;
                    }

                    if (Part == "lg")
                    {
                        HasLG = true;
                    }

                    if (CheckOthers)
                    {
                        // all parts currently available
                        if (Part == "wa" || Part == "cc" || Part == "fa" || Part == "ca" || Part == "ch" || Part == "he" || Part == "ea" || Part == "cp" || Part == "ha" || Part == "sh")
                        {
                            OthersOK = true;
                        }
                    }
                }
            }

            if (!HasHD || !HasLG)
            {
                return false;
            }

            if (CheckOthers && !OthersOK)
            {
                return false;
            }

            return true;
        }
    }
}
