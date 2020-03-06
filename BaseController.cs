using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TatlaCas.Asp.Domain.Models.Common;
using TatlaCas.Asp.Domain.Repos;
using TatlaCas.Asp.Domain.Resources;
using Microsoft.AspNetCore.Mvc;
using TatlaCas.Asp.Web.ViewModels;
using TatlaCas.Asp.Domain;

namespace TatlaCas.Asp.Web
{
    public abstract class BaseController : Controller
    {
        protected async Task<JsonResult> RetrieveItems<TEntity, TResource>(IRepo<TEntity, TResource> repo,
            DataTableParams dataTableParams, Expression<Func<TEntity, bool>> queryExpr = null,
            List<string> includeRelationships = null) where TEntity : IEntity where TResource : IResource
        {
            var perPage = dataTableParams.Pagination.Perpage > 0 ? dataTableParams.Pagination.Perpage : 20;
            var page = dataTableParams.Pagination.Page > 0 ? dataTableParams.Pagination.Page : 1;
            var orderByExpr = new List<OrderByFieldNames>();
            if (dataTableParams.Sort != null)
            {
                orderByExpr.Add(new OrderByFieldNames
                {
                    Direction = dataTableParams.Sort.Sort == "asc" ? OrderBy.Ascending : OrderBy.Descending,
                    FieldName = dataTableParams.Sort.Field.First().ToString().ToUpper() +
                    dataTableParams.Sort.Field.Substring(1)
                });
                
            }

            var dt = await repo.ResourcesWhereAsync(queryExpr, null,orderByExpr, perPage,
                page, includeRelationships);
            var total = await repo.CountAsync(queryExpr);
            var totalPages = (int) Math.Ceiling(total / (decimal) perPage);
            return new JsonResult(new
            {
                meta = new DataTableMeta
                {
                    Page = page,
                    Pages = totalPages,
                    Perpage = perPage,
                    Total = total,
                },
                data = dt
            });
        }
        
        protected void PrepView(string highlightKey,string pageHeading,string subHeading=null)
        {
            ViewBag.HighlightedMenuKey = highlightKey;
            ViewBag.PageHeading = pageHeading;
            ViewBag.PageSubHeading = subHeading;
        }
    }
}