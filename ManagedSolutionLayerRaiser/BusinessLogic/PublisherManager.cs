using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal static class PublisherManager
    {
        internal static EntityCollection GetPublishers(IOrganizationService orgService)
        {
            QueryExpression query = new QueryExpression("publisher");
            query.NoLock = true;
            query.ColumnSet = new ColumnSet(new string[] { "publisherid", "uniquename", "friendlyname", "customizationprefix", "customizationoptionvalueprefix" });
            query.AddOrder("friendlyname", OrderType.Ascending);
            PagingInfo pagingInfo = new PagingInfo();
            pagingInfo.PageNumber = 1;
            pagingInfo.PagingCookie = null;
            pagingInfo.Count = 500;
            query.PageInfo = pagingInfo;
            EntityCollection results = new EntityCollection();
            while (true)
            {
                EntityCollection publishersBatch = orgService.RetrieveMultiple(query);
                results.Entities.AddRange(publishersBatch.Entities);
                if (publishersBatch.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = publishersBatch.PagingCookie;
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
