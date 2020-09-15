using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;

namespace LicenseManager.Server.Controllers
{
    public static class ControllerExtensions
    {
        public static IActionResult PerformGridOerations<T>(this Controller controller, IEnumerable<T> gridObjects, DataManagerRequest dm) where T : class
        {
            var operation = new DataOperations();

            if (dm.Search != null && dm.Search.Count > 0)
            {
                gridObjects = operation.PerformSearching(gridObjects, dm.Search);
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                gridObjects = operation.PerformSorting(gridObjects, dm.Sorted);
            }

            if (dm.Where != null && dm.Where.Count > 0)
            {
                gridObjects = operation.PerformFiltering(gridObjects, dm.Where, dm.Where[0].Operator);
            }

            var count = gridObjects.Count();

            if (dm.Skip != 0)
            {
                gridObjects = operation.PerformSkip(gridObjects, dm.Skip);
            }

            if (dm.Take != 0)
            {
                gridObjects = operation.PerformTake(gridObjects, dm.Take);
            }

            if (dm.RequiresCounts)
            {
                return controller.Json(new { result = gridObjects, count });
            }
            else
            {
                return controller.Json(gridObjects);
            }
        }
    }
}
