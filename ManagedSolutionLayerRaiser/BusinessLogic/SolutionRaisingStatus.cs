namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal enum SolutionRaisingStatus
    {
        NotStarted,
        HoldingSolutionInstalledOriginalSolutionNotUninstalled,
        HoldingSolutionInstalledOriginalSolutionUninstalled,
        HoldingSolutionInstalledOriginalSolutionReinstalled,
        Complete
    }
}
