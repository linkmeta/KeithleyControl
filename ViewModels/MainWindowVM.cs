using KeithleyControl.Commands;
using KeithleyControl.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KeithleyControl.ViewModels
{
    internal class MainWindowVM : MainWindowBase, IDisposable
    {
        public string _LogInfo;
        public string LogInfo
        {
            get { return _LogInfo; }
            set
            {
                if (_LogInfo != value)
                {
                    _LogInfo = value;
                    OnPropertyChanged(nameof(LogInfo));
                }
            }
        }
        public void Log(string str)
        {
            LogInfo = str;
        }
        private Socket _socket;
        public SocketModel SocketModel { get; set; }
        public PowerSupplyModel PowerSupplyModel { get; set; }
        
        PlotModel _mPlotModelData = new PlotModel();
        public PlotModel PlotModelData
        {
            get { return _mPlotModelData; }
            set { _mPlotModelData = value; OnPropertyChanged(nameof(PlotModelData)); }
        }
        public DateTimeAxis XTimeAxis;

        public LinearAxis YCurrentVal;
        public LineSeries lineSeriesCurrentVal;
        public int YZoomFactor = 1;
        public void InitPlot()
        {
            PlotModelData = new PlotModel();

            PlotModelData.MouseDown += PlotModelData_MouseDown;
            XTimeAxis = new DateTimeAxis()
            {
                Title = "Time",
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.None,
                FontSize = 13,
                //MajorGridlineColor = OxyColor.FromArgb(40, 0, 0, 139),
                //MinorGridlineColor = OxyColor.FromArgb(20, 0, 0, 139),
                MinorGridlineStyle = LineStyle.Solid,

            };

            YCurrentVal = new LinearAxis()
            {
                Title = "Current(mA)",
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.None,
                Minimum = 0,
                AbsoluteMinimum = 0,

                Maximum = 1000.0,
                MajorStep = 100,
                FontSize = 13,
                PositionTier = 6,
                Key = "Current",
                
                //MajorGridlineColor = OxyColor.FromArgb(40, 10, 10, 139),
                //MinorGridlineColor = OxyColor.FromArgb(20, 5, 5, 139),
                MinorGridlineStyle = LineStyle.Solid,
                IsPanEnabled = false,
                IsZoomEnabled = false

                
            };

            PlotModelData.Axes.Add(XTimeAxis);
            PlotModelData.Axes.Add(YCurrentVal);


            lineSeriesCurrentVal = new OxyPlot.Series.LineSeries()
            {
                MarkerType = MarkerType.Circle,
                StrokeThickness = 1,
                YAxisKey = "Current",
                Title = "Current",
                Color = OxyColors.Red
            };

            PlotModelData.Series.Add(lineSeriesCurrentVal);
        }

        private void PlotModelData_MouseDown(object sender, OxyMouseDownEventArgs e)
        {

        }

        public void YZoomOut()
        {
            YCurrentVal.Maximum *= 2;
            YCurrentVal.MajorStep = (YCurrentVal.ActualMaximum - YCurrentVal.ActualMinimum)/10;
        }
        public void YZoomIn()
        {
            YCurrentVal.Maximum /= 2;
            YCurrentVal.MajorStep = (YCurrentVal.ActualMaximum - YCurrentVal.ActualMinimum) / 10;

        }
        public void ClearPlot()
        {
            lineSeriesCurrentVal.Points.Clear();
            PlotModelData.InvalidatePlot(true);
        }
        internal CancellationTokenSource plotTokenSource;
        internal CancellationToken cancelPlotToken;
        internal void StopUpdatePlotTask()
        {
            plotTokenSource.Cancel();
        }
        public void UpdatePlotTask()
        {
            double val;
            plotTokenSource = new CancellationTokenSource();
            cancelPlotToken = plotTokenSource.Token;
            Task.Run(
                () =>
                {
                    while (true)
                    {
                        if (plotTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        val = Convert.ToDouble(PowerSupplyModel.CurrentVal);
                        var date = DateTime.Now;
                        lineSeriesCurrentVal.Points.Add(DateTimeAxis.CreateDataPoint(date, val));

                        PlotModelData.InvalidatePlot(true);

                        if (date.ToOADate() > XTimeAxis.ActualMaximum)
                        {
                            var xPan = (XTimeAxis.ActualMaximum - XTimeAxis.DataMaximum) * XTimeAxis.Scale;
                            XTimeAxis.Pan(xPan);
                        }
                        Thread.Sleep(1000);
                    }
                }, cancelPlotToken);

        }

        private async void SocketRecvTask()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] bytes = new byte[128];
                        int length = _socket.Receive(bytes);
                        string msg = Encoding.UTF8.GetString(bytes, 0, length);
                        MessageBox.Show(msg, "Notify");
                    }
                    catch (Exception ex)
                    {
                        //ShowMsg("接收数据异常：" + er.ToString());
                        //break;
                        MessageBox.Show(ex.ToString(), "Notify");
                    }
                }
            });
        }
        internal void Connect()
        {
            try
            {
                if (_socket == null)
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(SocketModel.IpAddr, SocketModel.Port);
 
                    SocketModel.ConnectFlag = false;
                    SocketModel.DisConnectFlag = true;
                    PowerSupplyModel.Output = true;
                    Log("Connected OK!");

                }
            }
            catch (Exception ex)
            {
                _socket = null;

                Log("Connected fail!");
            }
        }
        internal void Disconnect()
        {
            try
            {
                _socket?.Close();
                _socket?.Dispose();
                _socket = null;
                SocketModel.DisConnectFlag = false;
                SocketModel.ConnectFlag = true;
                PowerSupplyModel.Output = false;
                Log("Disconnected OK!");
            }
            catch (Exception ex)
            {
                //Log("Disconnect error!");
            }

        }

        internal void Send(string msg)
        {
            try
            {
                if (_socket != null && !string.IsNullOrEmpty(msg))
                {
                    _socket.Send(Encoding.UTF8.GetBytes(msg));
                }
            }
            catch (Exception er)
            {

                Log("Send err!");

            }
        }
        internal string Recv(int size)
        {

            try
            {
                if (_socket != null && size > 0)
                {
                    byte[] bytes = new byte[size];
                    int length = _socket.Receive(bytes);
                    string msg = Encoding.UTF8.GetString(bytes, 0, length);
                    return msg;
                }
                return null;
            }
            catch (Exception er)
            {
                //ShowMsg("发送数据异常：" + er.ToString());
                //MessageBox.Show(er.ToString(), "Notify");
                return null;

            }
        }
        //PowerSupply API
        internal void DoReset()
        {
            string cmd = "*RST\n";
            Send(cmd);
        }
        internal void QueryId()
        {
            string cmd = "*IDN?\n";
            Send(cmd);
            //Recv(100);
        }
        internal void SetFunction(int val)
        {

        }
        internal void SetVoltage(double val)
        {
            string cmd = "SOUR1:VOLT " + val.ToString() + "\n";
            Send(cmd);
        }
        internal string GetVoltage()
        {
            string cmd = "SOUR1:VOLT?\n";
            Send(cmd);
            return Recv(32);
        }
        internal void SetVoltageProtection(double val)
        {
            string cmd = "SOUR1:VOLT:PROT " + val.ToString() + "\n";
            Send(cmd);
        }
        internal void SetCurrent(double val)
        {
            string cmd = "SOUR1:CURR " + val.ToString() + "\n";
            Send(cmd);
        }

        internal string GetCurrent()
        {
            string cmd = "SOUR1:CURR?\n";
            Send(cmd);
            return Recv(32);
        }
        internal void SetCurrentProtection(double val)
        {
            string cmd = "SOUR1:CURR:PROT " + val.ToString() + "\n";
            Send(cmd);
        }
        private Decimal ChangeDataToD(string strData)
        {
            Decimal dData = 0.0M;
            if (strData.Contains("E"))
            {
                dData = Decimal.Parse(strData, System.Globalization.NumberStyles.Float);
            }
            return dData;
        }
        internal CancellationTokenSource currentTokenSource;
        internal CancellationToken cancelCurrentToken;
        internal void StopMeasureCurrent()
        {
            currentTokenSource.Cancel();
        }
        internal string MeasureCurrent()
        {
            string cmd = "MEAS:CURR?\n";
            //Send(cmd);
            string temp = "0";
            currentTokenSource = new CancellationTokenSource();
            cancelCurrentToken = currentTokenSource.Token;

            Task.Run(
                () =>
                {
                    while (true)
                    {
                        if (currentTokenSource.IsCancellationRequested)
                        {
                            //Console.WriteLine("Task canceled");
                            break;
                        }
                        Send(cmd);
                        temp = Recv(64);

                        if(temp!=null && !temp.Contains("KEITHLEY")&&temp.Contains("A")&& temp.Contains("V"))
                        {
                            string[] bArray = temp.Split(',');
                            string[] strA = bArray[0].Split('A');
                            double current = (double)ChangeDataToD(strA[0]) * 1000;
                            PowerSupplyModel.CurrentVal = Math.Round(current, 5).ToString("0.00000");
                            //PowerSupplyModel.CurrentVal = current.ToString();
                            string[] strV = bArray[1].Split('V');
                            double voltage = (double)ChangeDataToD(strV[0]);
                            PowerSupplyModel.VoltageVal = Math.Round(voltage, 5).ToString("0.00000");
                        }

                        Thread.Sleep(500);
                    }
                }, cancelCurrentToken);
            return temp;
        }
        internal string MeassureVoltage()
        {
            string cmd = "MEAS:VOLT?\n";
            Send(cmd);
            return Recv(32);
        }
        internal void SetDataFormat()
        {
        
            string cmd = "FORM: ELEM \"READ\"\n";
            Send(cmd);
        }
        internal void SetOutputState(int val)
        {
            string cmd;
            if (val == 0)
            {
                cmd = "OUTP:STAT OFF\n";
            }
            else
            {
                cmd = "OUTP:STAT ON\n";
            }
            Send(cmd);
        }
        internal string GetOutputState()
        {
            string cmd = "OUTP:STAT?\n";
            string state = "OFF";
            Send(cmd);
            
            try
            {
                state = Recv(16);
                if (Convert.ToInt16(state) == 0)
                {
                    state = "OFF";
                }
                else
                {
                    state = "ON";
                }
            }catch (Exception ex)
            {
                
            }
            return state;
        }
        internal void SetDisplayText(string val)
        {
            string cmd = "DISP:USER:TEXT \"" + val + "\"\n";
            Send(cmd);
        }

        internal void PowerSupplySet()
        {
            bool val = PowerSupplyModel.Checked;
            if(!val)
            {
                StopMeasureCurrent();
                StopUpdatePlotTask();
                SetOutputState(0);
                PowerSupplyModel.State = GetOutputState();

                SetDisplayText("Powered Off DUT...");
                Log("Power Supply OFF");
            }
            else
            {
                //DoReset();
                SetVoltage(Convert.ToDouble(PowerSupplyModel.SetVoltageSet));
                SetVoltageProtection(Convert.ToDouble(PowerSupplyModel.SetVoltageSet)+1);

                SetCurrent(Convert.ToDouble(PowerSupplyModel.SetCurrentMax));
                SetCurrentProtection(Convert.ToDouble(PowerSupplyModel.SetCurrentMax));

                //Thread.Sleep(100);
                PowerSupplyModel.GetVoltageSet = GetVoltage();
                //Thread.Sleep(100);
                PowerSupplyModel.GetCurrentMax = GetCurrent();
                SetDataFormat();
                SetOutputState(1);
                //Thread.Sleep(100);
                PowerSupplyModel.State = GetOutputState();
                Thread.Sleep(100);
                //PowerSupplyModel.VoltageVal = MeassureVoltage();
                MeasureCurrent();
                SetDisplayText("Powering On DUT...");
                //SocketRecvTask();
                UpdatePlotTask();
                Log("Power Supply ON");


            }
        }
        internal void CmdDebug()
        {
            Send(SocketModel.Command);
            SocketModel.Response = Recv(100);
        }

        public ICommand CmdDebugCommand
        {
            get
            {
                return new RelayCommand(CmdDebug);
            }
        }
        public ICommand ConnectCommand
        {
            get
            {
                return new RelayCommand(Connect);
            }
        }
        public ICommand DisconnectCommand
        {
            get
            {
                return new RelayCommand(Disconnect);
            }
        }
        public ICommand ClearCommand
        {
            get
            {
                return new RelayCommand(ClearPlot);
            }
        }
        public ICommand ZoomOutCommand
        {
            get
            {
                return new RelayCommand(YZoomOut);
            }
        }
        public ICommand ZoomInCommand
        {
            get
            {
                return new RelayCommand(YZoomIn);
            }
        }
        public ICommand PowerSetCommand
        {
            get
            {
                return new RelayCommand(PowerSupplySet);
            }
        }
        public MainWindowVM()
        {
            SocketModel = new SocketModel();
            PowerSupplyModel = new PowerSupplyModel();
            InitPlot();
            LogInfo = "qqnokia@gmail.com";

        }

        #region IDisposable Support
        private bool disposedValue = false;   /* 冗余检测 */

        /// <summary>
        /// 释放组件所使用的非托管资源，并且有选择的释放托管资源（可以看作是Dispose()的安全实现）
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            /* 检查是否已调用dispose */
            if (!disposedValue)
            {
                if (disposing)
                {
                    /* 释放托管资源（如果需要的话） */

                    _socket?.Close();
                    _socket?.Dispose();
                }

                /* 释放非托管资源（如果有的话） */

                disposedValue = true;   /* 处理完毕 */
            }
        }

        /// <summary>
        /// 实现IDisposable，释放组件所使用的所有资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);   /* this: MetaCom.ViewModels.MainWindowViewmodel */
        }
        #endregion
    }



}
