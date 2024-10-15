using Microsoft.Xrm.Sdk;
using System;
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


            if ((solutionUniqueName != selectedSolution.UniqueName && (solutionUniqueName + "_Holding") != selectedSolution.UniqueName) ||
                solutionVersion != selectedSolution.Version ||
                solutionPublisher != selectedSolution.PublisherUniqueName ||
                solutionType != "1")
            {
                return false;
            }

            return true;
        }

        internal static SolutionRaisingStatus GetSolutionRaisingStatus(IOrganizationService orgService, string solutionUniqueName)
        {
            DateTime? originalSolutionInstallationDate = DateTime.MinValue;
            bool originalSolutionInstalled = SolutionManager.SolutionExists(orgService, solutionUniqueName, out originalSolutionInstallationDate);

            DateTime? holdingSolutionInstallationDate = DateTime.MinValue;
            bool holdingSolutionInstalled = SolutionManager.SolutionExists(orgService, solutionUniqueName + "_Holding", out holdingSolutionInstallationDate);

            DateTime currentDateTime = DateTime.UtcNow;

            if (originalSolutionInstalled && !holdingSolutionInstalled)
            {
                if (originalSolutionInstallationDate < currentDateTime.AddMinutes(-60))
                {
                    return SolutionRaisingStatus.NotStarted;
                }
                else
                {
                    return SolutionRaisingStatus.Complete;
                }
            }
            else if (originalSolutionInstalled && holdingSolutionInstalled)
            {
                if (originalSolutionInstallationDate < holdingSolutionInstallationDate)
                {
                    return SolutionRaisingStatus.HoldingSolutionInstalledOriginalSolutionNotUninstalled;
                }
                else
                {
                    return SolutionRaisingStatus.HoldingSolutionInstalledOriginalSolutionReinstalled;
                }
            }
            else
            {
                return SolutionRaisingStatus.HoldingSolutionInstalledOriginalSolutionUninstalled;
            }
        }
    }
}
