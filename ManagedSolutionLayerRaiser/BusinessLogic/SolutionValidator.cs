using System.Xml.Linq;
using System.Xml.XPath;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal static class SolutionValidator
    {
        internal static bool IsValid(XDocument solutionDoc, Solution selectedSolution)
        {
            var solutionUniqueName = solutionDoc.XPathSelectElement("/ImportExportXml/SolutionManifest/UniqueName").Value;
            var solutionVersion = solutionDoc.XPathSelectElement("/ImportExportXml/SolutionManifest/Version").Value;
            var solutionPublisher = solutionDoc.XPathSelectElement("/ImportExportXml/SolutionManifest/Publisher/UniqueName").Value;
            var solutionType = solutionDoc.XPathSelectElement("/ImportExportXml/SolutionManifest/Managed").Value;

            if (solutionUniqueName != selectedSolution.UniqueName ||
                solutionVersion != selectedSolution.Version ||
                solutionPublisher != selectedSolution.PublisherUniqueName ||
                solutionType != "1")
            {
                return false;
            }
            return true;
        }
    }
}
