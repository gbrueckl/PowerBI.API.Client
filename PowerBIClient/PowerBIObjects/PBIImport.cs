using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Microsoft.PowerBI.Api.V2.Models;

namespace gbrueckl.PowerBI.API.PowerBIObjects
{
    public class PBIImport : Import, IPBIObject
    {
        #region Constructors
        #endregion
        #region Private Properties for Serialization
        [JsonIgnore]
        private List<PBIDataset> _datasets;
        [JsonIgnore]
        private List<PBIReport> _reports;
        #endregion

        #region Public Properties
        [JsonIgnore]
        public new IList<PBIDataset> Datasets
        {
            get
            {
                if (_datasets == null)
                {
                    _datasets = new List<PBIDataset>();

                    foreach(Dataset ds in base.Datasets)
                    {
                        _datasets.Add(new PBIDataset(ds, this));
                    }
                }
                return _datasets;
            }
        }

        public new IList<PBIReport> Reports
        {
            get
            {
                if (_reports == null)
                {
                    _reports = new List<PBIReport>();

                    foreach (Report rep in base.Reports)
                    {
                        _reports.Add(new PBIReport(rep, this));
                    }
                }
                return _reports;
            }
        }


        [JsonIgnore]
        public PBIAPIClient ParentPowerBIAPI { get; set; }

        [JsonIgnore]
        public PBIGroup ParentGroup { get; set; }
        [JsonIgnore]
        public string ApiURL
        {
            get
            {
                if (ParentGroup == null)
                    return string.Format("/v1.0/myorg/imports/{0}", Id);
                else
                    return string.Format("/v1.0/myorg/groups/{0}/imports/{1}", ParentGroup.Id, Id);
            }
        }
        [JsonIgnore]
        public IPBIObject ParentObject { get; set; }

        #endregion

        #region Public Functions
        
        #endregion 
    }
}
