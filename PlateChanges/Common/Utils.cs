
using System.Runtime.ExceptionServices;

namespace PlateChanges.Common
{
    internal static class Utils
    {
        #region Levels

        public static List<Level> GetAllLevels(Document curDoc)
        {
            return new FilteredElementCollector(curDoc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .OfType<Level>()
                .ToList();           
        }

        internal static List<Level> GetFilteredAndSortedLevels(Document curDoc)
        {
            // get all the levels
            List<Level> m_listLevels = Utils.GetAllLevels(curDoc);

            // filter and sort using ProjectConstants
            return m_listLevels
                .Where(level => !ProjectConstants.ExcludedLevelNames.Contains(level.Name))
                .OrderBy(level => level.Elevation)
                .ToList();
        }

        #endregion

        #region Ribbon Panel
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel curPanel;

            if (GetRibbonPanelByName(app, tabName, panelName) == null)
                curPanel = app.CreateRibbonPanel(tabName, panelName);

            else
                curPanel = GetRibbonPanelByName(app, tabName, panelName);

            return curPanel;
        }        

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }

        #endregion
    }
}
