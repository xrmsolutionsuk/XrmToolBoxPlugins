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
        internal static FilePathSettings GenerateFileStructure(string selectedFileName, Settings mySettings) 
        {
            string originalSolutionFilePath = string.Format("{0}OriginalSolution.zip", mySettings.DefaultPathForTemporaryFiles);
            string holdingSolutionFilePath = string.Format("{0}HoldingSolution.zip", mySettings.DefaultPathForTemporaryFiles);
            string extractedSolutionFolderPath = string.Format("{0}Holding Solution", mySettings.DefaultPathForTemporaryFiles);
            if (File.Exists(originalSolutionFilePath))
            {
                File.Delete(originalSolutionFilePath);
            }
            File.Copy(selectedFileName, originalSolutionFilePath, true);
            if (Directory.Exists(extractedSolutionFolderPath))
            {
                Directory.Delete(extractedSolutionFolderPath, true);
            }
            Directory.CreateDirectory(extractedSolutionFolderPath);
            ZipFile.ExtractToDirectory(originalSolutionFilePath, extractedSolutionFolderPath);

            string solutionXmlLocation = string.Empty;
            if (File.Exists(string.Format("{0}\\solution.xml", extractedSolutionFolderPath)))
            {
                solutionXmlLocation = string.Format("{0}\\Solution.xml", extractedSolutionFolderPath);
            }
            else
            {
                solutionXmlLocation = string.Format("{0}\\Other\\Solution.xml", extractedSolutionFolderPath);
            }

            FilePathSettings fpS = new FilePathSettings();
            fpS.SolutionXmlFileLocation = solutionXmlLocation;
            fpS.OriginalSolutionFilePath = originalSolutionFilePath;
            fpS.ExtractedSolutionFolderPath = extractedSolutionFolderPath;
            fpS.HoldingSolutionFilePath = holdingSolutionFilePath;

            return fpS;
        }
    }
}
