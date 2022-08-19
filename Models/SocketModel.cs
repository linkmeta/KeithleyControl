using KeithleyControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithleyControl.Models
{
    internal class SocketModel : MainWindowBase
    {
        //public string[] Interface { get; set; }
        private string _IpAddr;
        public string IpAddr
        {
            get{ return _IpAddr; }
            set
            {
                if (_IpAddr != value)
                {
                    _IpAddr = value;
                    OnPropertyChanged(nameof(IpAddr));
                }
            }
        }
        private int _Port;
        public int Port
        {
            get{ return _Port; }
            set
            {
                if (_Port != value)
                {
                    _Port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        private int _Timeout;
        public int Timeout
        {
            get{ return _Timeout;}
            set
            {
                if (_Timeout != value)
                {
                    _Timeout = value;
                    OnPropertyChanged(nameof(Timeout));
                }
            }
        }

        private bool _ConnectFlag;
        public bool ConnectFlag
        {
            get{ return _ConnectFlag; }
            set
            {
                if (_ConnectFlag != value)
                {
                    _ConnectFlag = value;
                    OnPropertyChanged(nameof(ConnectFlag));
                }
            }
        }
        private bool _DisConnectFlag;
        public bool DisConnectFlag
        {
            get{ return _DisConnectFlag; }
            set
            {
                if (_DisConnectFlag != value)
                {
                    _DisConnectFlag = value;
                    OnPropertyChanged(nameof(DisConnectFlag));
                }
            }
        }
        private string _Command;
        public string Command
        {
            get { return _Command; }
            set
            {
                if (_Command != value)
                {
                    _Command = value;
                    OnPropertyChanged(nameof(Command));
                }
            }
        }
        private string _Response;
        public string Response
        {
            get { return _Response; }
            set
            {
                if (_Response != value)
                {
                    _Response = value;
                    OnPropertyChanged(nameof(Response));
                }
            }
        }
        public SocketModel()
        {
            IpAddr = "192.168.1.2";
            Port = 5025;
            Timeout = 60;
            ConnectFlag = true;
            DisConnectFlag = false;
            Command = "*IDN?\n";

        }
    }
}
