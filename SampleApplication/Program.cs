using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gbrueckl.PowerBI.API;
using gbrueckl.PowerBI.API.PowerBIObjects;
using System.Configuration;
using System.Threading;
using System.Data;

namespace SampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sample_Create_Model();

            //Sample_DataTable();

            Sample_Dataset_Refresh();

            Console.Write("Press <ENTER> to quit. ");
            Console.ReadLine();
        }

        private static void Sample_Create_Model()
        {
            // AppID: the ID of the Azure AD Application
            string ApplicationID = ConfigurationManager.AppSettings["PBI_ApplicationID"];

            PBIAPIClient pbic = new PBIAPIClient(ApplicationID);

            string datasetName = "MyPushDataset";
            string tableNameFacts = "MySalesTable";
            string tableNameProducts = "MyProductTable";

            PBIDataset dataset = new PBIDataset(datasetName);
            dataset.PBIDefaultMode = PBIDefaultMode.Push;
            dataset.ParentPowerBIAPI = pbic;
            dataset.SyncFromPowerBI(); // check if a Dataset with the same ID or Name already exists in the PowerBI-Service

            PBITable salesTable = new PBITable(tableNameFacts); // create a PBI table manually
            salesTable.Columns.Add(new PBIColumn("ProductKey", PBIDataType.Int64) { IsHidden = true }); // hiding a column
            salesTable.Columns.Add(new PBIColumn("SalesDate", PBIDataType.DateTime) { FormatString = "yyyy-MM-dd" }); // setting the Formatstring
            salesTable.Columns.Add(new PBIColumn("Amount_BASE", PBIDataType.Double) { FormatString = "$ #,##0.00", IsHidden = true });

            salesTable.Measures.Add(new PBIMeasure("Sales Amount", "SUM('{0}'[{1}])", tableNameFacts, "Amount_BASE")); // adding a measure

            // create a regular DataTable - but could also be derived from a SQL Database!
            DataTable dataTable = new DataTable(tableNameProducts);
            dataTable.Clear();
            dataTable.Columns.Add("ProductKey", typeof(int));
            dataTable.Columns.Add("ProductName", typeof(string));
            DataRow prod1 = dataTable.NewRow();
            prod1["ProductKey"] = 1;
            prod1["ProductName"] = "Bikes";
            dataTable.Rows.Add(prod1);

            DataRow prod2 = dataTable.NewRow();
            prod2["ProductKey"] = 2;
            prod2["ProductName"] = "Clothing";
            dataTable.Rows.Add(prod2);

            // create a PBI table from a regular DataTable object
            PBITable productsTable = new PBITable(dataTable);

            dataset.AddOrUpdateTable(salesTable);
            dataset.AddOrUpdateTable(productsTable);

            dataset.Relationships.Add(new PBIRelationship("MyRelationship", salesTable.GetColumnByName("ProductKey"), productsTable.GetColumnByName("ProductKey")));

            Console.Write("Publishing to PowerBI Service ... ");
            dataset.PublishToPowerBI();
            Console.WriteLine("Done!");

            salesTable.DeleteRowsFromPowerBI();
            PBIRow row = salesTable.GetSampleRow();
            row.SetValue("ProductKey", 1);
            row.SetValue("SalesDate", DateTime.Now);
            row.SetValue("Amount_BASE", 100);
            salesTable.PushRowToPowerBI(row);

            salesTable.PushRowToPowerBI(new PBIRow(new Dictionary<string, object> { { "ProductKey", 2 }, { "SalesDate", DateTime.Now }, { "Amount_BASE", 50.90 } }));
            salesTable.PushRowToPowerBI(new PBIRow(new Dictionary<string, object> { { "ProductKey", 1 }, { "SalesDate", DateTime.Now }, { "Amount_BASE", 70.3 } }));
            salesTable.PushRowToPowerBI(new PBIRow(new Dictionary<string, object> { { "ProductKey", 2 }, { "SalesDate", DateTime.Now }, { "Amount_BASE", 150.70 } }));

            productsTable.PushRowsToPowerBI();
        }

        private static void Sample_DataTable()
        {
            // AppID: the ID of the Azure AD Application
            string ApplicationID = ConfigurationManager.AppSettings["PBI_ApplicationID"];

            PBIAPIClient powerBIClient = new PBIAPIClient(ApplicationID);

            PBIDataset dataset = new PBIDataset("MyPushDataset");
            dataset.PBIDefaultMode = PBIDefaultMode.Push;
            dataset.ParentPowerBIAPI = powerBIClient;
            dataset.SyncFromPowerBI(); // check if a Dataset with the same ID or Name already exists in the PowerBI-Service

            // create a regular DataTable - but could also be derived from a SQL Database!
            DataTable dataTable = new DataTable();
            /* populate the dataTable */
            // create a PBI table from a regular DataTable object
            PBITable productsTable = new PBITable(dataTable);
            // publish the table and push the rows from the dataTable to the PowerBI table
            productsTable.PublishToPowerBI(true);
        }

        private static void Sample_Dataset_Refresh()
        {
            // AppID: the ID of the Azure AD Application
            string ApplicationID = ConfigurationManager.AppSettings["PBI_ApplicationID"];

            PBIAPIClient powerBIClient = new PBIAPIClient(ApplicationID);

            PBIDataset powerBIDataset = powerBIClient.GetDatasetByName("TestRefresh");
            powerBIDataset.Refresh();
        }
    }
}



            
            

            