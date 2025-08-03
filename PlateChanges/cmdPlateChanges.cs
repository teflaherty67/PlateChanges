using PlateChanges.Common;

namespace PlateChanges
{
    [Transaction(TransactionMode.Manual)]
    public class cmdPlateChanges : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document curDoc = uidoc.Document;

            // get data for the form
            List<Level> filteredLevels = Utils.GetFilteredAndSortedLevels(curDoc);

            // get all the ViewSection views
            List<View> listViews = Utils.GetAllSectionViews(curDoc);

            // get the first view whose Title on Sheet is "Front Elevation"
            View elevFront = listViews
                .FirstOrDefault(v => v.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION)?.AsString() == "Front Elevation");

            // set that view as the active view
            if (elevFront != null)
            {
                uidoc.ActiveView = elevFront;
            }
            else
            {
                Utils.TaskDialogInformation("Information", "Spec Conversion", "Front Elevation view not found. Proceeding with level adjustments in current view.");
            }

            // create a counter for summary report
            int countAdjusted = 0;

            // launch the form with level data
            frmPlateChanges curForm = new frmPlateChanges(filteredLevels);
            curForm.Topmost = true;

            // check if user clicked OK and process the results
            if(curForm.ShowDialog() == true)
            {
                // get data from the form
                Dictionary<Level, double> levelAdjustments = curForm.LevelAdjustments;

                // create & start a transaction
                using (Transaction t = new Transaction(curDoc, "Plate Height Adjustements"))
                {
                    // process the data from the form
                    t.Start();                    

                    // loop through the dictionary
                    foreach(var kvp in levelAdjustments)
                    {
                        // get the key value pairs
                        Level level = kvp.Key;
                        double adjustment = kvp.Value;

                        // only adjust if value is valid
                        if (adjustment != 0)
                        {
                            level.Elevation = level.Elevation + adjustment;

                            // increment the counter
                            countAdjusted++;
                        }
                    }

                    t.Commit();
                }

                // summary report
                Utils.TaskDialogInformation("Information", "Level Adjustments",
            $"{countAdjusted} level{(countAdjusted == 1 ? "" : "s")} {(countAdjusted == 1 ? "was" : "were")} adjusted.");

            }

            return Result.Succeeded;
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData.Data;
        }
    }
}
