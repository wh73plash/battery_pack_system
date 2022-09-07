using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Peak.Can.Basic;
using TPCANMsgFD = System.UInt64;
using TPCANHandle = System.UInt16;
using TPCANBitrateFD = System.String;
using TPCANTimestampFD = System.UInt64;

namespace Pack_Monitor.CAN {
    public static class Members {
        public static class logdata {
            public static bool is_while = true;

            public static string packvoltage;
            public static string packcurrent;
            public static string packsoc;
            public static string logdatanumber;
            public static string logdatacount;

            public static string maxcellvoltage;
            public static string mincellvoltage;
            public static string averagevoltage;
            public static string differencecellvoltage;

            public static string maxcelltemperature;
            public static string mincelltemperature;
            public static string averagetemperature;
            public static string differencetemperature;

            public static string chargedischargecount;

            public static int status_c = 0;
            public static int status_p = 0;
            public static string lifecycle;

            public static bool nextline = false;

            public static bool log_complete = false;
        }
        public static bool error_cout_bool = false;

        public static bool display_setting = false;
        public static int setting_cnt = 3;

        public static UInt16 check_enable_buffer = 0;

        public static bool packready = false;
        public static bool packrelayon = false;
        public static bool packwarning = false;
        public static bool packfault = false;

        public static bool wcellovervoltage = false;
        public static bool wcellundervoltage = false;
        public static bool wdifferencecellvoltage = false;
        public static bool wcellovertemperature = false;
        public static bool wcellundertemperature = false;
        public static bool wdifferencecelltemperature = false;

        public static bool wpackovervotlage = false;
        public static bool wpackundervoltage = false;
        public static bool wchargeovercurrent = false;
        public static bool wdischargeovercurrent = false;
        public static bool wsocover = false;
        public static bool wsocunder = false;


        public static bool ccellovervoltage = false;
        public static bool ccellundervoltage = false;
        public static bool cdifferencecellvoltage = false;
        public static bool ccellovertemperature = false;
        public static bool ccellundertemperature = false;
        public static bool cdifferencecelltemperature = false;

        public static bool cpackovervotlage = false;
        public static bool cpackundervoltage = false;
        public static bool cchargeovercurrent = false;
        public static bool cdischargeovercurrent = false;
        public static bool csocover = false;
        public static bool csocunder = false;

        public static string batterycapacityvaluesetting;
        public static string celllifecyclesetting;
        public static string cellbalancingstartvsetting;

        public static string numberofcell;

        public static string[] povervoltage = new string[2];
        public static string[] pundervoltage = new string[2];
        public static string[] pchargeovercurrent = new string[2];
        public static string[] pdischargeovercurrent = new string[2];
        public static string[] poversoc = new string[2];
        public static string[] pundersoc = new string[2];
        public static string[] pundersoh = new string[2];
        public static string[] covervoltage = new string[2];
        public static string[] cundervoltage = new string[2];
        public static string[] cdeviationvoltage = new string[2];
        public static string[] covertemperature = new string[2];
        public static string[] cundertemperature = new string[2];
        public static string[] cdeviationtemperature = new string[2];

        public static string battery_type;
        public static string current_direction;
        public static string current_sensor_type;
        public static string numberoftemperature;
        public static string wakeuptype;

        /// <summary>
        /// Read-Delegate Handler
        /// </summary>
        public delegate void ReadDelegateHandler( );

        /// <summary>
        /// Saves the desired connection mode
        /// </summary>
        public static bool m_IsFD;
        /// <summary>
        /// Saves the handle of a PCAN hardware
        /// </summary>
        public static TPCANHandle m_PcanHandle;
        /// <summary>
        /// Saves the baudrate register for a conenction
        /// </summary>
        public static TPCANBaudrate m_Baudrate;
        /// <summary>
        /// Saves the type of a non-plug-and-play hardware
        /// </summary>
        public static TPCANType m_HwType;
        /// <summary>
        /// Stores the status of received messages for its display
        /// </summary>
        public static System.Collections.ArrayList m_LastMsgsList;
        /// <summary>
        /// Read Delegate for calling the function "ReadMessages"
        /// </summary>
        public static ReadDelegateHandler m_ReadDelegate;
        /// <summary>
        /// Receive-Event
        /// </summary>
        public static System.Threading.AutoResetEvent m_ReceiveEvent;
        /// <summary>
        /// Thread for message reading (using events)
        /// </summary>
        public static System.Threading.Thread m_ReadThread;
        /// <summary>
        /// Handles of non plug and play PCAN-Hardware
        /// </summary>
        public static TPCANHandle[] m_NonPnPHandles;
    }
}