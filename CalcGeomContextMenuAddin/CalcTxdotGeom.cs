using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace CalcGeomContextMenuAddin
{
    internal class CalcTxdotGeom : Button
    {
        protected async override void OnClick()
        {
            // Run on the MCT async
             await QueuedTask.Run(() =>
            {
                ////// PATH TO TOOL USING TOOLBOX ALIAS ////
                string tool_path = "management.CalculateGeometryAttributes";

                //// TOOL ARGUMENTS ////
                var in_features = MapView.Active.Map.FindLayers("TxDOT_Roadways_Edits").First() as BasicFeatureLayer;
                string geometry_property = "SEG_LEN LENGTH"; 
                string length_unit = "MILES_US";
                string area = null;
                var spatial_ref = SpatialReferenceBuilder.CreateSpatialReference(3081);

                //// CANCELABLE PROGRESS DIALOG BOX ////
                var progDlg = new ProgressDialog("Calculating Geometry...", "Cancel", 1, false);
                var progSrc = new CancelableProgressorSource(progDlg);

                //// CONSTRUCT THE VALUE ARRAY ////
                var args = Geoprocessing.MakeValueArray(in_features, geometry_property, length_unit, area, spatial_ref);
                
                //// RUN GP TOOL ASYNC ////
                Geoprocessing.ExecuteToolAsync(tool_path, args, null, 
                    new CancelableProgressorSource(progDlg).Progressor, GPExecuteToolFlags.Default);
            });
        }
    }

}
