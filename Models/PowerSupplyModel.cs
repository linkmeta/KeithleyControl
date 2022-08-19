using KeithleyControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithleyControl.Models
{
    internal class PowerSupplyModel : MainWindowBase
    {
        private string _VoltageVal;
        public string VoltageVal
        {
            get{ return _VoltageVal; }
            set
            {
                if (_VoltageVal != value)
                {
                    _VoltageVal = value;
                    OnPropertyChanged(nameof(VoltageVal));
                }
            }
        }
        private string _CurrentVal;
        public string CurrentVal
        {
            get{ return _CurrentVal; }
            set
            {
                if (_CurrentVal != value)
                {
                    _CurrentVal = value;
                    OnPropertyChanged(nameof(CurrentVal));
                }
            }
        }

        private string _GetVoltageSet;
        public string GetVoltageSet
        {
            get{ return _GetVoltageSet; }
            set
            {
                if (_GetVoltageSet != value)
                {
                    _GetVoltageSet = value;
                    OnPropertyChanged(nameof(GetVoltageSet));
                }
            }
        }
        private string _GetCurrentMax;
        public string GetCurrentMax
        {
            get{ return _GetCurrentMax;}
            set{
                if (_GetCurrentMax != value)
                {
                    _GetCurrentMax = value;
                    OnPropertyChanged(nameof(GetCurrentMax));
                }
            }
        }
        private string _SetVoltageSet;
        public string SetVoltageSet
        {
            get { return _SetVoltageSet; }
            set
            {
                if (_SetVoltageSet != value)
                {
                    _SetVoltageSet = value;
                    OnPropertyChanged(nameof(SetVoltageSet));
                }
            }
        }
        private string _SetCurrentMax;
        public string SetCurrentMax
        {
            get { return _SetCurrentMax; }
            set
            {
                if (_SetCurrentMax != value)
                {
                    _SetCurrentMax = value;
                    OnPropertyChanged(nameof(SetCurrentMax));
                }
            }
        }
        private string _State;
        public string State
        {
            get{ return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        private bool _Output;
        public bool Output
        {
            get { return _Output; }
            set
            {
                if (_Output != value)
                {
                    _Output = value;
                    OnPropertyChanged(nameof(Output));
                }
            }
        }
        private bool _Checked;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked != value)
                {
                    _Checked = value;
                    OnPropertyChanged(nameof(Checked));
                }
            }
        }
        private string _Interface;
        public string Interface
        {
            get { return _Interface; }
            set
            {
                if (_Interface != value)
                {
                    _Interface = value;
                    OnPropertyChanged(nameof(Interface));
                }
            }
        }
        public PowerSupplyModel()
        {
            VoltageVal = "0.0000";
            CurrentVal = "0.0000";
            GetVoltageSet = "0.0000";
            GetCurrentMax = "0.0000";
            State = "OFF";
            SetVoltageSet = "4.000";
            SetCurrentMax = "3.000";
            Output = false;
            Checked = false;
            Interface = "LAN";
        }
    }
}
