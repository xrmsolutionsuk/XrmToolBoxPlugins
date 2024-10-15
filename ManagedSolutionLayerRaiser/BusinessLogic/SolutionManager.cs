using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Threading;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal static class SolutionManager
    {
        internal static bool SolutionExists(IOrganizationService orgService, string solutionUniqueName, out DateTime? installDate)
        {
            QueryExpression query = new QueryExpression("solution");
            query.NoLock = true;
            query.ColumnSet = new ColumnSet(new string[] { "solutionid", "uniquename", "installedon" });
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition("uniquename", ConditionOperator.Equal, solutionUniqueName);
            query.Criteria = filter;
            EntityCollection solutions = orgService.RetrieveMultiple(query);
            if (solutions.Entities.Count > 0)
            {
                installDate = (DateTime)solutions.Entities[0]["installedon"];
                return true;
            }
            installDate = DateTime.MinValue;
            return false;
        }

        internal static bool ImportSolution(IOrganizationService orgService, string solutionFilePath, string solutionUniqueName)
        {
            DateTime? installDate = DateTime.MinValue;
            if (SolutionExists(orgService, solutionUniqueName, out installDate))
            {
                return true;
            }
            ImportSolutionAsyncRequest request = new ImportSolutionAsyncRequest();
            request.AsyncRibbonProcessing = false;
            request.ConvertToManaged = false;
            request.CustomizationFile = File.ReadAllBytes(solutionFilePath);
            request.HoldingSolution = false;
            request.SkipQueueRibbonJob = true;
            request.PublishWorkflows = true;
            request.SkipProductUpdateDependencies = true;
            ImportSolutionAsyncResponse response = (ImportSolutionAsyncResponse)orgService.Execute(request);
            string importJobKey = response.ImportJobKey;
            Guid systemJobId = response.AsyncOperationId;

            int currentStatus = 0;
            while (currentStatus != 30 && currentStatus != 31 && currentStatus != 32)
            {
                Thread.Sleep(30000);
                Entity asynoperation = orgService.Retrieve("asyncoperation", systemJobId, new ColumnSet(new string[] { "asyncoperationid", "statecode", "statuscode" }));
                currentStatus = ((OptionSetValue)asynoperation["statuscode"]).Value;
            }
            return (currentStatus == 30);
        }

        internal static bool DeleteSolution(IOrganizationService orgService, string solutionUniqueName)
        {
            DateTime? installDate = DateTime.MinValue;
            if (!SolutionExists(orgService, solutionUniqueName, out installDate))
            {
                return true;
            }
            else
            {
                UninstallSolutionAsyncRequest request = new UninstallSolutionAsyncRequest();
                request.SolutionUniqueName = solutionUniqueName;
                UninstallSolutionAsyncResponse response = (UninstallSolutionAsyncResponse)orgService.Execute(request);
                Guid systemJobId = response.AsyncOperationId;

                int currentStatus = 0;
                while (currentStatus != 30 && currentStatus != 31 && currentStatus != 32)
                {
                    Thread.Sleep(30000);
                    Entity asynoperation = orgService.Retrieve("asyncoperation", systemJobId, new ColumnSet(new string[] { "asyncoperationid", "statecode", "statuscode" }));
                    currentStatus = ((OptionSetValue)asynoperation["statuscode"]).Value;
                }
                return (currentStatus == 30);
            }
        }

        internal static EntityCollection GetManagedSolutions(IOrganizationService orgService, string filterString, Publisher publisher)
        {
            QueryExpression solutionQuery = new QueryExpression("solution");
            solutionQuery.NoLock = true;
            solutionQuery.ColumnSet = new ColumnSet(new string[] { "solutionid", "uniquename", "publisherid", "friendlyname", "version", "ismanaged", "installedon", "updatedon", "solutiontype", "isapimanaged", "parentsolutionid" });
            solutionQuery.AddOrder("uniquename", OrderType.Ascending);
            solutionQuery.AddOrder("version", OrderType.Ascending);
            LinkEntity publisherLink = new LinkEntity("solution", "publisher", "publisherid", "publisherid", JoinOperator.Inner);
            publisherLink.EntityAlias = "publisher";
            publisherLink.Columns = new ColumnSet(new string[] { "publisherid", "uniquename", "friendlyname", "customizationprefix", "customizationoptionvalueprefix" });
            solutionQuery.LinkEntities.Add(publisherLink);

            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition("ismanaged", ConditionOperator.Equal, true);
            filter.AddCondition("isapimanaged", ConditionOperator.Equal, false);
            filter.AddCondition("isvisible", ConditionOperator.Equal, true);
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                FilterExpression subFiler = new FilterExpression(LogicalOperator.Or);
                subFiler.AddCondition("uniquename", ConditionOperator.Like, filterString);
                subFiler.AddCondition("friendlyname", ConditionOperator.Like, filterString);
                filter.AddFilter(subFiler);
            }
            if (publisher != null)
            {
                filter.AddCondition("publisherid", ConditionOperator.Equal, publisher.ID);
            }
            solutionQuery.Criteria = filter;
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PageNumber = 1;
            pagingInfo.PagingCookie = null;
            pagingInfo.Count = 500;
            solutionQuery.PageInfo = pagingInfo;
            EntityCollection results = new EntityCollection();
            while (true)
            {
                EntityCollection solutionsBatch = orgService.RetrieveMultiple(solutionQuery);
                results.Entities.AddRange(solutionsBatch.Entities);
                if (solutionsBatch.MoreRecords)
                {
                    solutionQuery.PageInfo.PageNumber++;
                    solutionQuery.PageInfo.PagingCookie = solutionsBatch.PagingCookie;
                }
                else
                {
                    break;
                }
            }
            return results;
        }

    }
}
