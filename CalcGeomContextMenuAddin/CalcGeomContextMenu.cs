﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace CalcGeomContextMenuAddin
{
    internal class CalcGeomContextMenu : Module
    {
        private static CalcGeomContextMenu _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static CalcGeomContextMenu Current
        {
            get
            {
                return _this ?? (_this = (CalcGeomContextMenu)FrameworkApplication.FindModule("CalcGeomContextMenuAddin_Module"));
            }
        }

        public static void CreateNotification(string message, string title, string uriString)
        {
            FrameworkApplication.AddNotification(new Notification()
            {
                Message = message,
                Title = title,
                ImageUrl = new Uri(uriString, UriKind.Absolute).AbsoluteUri
            });
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

    }
}
