using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal class Publisher
    {
        private Guid _iD;
        private bool _isSelected;
        private string _uniqueName;
        private string _friendlyName;

        [DisplayName("ID")]
        public Guid ID
        {
            get 
            { 
                return _iD; 
            }
            set
            {
                _iD = value;
            }
        }

        [DisplayName("Select")]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        [DisplayName("Unique Name")]
        public string UniqueName
        {
            get
            {
                return _uniqueName;
            }
            set
            {
                _uniqueName = value;
            }
        }

        [DisplayName("Friendly Name")]
        public string FriendlyName
        {
            get
            {
                return _friendlyName;
            }
            set
            {
                _friendlyName = value;
            }
        }

        public Publisher(Entity publisher)
        {
            this._iD = publisher.GetAttributeValue<Guid>("publisherid");
            this._friendlyName = publisher.GetAttributeValue<string>("friendlyname");
            this._uniqueName = publisher.GetAttributeValue<string>("uniquename");
            this._isSelected = false;
        }
    }
}
