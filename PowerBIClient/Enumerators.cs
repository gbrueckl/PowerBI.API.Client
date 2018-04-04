using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gbrueckl.PowerBI.API
{
    public enum PBIAPI
    {
        DataSets,
        Reports,
        Groups,
        Dashboards,
        Tiles,
        Tables,
        Datasources,
        Rows
    }

    // Most enumerators were extracted from here: https://powerbi.microsoft.com/en-us/blog/newdatasets/

    public enum PBIDataType
    {
        Int64,
        Boolean,
        String,
        DateTime,
        Double,
        Decimal
    }

    public enum PBIDefaultRetentionPolicy
    {
        None,
        BasicFIFO
    }

    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.powerbi.api.v1.models.dataset#Microsoft_PowerBI_Api_V1_Models_Dataset_DefaultMode
    public enum PBIDefaultMode
    {
        AsAzure,
        AsOnPrem,
        Push,
        Streaming,
        PushStreaming
    }

    public enum PBICrossFilteringBehavior
    {
        OneDirection,
        BothDirection,
        Automatic
    }

    public enum PBIDataCategory
    {
        Address,
        City,
        Continent,
        Country,
        Image,
        ImageUrl,
        Latitude,
        Longitude,
        Organization,
        Place,
        PostalCode,
        StateOrProvince,
        WebUrl
    }

    public enum PBISummarizeBy
    {
        Sum,
        Min,
        Max,
        Count,
        None,
        Average,
        DistinctCount
    }
}
