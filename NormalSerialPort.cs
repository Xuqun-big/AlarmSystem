using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hans.MV.Communication.Delegate;
using Hans.MV.Communication.Enum;
using Hans.MV.Communication.Interface;
using Hans.MV.General;

namespace Hans.MV.Communication.Entity
{
    public class NormalSerialPort : ISerialPort
    {
        private CommunicationType communicationType = CommunicationType.SerialPort;
        private SerialPort port=new SerialPort();
        private bool keeplisten=false;
        private object locker = new object();
        private object locker1 = new object();
        public string Com 
        { 
            get {
                return port.PortName;
            }
            set {
                if (!port.IsOpen)
                {
                    port.PortName = value;
                }
                else
                {
                    port.Close();
                    port.PortName = value;
                    Open();
                }
            }
        }
        public SerialPortBaudRates BaudRate {
            get { return (SerialPortBaudRates)port.BaudRate; }
            set {
                if (!port.IsOpen)
                {
                    port.BaudRate = (int)value;
                }
                else
                {
                    port.Close();
                    port.BaudRate = (int)value;
                    Open();
                }
            }
        }
        public SerialPortDatabits DataBit {
            get { return (SerialPortDatabits)port.DataBits; }
            set
            {
                if (!port.IsOpen)
                {
                    port.DataBits = (int)value;
                }
                else
                {
                    port.Close();
                    port.DataBits = (int)value;
                    Open();
                }
            }
            }
        public Parity Parity {
            get { return port.Parity; }
            set {
                if (!port.IsOpen)
                {
                    port.Parity = value;
                }
                else
                {
                    port.Close();
                    port.Parity = value;
                    Open();
                }
            }
        }
        public StopBits StopBit {
            get { return port.StopBits; }
            set {
                if (!port.IsOpen)
                {
                    port.StopBits = value;
                }
                else
                {
                    port.Close();
                    port.StopBits = value;
                    Open();
                }
            }
        }

        public bool Opened {
            get { return port.IsOpen; }
        }

        public CommunicationType CommunicationType => communicationType;
        public event DateReceiveDelegate DataReceived;
        public event ConnectDelegate PortOpened;
        public event ConnectDelegate PortClosed;
        public event SendDelegate SendOver;

        public bool Close()
        {
            keeplisten = false;
            if (port.IsOpen)
                port.Close();
            if (port.IsOpen)
            {
                PortClosed?.Invoke(new General.SendResult()
                {
                    Sender = port,
                    IsSuccess = false
                });
            }
            else
            {
                PortClosed?.Invoke(new General.SendResult()
                {
                    Sender = port,
                    IsSuccess = true
                }) ;
            }
            return !port.IsOpen;
        }

        public void Listen(bool start = true)
        {
            if (start == keeplisten)
                return;
            keeplisten = start;
            if (start&&port.IsOpen)
            {
                byte[] s ;
                while (keeplisten && port.IsOpen)
                {
                    System.Threading.Thread.Sleep(1);
                    if (keeplisten && port.IsOpen&&port.BytesToRead > 0)
                    {
                        s = new byte[port.BytesToRead];
                        port.Read(s, 0, s.Length);
                        DataReceived?.Invoke(port,s);
                    }
                }
            }
            keeplisten = false;
        }

        public bool Open()
        {
            try
            {
                if (!port.IsOpen)
                    port.Open();
                if (port.IsOpen)
                {
                    PortOpened?.Invoke(new General.SendResult()
                    {
                        Sender = port,
                        IsSuccess = true
                    });
                }
                else
                {
                    PortOpened?.Invoke(new General.SendResult()
                    {
                        Sender = port,
                        IsSuccess = false
                    });
                }
            }
            catch(IOException ex) {
                PortOpened?.Invoke(new General.SendResult()
                {
                    Sender = port,
                    IsSuccess = false,
                    Reason=ex.Message
                });
            }
            return port.IsOpen;
        }
        
        public byte[] Receive(int timeout = 3000)
        {
            lock (locker)
            {
                byte[] s = null;
                if (port.IsOpen)
                {
                    DateTime t1 = DateTime.Now;
                    while (port.IsOpen)
                    {
                        System.Threading.Thread.Sleep(1);
                        if (port.IsOpen&&port.BytesToRead > 0)
                        {
                            s = new byte[port.BytesToRead];
                            port.Read(s, 0, s.Length);
                            break;
                        }
                        if ((DateTime.Now - t1).TotalMilliseconds > timeout)
                            break;
                    }
                }
                return s;
            }
        }

        public bool Send(string msg)
        {
            lock (locker1)
            {
                bool res = false;
                if (port.IsOpen)
                {
                    port.Write(msg);
                    res = true;
                }
                SendOver?.Invoke(new General.SendResult()
                {
                    SendCmd = Transfer.GetByte(msg),
                    Sender = port,
                    IsSuccess = res
                });
                return res;
            }
        }

        public bool Send(byte[] msg)
        {
            lock (locker1)
            {
                bool res = false;
                if (port.IsOpen && msg != null)
                {
                    DateTime t1 = DateTime.Now;
                    while (port.BytesToWrite > 0)
                    {
                        DateTime t2 = DateTime.Now;
                        if ((t2 - t1).TotalMilliseconds > 3000)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                    port.DiscardOutBuffer();
                    port.Write(msg, 0, msg.Length);
                    res = true;
                }
                SendOver?.Invoke(new General.SendResult()
                {
                    SendCmd = msg,
                    Sender = port,
                    IsSuccess = res
                });
                return res;
            }
        }
    }
}
