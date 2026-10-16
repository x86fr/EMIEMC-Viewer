using System;
using System.IO;

namespace EMIEMC_Viewer
{
    // Picks the loader from the file content: '{' = TeraScan JSON (.tb), '<' = Tektronix XML (.emcemi)
    public static class MeasurementLoader
    {
        public static EmiMeasurement Load(string path)
        {
            int first = -1;

            using (StreamReader sr = new StreamReader(path, true))
            {
                int c;
                while ((c = sr.Read()) != -1)
                {
                    if (!char.IsWhiteSpace((char)c) && c != '﻿')
                    {
                        first = c;
                        break;
                    }
                }
            }

            if (first == '{')
                return TeraScanLoader.Load(path);
            if (first == '<')
                return TektronixLoader.Load(path);

            throw new InvalidDataException("Unknown file format (expected Tektronix .emcemi or TeraScan .tb).");
        }
    }
}
