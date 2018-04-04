using gbrueckl.PowerBI.API.PowerBIObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API
{
    public static class Helpers
    {
        public static string StreamToString(this Stream stream)
        {
            
            using (StreamReader reader = new StreamReader(stream))
            {
                string text = reader.ReadToEnd();
                return text;
            }
        }

        public static string ResponseToString(this HttpWebResponse webResponse)
        {
            using (Stream data = webResponse.GetResponseStream())
            {
                return data.StreamToString();
            }
        }

        public static void AddOrUpdateTable(this PBIDataset dataset, PBITable newTable)
        {
            for(int i = 0; i < dataset.Tables.Count; i++)
            {
                if(dataset.Tables[i].Name == newTable.Name)
                {
                    dataset.Tables[i].Columns = newTable.Columns;
                    dataset.Tables[i].Measures = newTable.Measures;
                    dataset.Tables[i].IsHidden = newTable.IsHidden;

                    return;
                }
            }

            newTable.ParentDataset = dataset;
            newTable.ParentObject = dataset;
            newTable.ParentGroup = dataset.ParentGroup;
            dataset.Tables.Add(newTable);
        }

        public static PBIDataType ToPBIDataType(this Type dataType)
        {
            switch(dataType.Name)
            {
                case "Boolean": return PBIDataType.Boolean;
                case "Byte": return PBIDataType.Int64;
                case "Char": return PBIDataType.String;
                case "DateTime": return PBIDataType.DateTime;
                case "Decimal": return PBIDataType.Double;
                case "Double": return PBIDataType.Double;
                case "Guid": return PBIDataType.String;
                case "Int16": return PBIDataType.Int64;
                case "Int32": return PBIDataType.Int64;
                case "Int64": return PBIDataType.Int64;
                case "SByte": return PBIDataType.Int64;
                case "Single": return PBIDataType.Int64;
                case "String": return PBIDataType.String;
                case "UInt16": return PBIDataType.Int64;
                case "UInt32": return PBIDataType.Int64;
                case "UInt64": return PBIDataType.Int64;
                default: throw new NotSupportedException(string.Format("Datatype '{0}' is not supported in Power BI!", dataType.Name));
            }
        }

        public static List<PBIRow> PBIRows(this DataTable dataTable)
        {
            List<PBIRow> rows = new List<PBIRow>(dataTable.Rows.Count);
            Dictionary<string, object> values;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                values = Enumerable.Range(0, dataTable.Columns.Count).ToDictionary(i => dataTable.Columns[i].ColumnName, i => dataRow.ItemArray[i]);
                rows.Add(new PBIRow(values));
            }

            return rows;
        }

        public static List<PBIColumn> PBIColumns(this DataTable dataTable)
        {
            List<PBIColumn> cols = new List<PBIColumn>(dataTable.Columns.Count);

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                cols.Add(new PBIColumn(dataColumn.ColumnName, dataColumn.DataType.ToPBIDataType()));
            }

            return cols;
        }
    }
}
