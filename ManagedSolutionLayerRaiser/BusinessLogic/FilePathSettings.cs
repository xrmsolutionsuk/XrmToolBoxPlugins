using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal class FilePathSettings
    {
        private string solutionXmlFileLocation;
        private string originalSolutionFilePath;
        private string holdingSolutionFilePath;
        private string extractedSolutionFolderPath;

        public string SolutionXmlFileLocation
        {
            get
            {
                return solutionXmlFileLocation;
            }
            set
            {
                solutionXmlFileLocation = value;
            }
        }

        public string OriginalSolutionFilePath
        {
            get
            {
                return originalSolutionFilePath;
            }
            set
            {
                originalSolutionFilePath = value;
            }
        }

        public string HoldingSolutionFilePath
        {
            get
            {
                return holdingSolutionFilePath;
            }
            set
            {
                holdingSolutionFilePath = value;
            }
        }

        public string ExtractedSolutionFolderPath
        {
            get
            {
                return extractedSolutionFolderPath;
            }
            set
            {
                extractedSolutionFolderPath = value;
            }
        }
    }
}
