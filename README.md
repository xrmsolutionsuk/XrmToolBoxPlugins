# XrmToolBox Plugins published by Xrm Solutions UK
This repository contains the source code for [XrmToolBox](https://www.xrmtoolbox.com/) plugins created and published by [Xrm Solutions UK](https://www.xrmsolutionsuk.com) for general use by the community. It includes the plugins detailed in the following sections

## Managed Solution Layer Raiser
The _Managed Solution Layer Raiser_ plugin allows you to quickly and easily raise your organization-owned managed solutions to the top of the stack of managed layers to resolve environmental inconsistency and make your applications behave as designed.

### Use Case
Occasionally when Microsoft or other ISV solutions that you take dependencies on ship updates to their products as brand new solutions, because of the way solution layering works, these new solutions will occupy the top most layer of managed solutions in the layering stack. In your non-development environments, this is problematic because you want your organizations owned customisations to take precedence. 

This tool allows you to automatically raise your organization's owned solutions to the top of the layering stack to ensure your customisations take effect. It does this using the 'holding solution' concept (as detailed below) in a safe manner without needing to fully uninstall your solutions, thereby avoiding any adverse effects like loss of data.

### Directions for Use
When you invoke the tool, you are prompted to supply a managed solution zip file corresponding to the solution you select to be raised. The tool validates that the Publisher and version of the selected solution match that of the supplied managed solution zip file. If there's a mismatch in any of these attributes then the operations do not continue. Otherwise, it continue as follows:

1. Changes the unique name of the supplied solution zip file to make the platform treat it as a unique solution
2. Installs the renamed solution, which, as a result of the rename, occupies the top-most layer of the stack of managed solutions
3. Uninstalls the selected solution original copy. This is safe at the stage because the newly installed solution in the previous step holds references to the containing objects. Therefore, no objects should be deleted as a result of this uninstallation
4. Reinstalls the original solution with the original name. Since the original copy of the solution was uninstalled in the previous step, it is now treated as a new solution by the platform and so it occupies the top-most position in the stack of managed solutions
5. Finally the tool uninstalls the renamed solution copy

### Important Notes
1. The tool peforms solution installations and uninstallations. Therefore it is recommended that you observe your normal environment operational support and maintenance practices whilst using this tool. For example, taking a back-up of the environment before starting the process, using an appropriate account with system administrator rights, taking any necessary change control actions etc
2. Occasionally the tool can fail due to unexpected circumstances (eg. Microsoft updates being installed at the same time). This may just be a UI failure because solution lifecycle operations are executed on the server side asynchronously. If this happens review, it is recommended that you review Solution History in the environment to determine the cause and retry later when any ongoing solution lifecycle operations are complete. The tool is resilient and intelligent enough to resume the process from the stage that it reached successfully during the previous attempt by detecting the current installation/uninstallation status upon retry. Therefore it will not attempt to retry any steps that it has previously completed successfully.


# Support
For any queries or support, please raise an issue in the repository. 

# Contributions
You are welcome to contribute to the project. Please fork the repository and then raise a Pull Request to merge your changes
