using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Pack_Monitor.rs232c {
    public class data_set {
        public uint ID;
        public uint value_number;
        public ushort worf;
        public string message;
    };
    public static class rsConnection {
        public static void write(data_set data_Set) {

        }
    }
}
