using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pack_Monitor.CAN {
    public static class protocol {
        public const UInt32 PACK_DATA = 288;
        public const UInt32 CELL_VOLTAGE_DATA = 289;
        public const UInt32 CELL_TEMPERATURE_DATA = 290;
        public const UInt32 CELL_DATA_POSITION = 291;
        public const UInt32 PACK_STATE_DATA = 292;
        public const UInt32 CELL_VOLTAGE_1 = 293;
        public const UInt32 CELL_VOLTAGE_2 = 294;
        public const UInt32 CELL_VOLTAGE_3 = 295;
        public const UInt32 CELL_VOLTAGE_4 = 296;
        public const UInt32 CELL_TEMPERATURE = 297;

        public const UInt32 VALUE_SETTING = 0x130;
        public const UInt32 DISPLAY_READ = 0x130;
    }
}
