using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal static class QueryManager
    {
        internal static EntityCollection GetManagedSolutions(string filterString, IOrganizationService orgService)
        {
            QueryExpression solutionQuery = new QueryExpression("solution");
            solutionQuery.NoLock = true;
            solutionQuery.ColumnSet = new ColumnSet(new string[] { "solutionid", "uniquename", "publisherid", "friendlyname", "version", "ismanaged", "installedon", "updatedon", "solutiontype", "isapimanaged", "parentsolutionid" });
            solutionQuery.AddOrder("uniquename", OrderType.Ascending);
            solutionQuery.AddOrder("version", OrderType.Ascending);
            LinkEntity publisherLink = new LinkEntity("solution", "publisher", "publisherid", "publisherid", JoinOperator.Inner);
            publisherLink.EntityAlias = "publisher";
            publisherLink.Columns = new ColumnSet(new string[] { "publisherid", "uniquename", "friendlyname", "customizationprefix", "customizationoptionvalueprefix" });
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                FilterExpression publisherFilters = new FilterExpression(LogicalOperator.Or);
                publisherFilters.AddCondition("uniquename", ConditionOperator.Like, string.Format("%{0}%", filterString));
                publisherFilters.AddCondition("friendlyname", ConditionOperator.Like, string.Format("%{0}%", filterString));
                publisherFilters.AddCondition("customizationprefix", ConditionOperator.Like, string.Format("%{0}%", filterString));
                publisherLink.LinkCriteria = publisherFilters;
            }
            solutionQuery.LinkEntities.Add(publisherLink);
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition("ismanaged", ConditionOperator.Equal, true);
            filter.AddCondition("isapimanaged", ConditionOperator.Equal, false);
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                FilterExpression subFiler = new FilterExpression(LogicalOperator.Or);
                subFiler.AddCondition("uniquename", ConditionOperator.Like, string.Format("%{0}%", filterString));
                subFiler.AddCondition("friendlyname", ConditionOperator.Like, string.Format("%{0}%", filterString));
                filter.AddFilter(subFiler);
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
