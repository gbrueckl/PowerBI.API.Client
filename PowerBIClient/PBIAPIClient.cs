using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using gbrueckl.PowerBI.API.PowerBIObjects;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace gbrueckl.PowerBI.API
{
    public class PBIAPIClient : IPBIObject
    {
        //Replace redirectUri with the uri you used when you registered your app
        public string RedirectUri = "https://login.live.com/oauth20_desktop.srf";

        //Power BI resource uri
        public const string ResourceUri = "https://analysis.windows.net/powerbi/api";
        //OAuth2 authority
        public const string AuthorityUri = "https://login.windows.net/common/oauth2/authorize";

        //Base API URL
        private const string PBIApiUrl = "https://api.powerbi.com";

        private string _clientID;
        private AuthenticationContext _authenticationContext = null;
        private string _accessToken = string.Empty;
        private string _tenantId = string.Empty;

        NumberStyles _style;
        CultureInfo _culture;

        #region Constructors
        public PBIAPIClient(string clientID)
        {
            _clientID = clientID;

            _style = NumberStyles.Number;
            _culture = CultureInfo.CreateSpecificCulture("en-US");

            Connect();
        }

        public PBIAPIClient(string clientID, string accessToken)
        {
            _clientID = clientID;
            _accessToken = accessToken;

            _style = NumberStyles.Number;
            _culture = CultureInfo.CreateSpecificCulture("en-US");
        }

        public PBIAPIClient(string clientID, string userName, string password)
        {
            _clientID = clientID;

            _style = NumberStyles.Number;
            _culture = CultureInfo.CreateSpecificCulture("en-US");

            Connect(userName, password);
        }
        #endregion

        #region Security/Authentication functions
        public void Connect(string username, string password)
        {
            //http://blogs.msdn.com/b/tomholl/archive/2014/11/25/unattended-authentication-to-azure-management-apis-with-azure-active-directory.aspx
            UserPasswordCredential upc = new UserPasswordCredential(username, password);

            if (_accessToken == String.Empty)
            {
                TokenCache TC = new TokenCache();
                _authenticationContext = new AuthenticationContext(AuthorityUri, TC);
                var asyncCall = _authenticationContext.AcquireTokenAsync(ResourceUri, _clientID, upc);
                _tenantId = asyncCall.Result.TenantId;
                _accessToken = asyncCall.Result.AccessToken.ToString();
            }
            else
            {
                var asyncCall = _authenticationContext.AcquireTokenSilentAsync(ResourceUri, _clientID);
                _accessToken = asyncCall.Result.AccessToken;
            }
        }

        public void Connect()
        {
            //Get access token: 
            // To call a Power BI REST operation, create an instance of AuthenticationContext and call AcquireToken
            // AuthenticationContext is part of the Active Directory Authentication Library NuGet package
            if (_accessToken == String.Empty)
            {
                TokenCache TC = new TokenCache();
                _authenticationContext = new AuthenticationContext(AuthorityUri, TC);
                var asyncCall = _authenticationContext.AcquireTokenAsync(ResourceUri, _clientID, new Uri(RedirectUri), new PlatformParameters(PromptBehavior.Auto));
                _tenantId = asyncCall.Result.TenantId;
                _accessToken = asyncCall.Result.AccessToken;
            }
            else
            {
                var asyncCall = _authenticationContext.AcquireTokenSilentAsync(ResourceUri, _clientID);
                _accessToken = asyncCall.Result.AccessToken;
            }
        }

        private string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(_accessToken))
                    Connect();
                return _accessToken;
            }
            set
            {
                _accessToken = value;
            }
        }

        public string TenantId
        {
            get
            {
                if (string.IsNullOrEmpty(_tenantId))
                    Connect();
                return _tenantId;
            }
            set
            {
                _tenantId = value;
            }
        }
        #endregion 

        #region Public Properties
        public List<PBIDataset> Datasets
        {
            get
            {
                PBIObjectList<PBIDataset> objList = JsonConvert.DeserializeObject<PBIObjectList<PBIDataset>>(SendGETRequest(ApiURL, PBIAPI.DataSets).ResponseToString());

                foreach (var item in objList.Items)
                {
                    item.ParentPowerBIAPI = this;
                    item.ParentGroup = null;
                    item.ParentObject = this;
                }

                return objList.Items;
            }
        }



        public List<PBIDashboard> Dashboards
        {
            get
            {
                PBIObjectList<PBIDashboard> objList = JsonConvert.DeserializeObject<PBIObjectList<PBIDashboard>>(SendGETRequest(ApiURL, PBIAPI.Dashboards).ResponseToString());

                foreach (var item in objList.Items)
                {
                    item.ParentPowerBIAPI = this;
                    item.ParentGroup = null;
                    item.ParentObject = this;
                }

                return objList.Items;
            }
        }

        public List<PBIReport> Reports
        {
            get
            {
                PBIObjectList<PBIReport> objList = JsonConvert.DeserializeObject<PBIObjectList<PBIReport>>(SendGETRequest(ApiURL, PBIAPI.Reports).ResponseToString());

                foreach (var item in objList.Items)
                {
                    item.ParentPowerBIAPI = this;
                    item.ParentGroup = null;
                    item.ParentObject = this;
                }

                return objList.Items;
            }
        }

        public List<PBIGroup> Groups
        {
            get
            {
               PBIObjectList<PBIGroup> objList = JsonConvert.DeserializeObject<PBIObjectList<PBIGroup>>(SendGETRequest(ApiURL, PBIAPI.Groups).ResponseToString());

                foreach (var item in objList.Items)
                {
                    item.ParentPowerBIAPI = this;
                    item.ParentGroup = null;
                    item.ParentObject = this;
                }

                return objList.Items;
            }
        }

        [JsonIgnore]
        public string Id { get => null; set { } }
        [JsonIgnore]
        public string ApiURL { get => "/v1.0/myorg"; }
        [JsonIgnore]
        public PBIGroup ParentGroup { get => null;}
        [JsonIgnore]
        public IPBIObject ParentObject { get => null; }
        #endregion

        #region Public Functions
        public PBIDataset GetDatasetByID(string id)
        {
            try
            {
                return Datasets.Single(x => string.Equals(x.Id, id, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Dataset with ID '{0}' could be found in PowerBI!", id), e);
            }
        }

        public PBIDataset GetDatasetByName(string name)
        {
            try
            {
                return Datasets.Single(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Dataset with name '{0}' could be found in PowerBI!", name), e);
            }
        }

        public PBIReport GetReportByName(string name)
        {
            try
            {
                return Reports.Single(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Report with name '{0}' could be found in PowerBI!", name), e);
            }
        }

        public PBIDashboard GetDashboardByName(string name)
        {
            try
            {
                return Dashboards.Single(x => string.Equals(x.DisplayName, name, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Dashboard with name '{0}' could be found in PowerBI!", name), e);
            }
        }

        public PBIGroup GetGroupByName(string name)
        {
            try
            {
                return Groups.Single(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Group/Workspace with name '{0}' could be found in PowerBI!", name), e);
            }
        }

        public PBIGroup GetGroupById(string id)
        {
            try
            {
                return Groups.Single(x => string.Equals(x.Id, id, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Group/Workspace with ID '{0}' could be found in PowerBI!", id), e);
            }
        }
        #endregion
        public HttpWebResponse SendGenericWebRequest(string url, string method, string body = null)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = System.Net.WebRequest.Create(url) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = method.ToUpper();
            request.ContentLength = 0;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken));

            if (body != null)
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = byteArray.Length;

                //Write JSON byte[] into a Stream
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
            }

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                using (WebResponse webResponse = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)webResponse;
                    using (Stream data = webResponse.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text, e);
                    }
                }
            }
            return response;
        }


        #region Private WebRequest Methods
        private HttpWebResponse SendApIRequest(string api, string method, string body = null)
        {
            HttpWebResponse response = null;

            HttpWebRequest request = System.Net.WebRequest.Create(string.Format("{0}{1}", PBIApiUrl, api)) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = method.ToUpper();
            request.ContentLength = 0;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken));

            if (body != null)
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = byteArray.Length;

                //Write JSON byte[] into a Stream
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
            }
            
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                using (WebResponse webResponse = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)webResponse;
                    using (Stream data = webResponse.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        throw new Exception(text, e);
                    }
                }
            }
            return response;
        }
        private async Task<HttpWebResponse> SendWebRequestAsync(string api, string method, string body = null)
        {
            HttpWebRequest request = System.Net.WebRequest.Create(string.Format("{0}{1}", PBIApiUrl, api)) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = method.ToUpper();
            request.ContentLength = 0;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken));

            if (body != null)
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = byteArray.Length;

                //Write JSON byte[] into a Stream
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
            }
            return (HttpWebResponse)await request.GetResponseAsync();

        }
        #endregion

        #region POST Requests
        public HttpWebResponse SendPOSTRequest(string api, string json)
        {
            return SendApIRequest(api, "POST", json);
        }
        public HttpWebResponse SendPOSTRequest(PBIAPI api, string json)
        {
            return SendPOSTRequest(api.ToString().ToLower(), json);
        }
        public HttpWebResponse SendPOSTRequest(string apiUrl, PBIAPI api, string json)
        {
            return SendPOSTRequest(apiUrl + "/" + api.ToString().ToLower(), json);
        }
        public async Task<HttpWebResponse> SendPOSTRequestAsync(string api, string json)
        {
            return await SendWebRequestAsync(api, "POST", json);
        }
        public async Task<HttpWebResponse> SendPOSTRequestAsync(PBIAPI api, string json)
        {
            return await SendWebRequestAsync(api.ToString().ToLower(), "POST", json);
        }
        public Task<HttpWebResponse> SendPOSTRequestAsync(string apiUrl, PBIAPI api, string json)
        {
            return SendWebRequestAsync(apiUrl + "/" + api.ToString().ToLower(), "POST", json);
        }
        #endregion

        #region GET Requests
        public HttpWebResponse SendGETRequest(string api)
        {
            return SendApIRequest(api, "GET");
        }
        public HttpWebResponse SendGETRequest(PBIAPI api)
        {
            return SendGETRequest(api.ToString().ToLower());
        }

        public HttpWebResponse SendGETRequest(string apiUrl, PBIAPI api)
        {
            return SendGETRequest(apiUrl + "/" + api.ToString().ToLower());
        }
        public async Task<HttpWebResponse> SendGETRequestAsync(string api)
        {
            return await SendWebRequestAsync(api, "GET");
        }
        public async Task<HttpWebResponse> SendGETRequestAsync(PBIAPI api)
        {
            return await SendWebRequestAsync(api.ToString().ToLower(), "GET");
        }
        #endregion

        #region PUT Requests
        public HttpWebResponse SendPUTRequest(string api, string json)
        {
            return SendApIRequest(api, "PUT", json);
        }
        public async Task<HttpWebResponse> SendPUTRequestAsync(string api, string json)
        {
            return await SendWebRequestAsync(api, "PUT", json);
        }
        #endregion

        #region DELETE Requests
        public HttpWebResponse SendDELETERequest(string api)
        {
            return SendApIRequest(api, "DELETE");
        }
        public HttpWebResponse SendDELETERequest(string apiUrl, PBIAPI api)
        {
            return SendApIRequest(apiUrl + "/" + api.ToString().ToLower(), "DELETE");
        }
        public async Task<HttpWebResponse> SendDELETERequestAsync(string api)
        {
            return await SendWebRequestAsync(api, "DELETE");
        }
        #endregion
    }

    
}
