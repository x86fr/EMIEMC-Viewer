using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMIEMC_Viewer
{
    public class Peaks
    {
        public double freq { get; set; }
        public double value { get; set; }
        public int pos { get; set; }
        public double margin { get; set; }

        public Peaks(double f, double v, int p)
        {
            freq = f;
            value = v;
            pos = p;
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}
