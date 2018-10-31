# PowerBI.API.Client
A C# wrapper for the Power BI REST API which allows you to easily integrate the Power BI API into your C# application. All objects from PowerBI are represented as native C# objects including all properties, functions/methods, etc.
The Power BI REST API is very well documented here: https://docs.microsoft.com/en-us/rest/api/power-bi/
The wrapper includes most of the functionality that available via the REST API but due to the high frequency of updates this wrapper may be a bit slower to integrate the latest features.

# Supported features
- List Dashboards, Reports, Datasets, Groups
- Import PBIX files
- Refresh datases
- Update parameters
- Rebind/Clone reports
- Authenticate using username/password for CI/CD scenarios
- Authenticate using a prompt for interactive use cases

If you need a feature that is not listed above please file an issue here in this repository!

# Basic Sample
This is a sample that connects to Power BI by asking the user for credentials, gets a dataset by its name and triggers a refresh
```c#
static void Main(string[] args)
{
    string ApplicationID = ConfigurationManager.AppSettings["PBI_ApplicationID"];

    PBIAPIClient pbic = new PBIAPIClient(ApplicationID);
    // PBIAPIClient pbic = new PBIAPIClient(ApplicationID, "myUser@myDomain.com", "Pass@word01!"); // to avoid prompt

    PBIDataset dataset = pbic.GetDatasetByName("myDataset");

    dataset.Refresh();
}
```

# Setup
The setup is quite easy, clone the project and compile the project using Visual Studio. This project targets .NET Framework v4.5!

There are also some NuGet packages that are required:
- Microsoft.PowerBI.Api
- Microsoft.IdentityModel.Clients.ActiveDirectory

You can either install them manually or via the NuGet Package Manager Console using the following commands:
- Install-Package Microsoft.PowerBI.Api
- Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory

