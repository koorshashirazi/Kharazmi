namespace Kharazmi.AspNetCore.Core.Test.Settings
{
    public class SinglePageOption
    {
        private bool _displayMenu;
        private bool _displayMenuCreateProcess;
        private bool _displayMenuLanguage;
        private bool _displayMenuDelete;

        private bool _displayLeftPanel;
        private bool _displayPrimaryRightPanel;
        private bool _displaySecondRightPanel;
        private bool _displayMenuJobServer;

        public bool SinglePageEnable { get; set; }

        public string ApiResourceUrl { get; set; }

        public bool DisplayMenu
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayMenu;
                return true;
            }
            set => _displayMenu = value;
        }

        public bool DisplayMenuCreateProcess
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayMenu && _displayMenuCreateProcess;
                return true;
            }
            set => _displayMenuCreateProcess = value;
        }

        public bool DisplayMenuLanguage
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayMenu && _displayMenuLanguage;
                return true;
            }
            set => _displayMenuLanguage = value;
        }

        public bool DisplayMenuDelete
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayMenu && _displayMenuDelete;
                return true;
            }
            set => _displayMenuDelete = value;
        }

        public bool DisplayMenuJobServer
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayMenu && _displayMenuJobServer;
                return true;
            }
            set => _displayMenuJobServer = value;
        }

        public bool DisplayLeftPanel
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayLeftPanel;
                return true;
            }
            set => _displayLeftPanel = value;
        }

        public bool DisplayPrimaryRightPanel
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayLeftPanel && _displayPrimaryRightPanel;
                return true;
            }
            set => _displayPrimaryRightPanel = value;
        }

        public bool DisplaySecondRightPanel
        {
            get
            {
                if (SinglePageEnable)
                    return SinglePageEnable && _displayLeftPanel && _displaySecondRightPanel;
                return true;
            }
            set => _displaySecondRightPanel = value;
        }
    }
}