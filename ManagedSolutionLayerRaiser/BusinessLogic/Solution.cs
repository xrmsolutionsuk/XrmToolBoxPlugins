﻿using Microsoft.Xrm.Sdk;
using System;
using System.ComponentModel;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal class Solution
    {
        private Guid _iD;
        private bool _isSelected;
        private string _uniqueName;
        private string _friendlyName;
        private string _version;
        private string _publisherUniqueName;
        private string _publisherFriendlyName;
        private DateTime _installedOn;
        private DateTime _uppdatedOn;

        [DisplayName("Solution ID")]
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

        [DisplayName("Name")]
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

        [DisplayName("Display Name")]
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

        [DisplayName("Version")]
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        [DisplayName("Publisher Unique Name")]
        public string PublisherUniqueName
        {
            get
            {
                return _publisherUniqueName;
            }
            set
            {
                _publisherUniqueName = value;
            }
        }

        [DisplayName("Publisher Friendly Name")]
        public string PublisherFriendlyName
        {
            get
            {
                return _publisherFriendlyName;
            }
            set
            {
                _publisherFriendlyName = value;
            }
        }

        [DisplayName("Installed On")]
        public DateTime InstalledOn
        {
            get
            {
                return _installedOn;
            }
            set
            {
                _installedOn = value;
            }
        }

        [DisplayName("Updated On")]
        public DateTime UpdatedOn
        {
            get
            {
                return _uppdatedOn;
            }
            set
            {
                _uppdatedOn = value;
            }
        }

        public Solution(Entity solutionRecord)
        {
            this.ID = (Guid)solutionRecord.GetAttributeValue<Guid>("solutionid");
            this.IsSelected = false;
            this.UniqueName = solutionRecord.GetAttributeValue<string>("uniquename");
            this.FriendlyName = solutionRecord.GetAttributeValue<string>("friendlyname");
            this.PublisherUniqueName = solutionRecord.GetAttributeValue<AliasedValue>("publisher.uniquename").Value.ToString();
            this.PublisherFriendlyName = solutionRecord.GetAttributeValue<AliasedValue>("publisher.friendlyname").Value.ToString();
            this.Version = solutionRecord.GetAttributeValue<string>("version");
            this.InstalledOn = solutionRecord.GetAttributeValue<DateTime>("installedon");
            this.UpdatedOn = solutionRecord.GetAttributeValue<DateTime>("updatedon");
        }
    }
}
