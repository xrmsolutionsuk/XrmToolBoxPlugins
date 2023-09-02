using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal static class FileStructureGenerator
    {
        internal static string GenerateFileStructure(string selectedFileName) 
        {
            if (File.Exists("OriginalSolution.zip"))
            {
                File.Delete("OriginalSolution.zip");
            }
            File.Copy(selectedFileName, "OriginalSolution.zip", true);
            if (Directory.Exists("Holding Solution"))
            {
                Directory.Delete("Holding Solution", true);
            }
            Directory.CreateDirectory("Holding Solution");
            ZipFile.ExtractToDirectory("OriginalSolution.zip", "Holding Solution");

            string solutionXmlLocation = string.Empty;
            if (File.Exists("Holding Solution\\solution.xml"))
            {
                solutionXmlLocation = "Holding Solution\\Solution.xml";
            }
            else
            {
                solutionXmlLocation = "Holding Solution\\Other\\Solution.xml";
            }
            return solutionXmlLocation;
        }
    }
}
