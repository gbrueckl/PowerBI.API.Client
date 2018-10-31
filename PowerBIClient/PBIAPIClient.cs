using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using gbrueckl.PowerBI.API.PowerBIObjects;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Newtonsoft.Json;

namespace gbrueckl.PowerBI.API
{
    public class PBIAPIClient : PBIGroup, IPBIObject
    {
        //Replace redirectUri with the uri you used when you registered your app
        public string RedirectUri = "https://login.live.com/oauth20_desktop.srf";

        //Power BI resource uri
        public const string ResourceUri = "https://analysis.windows.net/powerbi/api";
        //OAuth2 authority
        //public const string AuthorityUri = "https://login.windows.net/common/oauth2/authorize";
        public const string AuthorityUri = "https://login.microsoftonline.com/common/oauth2/authorize";

        //Base API URL
        public const string PBIApiRootUrl = "https://api.powerbi.com";

        private string _clientID;
        private AuthenticationContext _authenticationContext = null;
        private string _accessToken = string.Empty;
        private string _tenantId = string.Empty;

        NumberStyles _style;
        CultureInfo _culture;

        #region Constructors

        private void Initialize()
        {
            base.ParentPowerBIAPI = this;
            base.ApiURL = "/v1.0/myorg";

            _style = NumberStyles.Number;
            _culture = CultureInfo.CreateSpecificCulture("en-US");
        }
        public PBIAPIClient(string clientID)
        {
            _clientID = clientID;

            Initialize();
            
            Connect();
        }

        public PBIAPIClient(string clientID, string accessToken)
        {
            _clientID = clientID;
            _accessToken = accessToken;

            Initialize();
        }

        public PBIAPIClient(string clientID, string userName, string password)
        {
            _clientID = clientID;

            Initialize();

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
                var asyncCall = _authenticationContext.AcquireTokenAsync(ResourceUri, _clientID, new Uri(RedirectUri), new PlatformParameters(PromptBehavior.Always));
                _tenantId = asyncCall.Result.TenantId;
                _accessToken = asyncCall.Result.AccessToken;
            }
            else
            {
                var asyncCall = _authenticationContext.AcquireTokenSilentAsync(ResourceUri, _clientID);
                _accessToken = asyncCall.Result.AccessToken;
            }
        }

        public string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(_accessToken))
                    Connect();
                return _accessToken;
            }
            private set
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
        #endregion


        [JsonIgnore]
        public Dictionary<string,List<string>> RequestHeaders
        {
            get
            {
                Dictionary < string,List <string>> headers = new Dictionary<string, List<string>>();
                List<string> authHeader = new List<string>();

                authHeader.Add(String.Format("Bearer {0}", AccessToken));
                headers.Add("Authorization", authHeader);

                return headers;
            }
        }

        #region Public Functions
        public PBIGroup GetGroupByID(string id)
        {
            try
            {
                return Groups.Single(x => string.Equals(x.Id, id, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception e)
            {
                //return null;
                throw new KeyNotFoundException(string.Format("No Group with ID '{0}' could be found in PowerBI!", id), e);
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
                throw new KeyNotFoundException(string.Format("No Group with name '{0}' could be found in PowerBI!", name), e);
            }
        }

        public PBIGroup CreateGroup(string name)
        {
            try
            {
                PBIGroup ret;
                using (HttpWebResponse response = this.SendPOSTRequest(ApiURL, PBIAPI.Groups, "{\"name\":\"" + name + "\"}"))
                {
                    string result = response.ResponseToString();
                    ret = JsonConvert.DeserializeObject<PBIGroup>(result);

                    ret.ParentPowerBIAPI = this;
                    ret.ParentGroup = null;
                    ret.ParentObject = this;
                }
                return ret;
            }
            catch (Exception e)
            {
                //return null;
                throw e;
            }
        }
        public HttpResponseMessage SendPOSTStream(string url, StreamContent content)
        {
            MultipartFormDataContent requestBody = new MultipartFormDataContent(Guid.NewGuid().ToString());
            requestBody.Add(content);
            // create and configure HttpClient
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ParentPowerBIAPI.AccessToken);
            // post request
            var response = client.PostAsync(url, requestBody).Result;
            // check for success

            return response;
        }
        public HttpWebResponse SendGenericWebRequest(string url, string method, string body = null)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = System.Net.WebRequest.Create(url) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = method.ToUpper();
            request.ContentLength = 0;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken));

            if (!string.IsNullOrEmpty(body))
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
        #endregion

        #region Private WebRequest Methods
        private HttpWebResponse SendApIRequest(string api, string method, string body = null)
        {
            HttpWebResponse response = null;

            HttpWebRequest request = System.Net.WebRequest.Create(string.Format("{0}{1}", PBIApiRootUrl, api)) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = method.ToUpper();
            request.ContentLength = 0;
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken));

            if (!string.IsNullOrEmpty(body))
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
                    string text = httpResponse.ResponseToString();

                    throw new Exception(text, e);
                }
            }
            return response;
        }
        private async Task<HttpWebResponse> SendWebRequestAsync(string api, string method, string body = null)
        {
            HttpWebRequest request = System.Net.WebRequest.Create(string.Format("{0}{1}", PBIApiRootUrl, api)) as System.Net.HttpWebRequest;
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
        public HttpWebResponse SendPOSTRequest(string api, string body)
        {
            return SendApIRequest(api, "POST", body);
        }

        public HttpResponseMessage SendPOSTRequest(string api, Stream content, Dictionary<string, string> contentHeaders = null)
        {
            StreamContent streamContent = new StreamContent(content);
            if (contentHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in contentHeaders)
                {
                    streamContent.Headers.Add(header.Key, header.Value);
                }
            }
            return SendPOSTStream(PBIApiRootUrl + api, streamContent);
        }
        public HttpWebResponse SendPOSTRequest(PBIAPI api, string body)
        {
            return SendPOSTRequest(api.ToString().ToLower(), body);
        }
        public HttpWebResponse SendPOSTRequest(string apiUrl, PBIAPI api, string body)
        {
            return SendPOSTRequest(apiUrl + "/" + api.ToString().ToLower(), body);
        }
        public async Task<HttpWebResponse> SendPOSTRequestAsync(string api, string json)
        {
            return await SendWebRequestAsync(api, "POST", json);
        }
        /*
        public async Task<HttpWebResponse> SendPOSTRequestAsync(string api, Stream content)
        {
            throw new NotImplementedException("Not yet implemented!");

            var pbixBodyContent = new StreamContent(File.Open(pbixPath, FileMode.Open));
            // add headers for request bod content
            pbixBodyContent.Headers.Add("Content-Type", "application/octet-stream");
            pbixBodyContent.Headers.Add("Content-Disposition",
                                         @"form-data; name=""file""; filename=""" + pbixFilePath + @"""");
            // load PBIX content into body using multi-part form data
            MultipartFormDataContent requestBody = new MultipartFormDataContent(Guid.NewGuid().ToString());
            requestBody.Add(pbixBodyContent);
            // create and configure HttpClient
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
            // post request
            var response = client.PostAsync(restUrlImportPbix, requestBody).Result;
            return await SendApIRequestAsync(api, "POST", json);
            
        }
        */
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
        public HttpWebResponse SendDELETERequest(string apiUrl, PBIAPI api, params string[] parameters)
        {
            string dynamicParameters = "";
            if (parameters != null)
                dynamicParameters = "/" + string.Join("/", parameters);

            return SendApIRequest(apiUrl + "/" + api.ToString().ToLower() + dynamicParameters, "DELETE");
        }
        public async Task<HttpWebResponse> SendDELETERequestAsync(string api)
        {
            return await SendWebRequestAsync(api, "DELETE");
        }
        #endregion
    }

    
}
