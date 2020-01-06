using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ArcGIS.Desktop.Editing.Attributes;
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
        protected override void OnClick()
        {
            try
            {
                //// CANCELABLE PROGRESS DIALOG BOX ////
                //var progDlg = new ProgressDialog("Calculating Geometry...", "Cancel", 1, false);
                //var progSrc = new CancelableProgressorSource(progDlg);

                var layer = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().First(l => l.Name.Contains("TxDOT_Roadways_Edits"));

                QueuedTask.Run(() =>
                {
                    var selFeats = layer.GetSelection();
                    IReadOnlyList<long> SelectedOIDs = selFeats.GetObjectIDs();

                    var calcGeomInspector = new Inspector();

                    //for loop to add route numbers to any CR or CS add routes
                    foreach (int oid in SelectedOIDs)
                    {
                        var modifyRteFeatures = new EditOperation();
                        modifyRteFeatures.Name = "TxDOT Calc Geom";
                        calcGeomInspector.Load(layer, oid);
                        var shp = calcGeomInspector["SHAPE"] as Polyline;
                        double length = shp.Length;
                        var unit = shp.SpatialReference.Unit.ToString();
                        //MessageBox.Show(shp.Length.ToString());
                        //System.Threading.Thread.Sleep(1);

                        if (unit == "Meter")
                            calcGeomInspector["SEG_LEN"] = Math.Round((length * 0.000621371), 3);
                        else if (unit == "Foot")
                            calcGeomInspector["SEG_LEN"] = Math.Round((length * 0.000189394), 3);
                        else if (unit == "US Survey Foot")
                            calcGeomInspector["SEG_LEN"] = Math.Round((length * 0.000189394), 3);
                        else if (unit == "Kilometer")
                            calcGeomInspector["SEG_LEN"] = Math.Round((length * 0.001), 3);
                        else if (unit == "Mile")
                            calcGeomInspector["SEG_LEN"] = Math.Round(length, 3);

                        modifyRteFeatures.Modify(calcGeomInspector);
                        modifyRteFeatures.Execute();

                    }


                }); //, progSrc.Progressor

                CalcGeomContextMenu.CreateNotification("Length Calculated", "Success", "pack://application:,,,/CalcGeomContextMenuAddin;component/Images/texCalcNot.png");
            }

            catch (Exception e)
            {
                CalcGeomContextMenu.CreateNotification(String.Format("{0}", e), "ERROR", "pack://application:,,,/CalcGeomContextMenuAddin;component/Images/texCalcNot.png");
                Trace.WriteLine(e);
            }

            // Run on the MCT async
            // await QueuedTask.Run(() =>
            //{
            //    ////// PATH TO TOOL USING TOOLBOX ALIAS ////
            //    string tool_path = "management.CalculateGeometryAttributes";

            //    //// TOOL ARGUMENTS ////
            //    var in_features = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().First(l => l.Name.Contains("TxDOT_Roadways_Edits"));
            //    //var in_features = MapView.Active.Map.FindLayers("TxDOT_Roadways_Edits").First() as BasicFeatureLayer;
            //    string geometry_property = "SEG_LEN LENGTH"; 
            //    string length_unit = "MILES_US";
            //    string area = null;
            //    var spatial_ref = SpatialReferenceBuilder.CreateSpatialReference(3081);

            //    //// CANCELABLE PROGRESS DIALOG BOX ////
            //    var progDlg = new ProgressDialog("Calculating Geometry...", "Cancel", 1, false);
            //    var progSrc = new CancelableProgressorSource(progDlg);

            //    //// CONSTRUCT THE VALUE ARRAY ////
            //    var args = Geoprocessing.MakeValueArray(in_features, geometry_property, length_unit, area, spatial_ref);

            //    //// RUN GP TOOL ASYNC ////
            //    Geoprocessing.ExecuteToolAsync(tool_path, args, null, 
            //        new CancelableProgressorSource(progDlg).Progressor, GPExecuteToolFlags.Default);
            //});
        }
    }

}
