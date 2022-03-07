using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LGHBAcsEngine;
using Peak.Can.Basic;

using TPCANHandle = System.UInt16;
using TPCANBitrateFD = System.String;
using TPCANTimestampFD = System.UInt64;

namespace Pack_Monitor.CAN {
    public class setting_data_set {
        public uint ID;
        public uint value_number;
        public ushort worf;
        public string message;
    };

    public static class Connection {
        public static bool[] b_ls = new bool[28];
        public static string[] buffer_ls = new string[38];
        public static bool connection_check = false;
        private static TPCANMsg process_write_message(setting_data_set message) {
            try {
                TPCANMsg msg = new TPCANMsg( );

                msg.DATA = new byte[8];
                msg.ID = message.ID;
                msg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
                msg.LEN = 8;
                msg.DATA[0] = Convert.ToByte(message.value_number);
                msg.DATA[1] = Convert.ToByte(message.worf);
                byte[] arr_b = new byte[2];
                switch (msg.DATA[0]) {
                    case 1:
                        msg.DATA[2] = 0x11;
                        return msg;
                    case 2:
                    case 3:
                    case 6:
                        if (message.message.Contains('.')) {
                            int buff1 = Convert.ToInt32(message.message.Replace(".", string.Empty));
                            byte[] buff2 = BitConverter.GetBytes(buff1);
                            msg.DATA[2] = buff2[0];
                            msg.DATA[3] = buff2[1];
                        } else {
                            int buff1 = Convert.ToInt32(message.message) * 10;
                            byte[] buff2 = BitConverter.GetBytes(buff1);
                            msg.DATA[2] = buff2[0];
                            msg.DATA[3] = buff2[1];
                        }
                        return msg;
                    case 4:
                        if (message.message.Contains('.')) {
                            if(message.message.Split('.')[1].Length < 2) {
                                arr_b = BitConverter.GetBytes(Convert.ToInt32(message.message.Replace(".", string.Empty), 10) * 10);
                                msg.DATA[2] = arr_b[0];
                                msg.DATA[3] = arr_b[1];
                            } else {
                                arr_b = BitConverter.GetBytes(Convert.ToInt32(message.message.Replace(".", string.Empty), 10));
                                msg.DATA[2] = arr_b[0];
                                msg.DATA[3] = arr_b[1];
                            }
                        } else {
                            arr_b = BitConverter.GetBytes(Convert.ToInt32(message.message, 10) * 100);
                            msg.DATA[2] = arr_b[0];
                            msg.DATA[3] = arr_b[1];
                        }
                        return msg;
                    case 5:
                    case 13:
                    case 14:
                    case 15:
                        arr_b = BitConverter.GetBytes(Convert.ToInt32(message.message, 10));
                        msg.DATA[2] = arr_b[0];
                        msg.DATA[3] = arr_b[1];
                        return msg;
                    case 7:
                        msg.DATA[2] = 0xAA;
                        return msg;
                    case 8:
                        msg.DATA[2] = 0x0A;
                        return msg;
                    case 9:
                        msg.DATA[2] = 0xA0;
                        return msg;
                    case 10:
                        if (message.message == "0xFF")
                            msg.DATA[2] = 0xFF;
                        else
                            msg.DATA[2] = 0xBB;
                        return msg;
                    case 11:
                        arr_b = BitConverter.GetBytes(Convert.ToInt32(message.message, 10));
                        msg.DATA[2] = arr_b[0];
                        msg.DATA[3] = arr_b[1];
                        return msg;
                    case 12:
                        int buffer = Convert.ToInt32(message.message, 10);
                        byte[] buff_arr = BitConverter.GetBytes(buffer);
                        msg.DATA[2] = buff_arr[0];
                        msg.DATA[3] = buff_arr[1];
                        return msg;
                    case 21:
                    case 22:
                        string[] string_buffer = message.message.Split(',');
                        string a = string_buffer[0];
                        if (!a.Contains('.')) {
                            buffer = Convert.ToInt32(a, 10) * 100;
                        } else {
                            string[] sstring = a.Split('.');
                            if (sstring[1].Length == 1) {
                                a += '0';
                            }
                            buffer = Convert.ToInt32(a.Replace(".", string.Empty), 10);
                        }
                        byte[] barr = BitConverter.GetBytes(buffer);
                        msg.DATA[2] = barr[0];
                        msg.DATA[3] = barr[1];

                        buffer = string_buffer[1].Contains('.') ? Convert.ToUInt16(string_buffer[1].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[1], 10);
                        msg.DATA[4] = Convert.ToByte(buffer);

                        a = string_buffer[2];
                        if (!a.Contains('.')) {
                            buffer = Convert.ToInt32(a, 10) * 100;
                        } else {
                            string[] sstring = a.Split('.');
                            if (sstring[1].Length == 1) {
                                a += '0';
                            }
                            buffer = Convert.ToInt32(a.Replace(".", string.Empty), 10);
                        }
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[5] = barr[0];
                        msg.DATA[6] = barr[1];

                        buffer = string_buffer[3].Contains('.') ? Convert.ToUInt16(string_buffer[3].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[3], 10);
                        msg.DATA[7] = Convert.ToByte(buffer);

                        return msg;
                    case 28:
                    case 29:
                        string_buffer = message.message.Split(',');
                        a = string_buffer[0];
                        if (!a.Contains('.')) {
                            buffer = Convert.ToInt32(a, 10) * 1000;
                        } else {
                            string[] sstring = a.Split('.');
                            switch (sstring[1].Length) {
                                case 1:
                                    a += "00";
                                    break;
                                case 2:
                                    a += "0";
                                    break;
                            }
                            buffer = Convert.ToInt32(a.Replace(".", string.Empty), 10);
                        }
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[2] = barr[0];
                        msg.DATA[3] = barr[1];

                        buffer = string_buffer[1].Contains('.') ? Convert.ToUInt16(string_buffer[1].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[1], 10);
                        msg.DATA[4] = Convert.ToByte(buffer);

                        a = string_buffer[2];
                        if (!a.Contains('.')) {
                            buffer = Convert.ToInt32(a, 10) * 1000;
                        } else {
                            string[] sstring = a.Split('.');
                            switch (sstring[1].Length) {
                                case 1:
                                    a += "00";
                                    break;
                                case 2:
                                    a += "0";
                                    break;
                            }
                            buffer = Convert.ToInt32(a.Replace(".", string.Empty), 10);
                        }
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[5] = barr[0];
                        msg.DATA[6] = barr[1];

                        buffer = string_buffer[3].Contains('.') ? Convert.ToUInt16(string_buffer[3].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[3], 10);
                        msg.DATA[7] = Convert.ToByte(buffer);
                        return msg;
                    case 23:
                    case 24:
                    case 31:
                    case 32:
                        string_buffer = message.message.Split(',');
                        a = string_buffer[0];
                        buffer = a.Contains('.') ? Convert.ToInt32(a.Replace(".", string.Empty), 10) : Convert.ToInt32(a, 10) * 10;
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[2] = barr[0];
                        msg.DATA[3] = barr[1];

                        buffer = string_buffer[1].Contains('.') ? Convert.ToUInt16(string_buffer[1].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[1], 10);
                        msg.DATA[4] = Convert.ToByte(buffer);

                        a = string_buffer[2];
                        buffer = a.Contains('.') ? Convert.ToInt32(a.Replace(".", string.Empty), 10) : Convert.ToInt32(a, 10) * 10;
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[5] = barr[0];
                        msg.DATA[6] = barr[1];

                        buffer = string_buffer[3].Contains('.') ? Convert.ToUInt16(string_buffer[3].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[3], 10);
                        msg.DATA[7] = Convert.ToByte(buffer);
                        return msg;
                    case 25:
                    case 26:
                        string_buffer = message.message.Split(',');
                        a = string_buffer[0].Contains('.') ? string_buffer[0].Replace(".", string.Empty) : string_buffer[0] + '0';
                        buffer = Convert.ToInt32(a, 10);
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[2] = barr[0];
                        msg.DATA[3] = barr[1];

                        buffer = string_buffer[1].Contains('.') ? Convert.ToUInt16(string_buffer[1].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[1], 10);
                        msg.DATA[4] = Convert.ToByte(buffer);

                        a = string_buffer[2].Contains('.') ? string_buffer[2].Replace(".", string.Empty) : string_buffer[2] + '0';
                        buffer = Convert.ToInt32(a, 10);
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[5] = barr[0];
                        msg.DATA[6] = barr[1];

                        buffer = string_buffer[3].Contains('.') ? Convert.ToUInt16(string_buffer[3].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[3], 10);
                        msg.DATA[7] = Convert.ToByte(buffer);
                        return msg;
                    case 27:
                        string_buffer = message.message.Split(',');
                        a = string_buffer[0].Contains('.') ? string_buffer[0].Replace(".", string.Empty) : string_buffer[0] + '0';
                        barr = BitConverter.GetBytes(Convert.ToInt32(a, 10));
                        msg.DATA[2] = barr[0];
                        msg.DATA[3] = barr[1];

                        buffer = string_buffer[1].Contains('.') ? Convert.ToUInt16(string_buffer[1].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[1], 10);
                        msg.DATA[4] = Convert.ToByte(buffer);
                        msg.DATA[5] = 0;
                        msg.DATA[6] = 0;
                        msg.DATA[7] = 0;
                        return msg;
                    case 30:
                        string_buffer = message.message.Split(',');
                        a = string_buffer[0];
                        buffer = Convert.ToInt32(a, 10);
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[2] = barr[0];
                        msg.DATA[3] = barr[1];

                        buffer = string_buffer[1].Contains('.') ? Convert.ToUInt16(string_buffer[1].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[1], 10);
                        msg.DATA[4] = Convert.ToByte(buffer);

                        a = string_buffer[2];
                        buffer = Convert.ToInt32(a, 10);
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[5] = barr[0];
                        msg.DATA[6] = barr[1];

                        buffer = string_buffer[3].Contains('.') ? Convert.ToUInt16(string_buffer[3].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[3], 10);
                        msg.DATA[7] = Convert.ToByte(buffer);
                        return msg;
                    case 33:
                        string_buffer = message.message.Split(',');
                        a = string_buffer[0];
                        buffer = a.Contains('.') ? Convert.ToInt32(a.Replace(".", string.Empty), 10) : Convert.ToInt32(a, 10) * 10;
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[2] = barr[0];
                        msg.DATA[3] = barr[1];

                        buffer = string_buffer[1].Contains('.') ? Convert.ToUInt16(string_buffer[1].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[1], 10);
                        msg.DATA[4] = Convert.ToByte(buffer);

                        a = string_buffer[2];
                        buffer = a.Contains('.') ? Convert.ToInt32(a.Replace(".", string.Empty), 10) : Convert.ToInt32(a, 10) * 10;
                        barr = BitConverter.GetBytes(buffer);
                        msg.DATA[5] = barr[0];
                        msg.DATA[6] = barr[1];

                        buffer = string_buffer[3].Contains('.') ? Convert.ToUInt16(string_buffer[3].Replace(".", string.Empty), 10) : Convert.ToUInt16(string_buffer[3], 10);
                        msg.DATA[7] = Convert.ToByte(buffer);
                        return msg;
                    case 34:
                        msg.DATA[2] = 0xFF;
                        return msg;
                }
                return msg;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
            return new TPCANMsg( );
        }

        public static void write_message(setting_data_set message) {
            try {
                TPCANStatus result;
                StringBuilder strMsg = new StringBuilder(256);
                TPCANMsg msg = new TPCANMsg( );

                msg = process_write_message(message);

                result = PCANBasic.Write(Members.m_PcanHandle, ref msg);
                if (result != TPCANStatus.PCAN_ERROR_OK) { //error
                    PCANBasic.GetErrorText(result, 0, strMsg);
                    TraceManager.AddLog("ERROR   #write message error  $" + strMsg.ToString( ) + "@");
                } else {
                    //no error
                    TraceManager.AddLog("SUCCESS #write message  $" + message.ID + "@" + message.message);
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        public static void process_setting_message(TPCANMsg newMsg) {
            byte[] bb = new byte[4];
            int[] arr = new int[4];

            switch (newMsg.DATA[0]) {
                case 4:
                    Members.setting_cnt++;
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    int asdf = BitConverter.ToInt32(bb, 0);
                    Members.batterycapacityvaluesetting = (asdf / 100).ToString( ) + "." + (asdf % 100).ToString("D2");
                    break;
                case 5:
                    Members.setting_cnt++;
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    Members.celllifecyclesetting = BitConverter.ToInt32(bb, 0).ToString( );
                    break;
                case 6:
                    bb = new byte[4];
                    Members.setting_cnt++;
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    int ba = BitConverter.ToInt32(bb, 0);
                    Members.cellbalancingstartvsetting = (ba / 1000).ToString( ) + '.' + (ba % 1000).ToString("D3");
                    break;
                case 10:
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    asdf = BitConverter.ToInt32(bb, 0);
                    if(asdf == 0xCC) {
                        MessageBox.Show("Log Data Memory is Empty", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }else if(asdf == 0xBB) {
                        MessageBox.Show("Log Data Send Complete Response", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Members.logdata.is_while = false;
                    break;
                case 11:
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    Members.numberofcell = BitConverter.ToInt32(bb, 0).ToString( );
                    break;
                case 12:
                    Members.setting_cnt++;
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    Members.check_enable_buffer = BitConverter.ToUInt16(bb, 0);
                    break;
                case 13:
                    Members.setting_cnt++;
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    Members.battery_type = BitConverter.ToInt32(bb, 0).ToString( );
                    break;
                case 14:
                    Members.setting_cnt++;
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    Members.current_direction = BitConverter.ToInt32(bb, 0).ToString( );
                    break;
                case 15:
                    Members.setting_cnt++;
                    bb = new byte[4];
                    Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                    Members.current_sensor_type = BitConverter.ToInt32(bb, 0).ToString( );
                    break;

                case 21:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.povervoltage[0] = 
                            (arr[0] / 100).ToString( ) + '.' + (arr[0] % 100).ToString("D2") + ',' +
                            arr[1].ToString( ) + ',' + 
                            (arr[2] / 100).ToString( ) + '.' + (arr[2] % 100).ToString("D2") + ',' + 
                            arr[3].ToString( );

                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.povervoltage[1] = 
                            (arr[0] / 100).ToString( ) + '.' + (arr[0] % 100).ToString("D2") + ',' + 
                            arr[1].ToString( ) + ',' + 
                            (arr[2] / 100).ToString( ) + '.' + (arr[2] % 100).ToString("D2") + ',' + 
                            arr[3].ToString( );
                    }
                    break;
                case 22:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.pundervoltage[0] = (arr[0] / 100).ToString( ) + '.' + (arr[0] % 100).ToString("D2") + ',' +
                            arr[1].ToString( ) + ',' + (arr[2] / 100).ToString( ) + '.' + (arr[2] % 100).ToString("D2") + ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.pundervoltage[1] = (arr[0] / 100).ToString( ) + '.' + (arr[0] % 100).ToString("D2") + ',' +
                            arr[1].ToString( ) + ',' + (arr[2] / 100).ToString( ) + '.' + (arr[2] % 100).ToString("D2") + ',' + arr[3].ToString( );
                    }
                    break;
                case 23:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.pchargeovercurrent[0] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        if (arr[1] / 10 == 0) {
                            Members.pchargeovercurrent[0] += ',' + arr[1].ToString( );
                        } else {
                            Members.pchargeovercurrent[0] += ',' + (arr[1] / 10).ToString( ) + '.' + (arr[1] % 10).ToString( );
                        }
                        Members.pchargeovercurrent[0] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        if (arr[3] / 10 == 0) {
                            Members.pchargeovercurrent[0] += ',' + arr[3].ToString( );
                        } else {
                            Members.pchargeovercurrent[0] += ',' + (arr[3] / 10).ToString( ) + '.' + (arr[3] % 10).ToString( );
                        }
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.pchargeovercurrent[1] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        if (arr[1] / 10 == 0) {
                            Members.pchargeovercurrent[1] += ',' + arr[1].ToString( );
                        } else {
                            Members.pchargeovercurrent[1] += ',' + (arr[1] / 10).ToString( ) + '.' + (arr[1] % 10).ToString( );
                        }
                        Members.pchargeovercurrent[1] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        if (arr[3] / 10 == 0) {
                            Members.pchargeovercurrent[1] += ',' + arr[3].ToString( );
                        } else {
                            Members.pchargeovercurrent[1] += ',' + (arr[3] / 10).ToString( ) + '.' + (arr[3] % 10).ToString( );
                        }
                    }
                    break;
                case 24:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        if (arr[0] >= 0x8000) {
                            arr[0] = 0xFFFF - (arr[0] - 1);
                            Members.pdischargeovercurrent[0] = '-' + (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        } else {
                            Members.pdischargeovercurrent[0] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        }

                        if (arr[1] / 10 == 0) {
                            Members.pdischargeovercurrent[0] += ',' + arr[1].ToString( );
                        } else {
                            Members.pdischargeovercurrent[0] += ',' + (arr[1] / 10).ToString( ) + '.' + (arr[1] % 10).ToString( );
                        }

                        if (arr[2] >= 0x8000) {
                            arr[2] = 0xFFFF - (arr[2] - 1);
                            Members.pdischargeovercurrent[0] += ",-" + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        } else {
                            Members.pdischargeovercurrent[0] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        }

                        if (arr[3] / 10 == 0) {
                            Members.pdischargeovercurrent[0] += ',' + arr[3].ToString( );
                        } else {
                            Members.pdischargeovercurrent[0] += ',' + (arr[3] / 10).ToString( ) + '.' + (arr[3] % 10).ToString( );
                        }
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        if (arr[0] >= 0x8000) {
                            arr[0] = 0xFFFF - (arr[0] - 1);
                            Members.pdischargeovercurrent[1] = '-' + (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        } else {
                            //positive
                            Members.pdischargeovercurrent[1] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        }
                        if (arr[1] / 10 == 0) {
                            Members.pdischargeovercurrent[1] += ',' + arr[1].ToString( );
                        } else {
                            Members.pdischargeovercurrent[1] += ',' + (arr[1] / 10).ToString( ) + '.' + (arr[1] % 10).ToString( );
                        }

                        if (arr[2] >= 0x8000) {
                            arr[2] = 0xFFFF - (arr[2] - 1);
                            Members.pdischargeovercurrent[1] += ",-" + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        } else {
                            Members.pdischargeovercurrent[1] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        }

                        if (arr[3] / 10 == 0) {
                            Members.pdischargeovercurrent[1] += ',' + arr[3].ToString( );
                        } else {
                            Members.pdischargeovercurrent[1] += ',' + (arr[3] / 10).ToString( ) + '.' + (arr[3] % 10).ToString( );
                        }
                    }
                    break;
                case 25:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.poversoc[0] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( ) + ',' + arr[1].ToString( ) + ',' +
                            (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( ) + ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.poversoc[1] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( ) + ',' + arr[1].ToString( ) + ',' +
                            (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( ) + ',' + arr[3].ToString( );
                    }
                    break;
                case 26:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.pundersoc[0] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( ) + ',' + arr[1].ToString( ) + ',' +
                            (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( ) + ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.pundersoc[1] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( ) + ',' + arr[1].ToString( ) + ',' +
                            (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( ) + ',' + arr[3].ToString( );
                    }
                    break;
                case 27:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);

                        Members.pundersoh[0] = (arr[0] / 10).ToString( ) + "." + (arr[0] % 10).ToString( ) + ',' + arr[1].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);

                        Members.pundersoh[1] = (arr[0] / 10).ToString( ) + "." + (arr[0] % 10).ToString( ) + ',' + arr[1].ToString( );
                    }
                    break;
                case 28:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.covervoltage[0] = (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + ',' +
                            arr[1].ToString( ) + ',' + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.covervoltage[1] = (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + ',' + arr[1].ToString( )
                            + ',' + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + ',' + arr[3].ToString( );
                    }
                    break;
                case 29:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.cundervoltage[0] = (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + ',' + arr[1].ToString( )
                            + ',' + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.cundervoltage[1] = (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + ',' + arr[1].ToString( )
                            + ',' + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + ',' + arr[3].ToString( );
                    }
                    break;
                case 30:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.cdeviationvoltage[0] = arr[0].ToString( );
                        Members.cdeviationvoltage[0] += ',' + arr[1].ToString( );
                        Members.cdeviationvoltage[0] += ',' + arr[2].ToString( );
                        Members.cdeviationvoltage[0] += ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.cdeviationvoltage[1] = arr[0].ToString( );
                        Members.cdeviationvoltage[1] += ',' + arr[1].ToString( );
                        Members.cdeviationvoltage[1] += ',' + arr[2].ToString( );
                        Members.cdeviationvoltage[1] += ',' + arr[3].ToString( );
                    }
                    break;
                case 31:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        if (arr[0] >= 0x8000) {
                            arr[0] = 0xFFFF - (arr[0] - 1);
                            Members.covertemperature[0] = '-' + (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        } else {
                            Members.covertemperature[0] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        }
                        Members.covertemperature[0] += ',' + arr[1].ToString( ) + ',';
                        if (arr[2] >= 0x8000) {
                            arr[2] = 0xFFFF - (arr[2] - 1);
                            Members.covertemperature[0] += (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        } else {
                            Members.covertemperature[0] += (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        }
                        Members.covertemperature[0] += ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        if (arr[0] >= 0x8000) {
                            arr[0] = 0xFFFF - (arr[0] - 1);
                            Members.covertemperature[1] = '-' + (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        } else {
                            Members.covertemperature[1] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        }
                        Members.covertemperature[1] += ',' + arr[1].ToString( );
                        if (arr[2] >= 0x8000) {
                            arr[2] = 0xFFFF - (arr[2] - 1);
                            Members.covertemperature[1] += ",-" + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        } else {
                            Members.covertemperature[1] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        }
                        Members.covertemperature[1] += ',' + arr[3].ToString( );
                    }
                    break;
                case 32:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        if (arr[0] >= 0x8000) {
                            arr[0] = 0xFFFF - (arr[0] - 1);
                            Members.cundertemperature[0] = '-' + (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        } else {
                            Members.cundertemperature[0] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        }
                        Members.cundertemperature[0] += ',' + arr[1].ToString( );
                        if (arr[2] >= 0x8000) {
                            arr[2] = 0xFFFF - (arr[2] - 1);
                            Members.cundertemperature[0] = ",-" + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        } else {
                            Members.cundertemperature[0] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        }
                        Members.cundertemperature[0] += ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        if (arr[0] >= 0x8000) {
                            arr[0] = 0xFFFF - (arr[0] - 1);
                            Members.cundertemperature[1] = '-' + (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        } else {
                            Members.cundertemperature[1] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        }
                        Members.cundertemperature[1] += ',' + arr[1].ToString( );
                        if (arr[2] >= 0x8000) {
                            arr[2] = 0xFFFF - (arr[2] - 1);
                            Members.cundertemperature[1] += ",-" + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        } else {
                            Members.cundertemperature[1] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        }
                        Members.cundertemperature[1] += ',' + arr[3].ToString( );
                    }
                    break;
                case 33:
                    Members.setting_cnt++;
                    if (newMsg.DATA[1] == 1) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.cdeviationtemperature[0] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        Members.cdeviationtemperature[0] += ',' + arr[1].ToString( );
                        Members.cdeviationtemperature[0] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        Members.cdeviationtemperature[0] += ',' + arr[3].ToString( );
                    } else if (newMsg.DATA[1] == 2) {
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.cdeviationtemperature[1] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        Members.cdeviationtemperature[1] += ',' + arr[1].ToString( );
                        Members.cdeviationtemperature[1] += ',' + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        Members.cdeviationtemperature[1] += ',' + arr[3].ToString( );

                        Members.display_setting = true;
                    }
                    break;
            }
        }

        private async static void process_log_data(TPCANMsg newMsg) {
            try {
                byte[] bb = new byte[4];
                int[] arr = new int[4];
                switch (newMsg.ID) {
                    case 0x140:
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 1);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 1, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        int buffer_ab = BitConverter.ToInt32(bb, 0);

                        Members.logdata.logdatanumber = arr[0].ToString( );
                        Members.logdata.logdatacount = arr[1].ToString( );
                        Members.logdata.packvoltage = (arr[2] / 100).ToString( ) + (arr[2] % 100).ToString("D2");
                        if(arr[3] >= 0x8000) {
                            arr[3] = 0xFFFF - (arr[3] - 1);
                            Members.logdata.packcurrent = "-" + (arr[3] / 10).ToString( ) + "." + (arr[3] % 10).ToString( );
                        } else {
                            Members.logdata.packcurrent = (arr[3] / 10).ToString( ) + "." + (arr[3] % 10).ToString( );
                        }
                        Members.logdata.packsoc = (buffer_ab / 10).ToString( ) + "." + (buffer_ab % 10).ToString( );
                        break;
                    case 0x141:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.logdata.maxcellvoltage = (arr[0] / 1000).ToString( ) + "." + (arr[0] % 1000).ToString("D3");
                        Members.logdata.mincellvoltage = (arr[1] / 1000).ToString( ) + "." + (arr[1] % 1000).ToString("D3");
                        Members.logdata.averagevoltage = (arr[2] / 1000).ToString( ) + "." + (arr[2] % 1000).ToString("D3");
                        Members.logdata.differencecellvoltage = (arr[3] / 1000).ToString( ) + "." + (arr[3] % 1000).ToString("D3");
                        break;
                    case 0x142:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        if(arr[0] >= 0x8000) {
                            arr[0] = 0xFFFF - (arr[0] - 1);
                            Members.logdata.maxcelltemperature = "-" + (arr[0] / 10).ToString( ) + "." + (arr[0] % 10).ToString( );
                        } else {
                            Members.logdata.maxcelltemperature = (arr[0] / 10).ToString( ) + "." + (arr[0] % 10).ToString( );
                        }

                        if (arr[1] >= 0x8000) {
                            arr[1] = 0xFFFF - (arr[1] - 1);
                            Members.logdata.mincelltemperature = "-" + (arr[1] / 10).ToString( ) + "." + (arr[1] % 10).ToString( );
                        } else {
                            Members.logdata.mincelltemperature = (arr[1] / 10).ToString( ) + "." + (arr[1] % 10).ToString( );
                        }

                        if (arr[2] >= 0x8000) {
                            arr[2] = 0xFFFF - (arr[2] - 1);
                            Members.logdata.averagetemperature = "-" + (arr[2] / 10).ToString( ) + "." + (arr[2] % 10).ToString( );
                        } else {
                            Members.logdata.averagetemperature = (arr[2] / 10).ToString( ) + "." + (arr[2] % 10).ToString( );
                        }
                        Members.logdata.differencetemperature = (arr[3] / 10).ToString( ) + "." + (arr[3] % 10).ToString( );
                        break;
                    case 0x143:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 4);
                        UInt32 buffer = BitConverter.ToUInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 1);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);

                        Members.logdata.chargedischargecount = (buffer / 100).ToString( ) + "." + (buffer % 100).ToString("D2");
                        string buffstring = Convert.ToString(arr[1], 2).PadLeft(8, '0');
                        Members.logdata.status_c = buffstring[7] == '1' ? 0b00000001 : 0;
                        Members.logdata.status_c += buffstring[6] == '1' ? 0b00000010 : 0;
                        Members.logdata.status_c += buffstring[5] == '1' ? 0b00000100 : 0;
                        Members.logdata.status_c += buffstring[3] == '1' ? 0b00010000 : 0;
                        Members.logdata.status_c += buffstring[2] == '1' ? 0b00100000 : 0;
                        Members.logdata.status_c += buffstring[1] == '1' ? 0b01000000 : 0;

                        buffstring = Convert.ToString(arr[2], 2).PadLeft(8, '0');
                        Members.logdata.status_p = buffstring[7] == '1' ? 0b00000001 : 0;
                        Members.logdata.status_p += buffstring[6] == '1' ? 0b00000010 : 0;
                        Members.logdata.status_p += buffstring[5] == '1' ? 0b00000100 : 0;
                        Members.logdata.status_p += buffstring[4] == '1' ? 0b00001000 : 0;
                        Members.logdata.status_p += buffstring[3] == '1' ? 0b00010000 : 0;
                        Members.logdata.status_p += buffstring[2] == '1' ? 0000100000 : 0;

                        Members.logdata.lifecycle = (arr[3] / 100).ToString( ) + "." + (arr[3] % 100).ToString("D2");
                        Members.logdata.nextline = true;
                        break;
                }
                return;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
                return;
            }
        }

        public async static void process_message(TPCANMsg newMsg) {
            try {
                byte[] bb = new byte[4];
                int[] arr = new int[4];
                switch (newMsg.ID) {
                    case 0x140:
                    case 0x141:
                    case 0x142:
                    case 0x143:
                        process_log_data(newMsg);
                        break;

                    case 0x130:
                        process_setting_message(newMsg);
                        break;
                    case protocol.PACK_DATA:
                        a++;
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        buffer_ls[0] = (arr[0] / 100).ToString( ) + '.' + (arr[0] % 100).ToString("D2");
                        if (arr[1] > 0x8000) {
                            int a = 0xFFFF - (arr[1] - 1);
                            buffer_ls[1] = '-' + (a / 10).ToString( ) + '.' + (a % 10).ToString( );
                        } else {
                            buffer_ls[1] = (arr[1] / 10).ToString( ) + '.' + (arr[1] % 10).ToString( );
                        }
                        buffer_ls[2] = (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        buffer_ls[3] = (arr[3] / 10).ToString( ) + '.' + (arr[3] % 10).ToString( );
                        connection_check = !connection_check;
                        break;
                    case protocol.CELL_VOLTAGE_DATA:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Connection.buffer_ls[4] = (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3");
                        Connection.buffer_ls[5] = (arr[1] / 1000).ToString( ) + '.' + (arr[1] % 1000).ToString("D3");
                        Connection.buffer_ls[6] = (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3");
                        Connection.buffer_ls[7] = (arr[3] / 1000).ToString( ) + '.' + (arr[3] % 1000).ToString("D3");
                        break;
                    case protocol.CELL_TEMPERATURE_DATA:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        if (arr[0] > 0x8000) {
                            int a = 0xFFFF - (arr[0] - 1);
                            Connection.buffer_ls[8] = (-1 * a / 10).ToString( ) + '.' + (a % 10).ToString( );
                        } else {
                            Connection.buffer_ls[8] = (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( );
                        }
                        if (arr[1] > 0x8000) {
                            int a = 0xFFFF - (arr[1] - 1);
                            Connection.buffer_ls[9] = (-1 * a / 10).ToString( ) + '.' + (a % 10).ToString( );
                        } else {
                            Connection.buffer_ls[9] = (arr[1] / 10).ToString( ) + '.' + (arr[1] % 10).ToString( );
                        }
                        if (arr[2] > 0x8000) {
                            int a = 0xFFFF - (arr[2] - 1);
                            Connection.buffer_ls[10] = (-1 * a / 10).ToString( ) + '.' + (a % 10).ToString( );
                        } else {
                            Connection.buffer_ls[10] = (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( );
                        }
                        Connection.buffer_ls[11] = (arr[3] / 10).ToString( ) + '.' + (arr[3] % 10).ToString( );
                        break;
                    case protocol.CELL_DATA_POSITION:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 1);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 1, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 1);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 3, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Connection.buffer_ls[12] = arr[0].ToString( );
                        Connection.buffer_ls[13] = arr[1].ToString( );
                        Connection.buffer_ls[14] = arr[2].ToString( );
                        Connection.buffer_ls[15] = arr[3].ToString( );
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 4);
                        UInt32 b = BitConverter.ToUInt32(bb, 0);
                        Connection.buffer_ls[16] = (b / 100).ToString( ) + '.' + (b % 100).ToString("D2");
                        break;
                    case protocol.PACK_STATE_DATA:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 1);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 1, bb, 0, 1);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 1);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 3, bb, 0, 1);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 1);
                        int bka = BitConverter.ToInt32(bb, 0);
                        string buffstring = Convert.ToString(arr[0], 2).PadLeft(8, '0');
                        Members.packready = buffstring[7] == '1';
                        Members.packwarning = buffstring[6] == '1';
                        Members.packfault = buffstring[5] == '1';
                        Members.packrelayon = buffstring[3] == '1';

                        buffstring = Convert.ToString(arr[1], 2).PadLeft(8, '0');
                        Members.wcellovervoltage = buffstring[7] == '1';
                        Members.wcellundervoltage = buffstring[6] == '1';
                        Members.wdifferencecellvoltage = buffstring[5] == '1';
                        Members.wcellovertemperature = buffstring[3] == '1';
                        Members.wcellundertemperature = buffstring[2] == '1';
                        Members.wdifferencecelltemperature = buffstring[1] == '1';

                        buffstring = Convert.ToString(arr[2], 2).PadLeft(8, '0');
                        Members.wpackovervotlage = buffstring[7] == '1';
                        Members.wpackundervoltage = buffstring[6] == '1';
                        Members.wchargeovercurrent = buffstring[5] == '1';
                        Members.wdischargeovercurrent = buffstring[4] == '1';
                        Members.wsocover = buffstring[3] == '1';
                        Members.wsocunder = buffstring[2] == '1';

                        buffstring = Convert.ToString(arr[3], 2).PadLeft(8, '0');
                        Members.ccellovervoltage = buffstring[7] == '1';
                        Members.ccellundervoltage = buffstring[6] == '1';
                        Members.cdifferencecellvoltage = buffstring[5] == '1';
                        Members.ccellovertemperature = buffstring[3] == '1';
                        Members.ccellundertemperature = buffstring[2] == '1';
                        Members.cdifferencecelltemperature = buffstring[1] == '1';

                        buffstring = Convert.ToString(bka, 2).PadLeft(8, '0');
                        Members.cpackovervotlage = buffstring[7] == '1';
                        Members.cpackundervoltage = buffstring[6] == '1';
                        Members.cchargeovercurrent = buffstring[5] == '1';
                        Members.cdischargeovercurrent = buffstring[4] == '1';
                        Members.csocover = buffstring[3] == '1';
                        Members.csocunder = buffstring[2] == '1';

                        Buffer.BlockCopy(newMsg.DATA, 5, bb, 0, 2);
                        int b1 = BitConverter.ToInt32(bb, 0);

                        bb = new byte[4];
                        Buffer.BlockCopy(newMsg.DATA, 7, bb, 0, 1);
                        int b2 = BitConverter.ToInt32(bb, 0);

                        Connection.buffer_ls[36] = (b1 / 10).ToString( ) + '.' + (b1 % 10).ToString( );
                        Connection.buffer_ls[37] = b2.ToString( );

                        break;
                    case protocol.CELL_VOLTAGE_1:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Connection.buffer_ls[17] = "#1    :    " + (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[18] = "#2    :    " + (arr[1] / 1000).ToString( ) + '.' + (arr[1] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[19] = "#3    :    " + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[20] = "#4    :    " + (arr[3] / 1000).ToString( ) + '.' + (arr[3] % 1000).ToString("D3") + "  V";
                        break;
                    case protocol.CELL_VOLTAGE_2:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Connection.buffer_ls[21] = "#5    :    " + (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[22] = "#6    :    " + (arr[1] / 1000).ToString( ) + '.' + (arr[1] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[23] = "#7    :    " + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[24] = "#8    :    " + (arr[3] / 1000).ToString( ) + '.' + (arr[3] % 1000).ToString("D3") + "  V";
                        break;
                    case protocol.CELL_VOLTAGE_3:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Connection.buffer_ls[25] = "#9    :    " + (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[26] = "#10  :    " + (arr[1] / 1000).ToString( ) + '.' + (arr[1] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[27] = "#11  :    " + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[28] = "#12  :    " + (arr[3] / 1000).ToString( ) + '.' + (arr[3] % 1000).ToString("D3") + "  V";
                        break;
                    case protocol.CELL_VOLTAGE_4:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        Connection.buffer_ls[29] = "#13  :    " + (arr[0] / 1000).ToString( ) + '.' + (arr[0] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[30] = "#14  :    " + (arr[1] / 1000).ToString( ) + '.' + (arr[1] % 1000).ToString("D3") + "  V";
                        Connection.buffer_ls[31] = "#15  :    " + (arr[2] / 1000).ToString( ) + '.' + (arr[2] % 1000).ToString("D3") + "  V";
                        break;
                    case protocol.CELL_TEMPERATURE:
                        Buffer.BlockCopy(newMsg.DATA, 0, bb, 0, 2);
                        arr[0] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 2, bb, 0, 2);
                        arr[1] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 4, bb, 0, 2);
                        arr[2] = BitConverter.ToInt32(bb, 0);
                        Buffer.BlockCopy(newMsg.DATA, 6, bb, 0, 2);
                        arr[3] = BitConverter.ToInt32(bb, 0);
                        if (arr[0] > 0x8000) {
                            int a = 0xFFFF - (arr[0] - 1);
                            Connection.buffer_ls[32] = "#1    :    " + (-1 * a / 10).ToString( ) + '.' + (a % 10).ToString( ) + " ℃";
                        } else {
                            Connection.buffer_ls[32] = "#1    :    " + (arr[0] / 10).ToString( ) + '.' + (arr[0] % 10).ToString( ) + " ℃";
                        }
                        if (arr[1] > 0x8000) {
                            int a = 0xFFFF - (arr[1] - 1);
                            Connection.buffer_ls[33] = "#2    :    " + (-1 * a / 10).ToString( ) + '.' + (a % 10).ToString( ) + " ℃";
                        } else {
                            Connection.buffer_ls[33] = "#2    :    " + (arr[1] / 10).ToString( ) + '.' + (arr[1] % 10).ToString( ) + " ℃";
                        }
                        if (arr[2] > 0x8000) {
                            int a = 0xFFFF - (arr[2] - 1);
                            Connection.buffer_ls[34] = "#3    :    " + (-1 * a / 10).ToString( ) + '.' + (a % 10).ToString( ) + " ℃";
                        } else {
                            Connection.buffer_ls[34] = "#3    :    " + (arr[2] / 10).ToString( ) + '.' + (arr[2] % 10).ToString( ) + " ℃";
                        }
                        if (arr[3] > 0x8000) {
                            int a = 0xFFFF - (arr[3] - 1);
                            Connection.buffer_ls[35] = "#4    :    " + (-1 * a / 10).ToString( ) + '.' + (a % 10).ToString( ) + " ℃";
                        } else {
                            Connection.buffer_ls[35] = "#4    :    " + (arr[3] / 10).ToString( ) + '.' + (arr[3] % 10).ToString( ) + " ℃";
                        }
                        break;
                }

                string message = "SUCCESS #read message  $" + newMsg.ID + "@";
                for (int i = 0; i < newMsg.LEN; ++i) {
                    message += newMsg.DATA[i] + " | ";
                }
                TraceManager.AddLog(message);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }
        public static int a = 0;

        public static void reset( ) {
            try {
                TPCANStatus result;
                StringBuilder strMsg = new StringBuilder(256);

                result = PCANBasic.Reset(Members.m_PcanHandle);
                if (result != TPCANStatus.PCAN_ERROR_OK) {
                    PCANBasic.GetErrorText(result, 0, strMsg);
                    TraceManager.AddLog("ERROR   #can queue reset error  $" + strMsg.ToString( ));
                } else {
                    TraceManager.AddLog("SUCCESS #pcan reset  $PCAN reset @No Exception StackTrace");
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        public async static void read_message( ) {
            try {
                TPCANMsg CANMsg;
                TPCANStatus stsResult;
                StringBuilder strMsg = new StringBuilder(256);
                stsResult = PCANBasic.Read(Members.m_PcanHandle, out CANMsg);
                if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY && CANMsg.ID != 0) {
                    Members.error_cout_bool = true;
                    process_message(CANMsg);
                    return;
                } else {
                    PCANBasic.GetErrorText(stsResult, 0, strMsg);
                    //TraceManager.AddLog("ERROR   #Read Message_error  $" + strMsg.ToString( ));
                    return;
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
                return;
            }
        }

        public static bool disconnect( ) {
            StringBuilder strMsg = new StringBuilder(256);
            try {
                TPCANStatus result = PCANBasic.Uninitialize(Members.m_PcanHandle);
                if (result != TPCANStatus.PCAN_ERROR_OK) { //error
                    PCANBasic.GetErrorText(result, 0, strMsg);
                    TraceManager.AddLog("ERROR   #DisConnection_Error  $" + strMsg.ToString( ) + "@No Exception StackTrace");
                    MessageBox.Show(strMsg.ToString( ), "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                } else { //no error
                    TraceManager.AddLog("DISCONNECT #DisConnect  $PCAN Disconnected @No Exception StackTrace");
                    MessageBox.Show("PCAN DisConnected", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
                MessageBox.Show(ex.Message, "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool connect( ) {
            TPCANStatus stsResult;
            Members.m_PcanHandle = PCANBasic.PCAN_USBBUS1;
            StringBuilder strMsg = new StringBuilder(256);
            try {
                Members.m_HwType = 0;
                Members.m_Baudrate = TPCANBaudrate.PCAN_BAUD_500K;
                stsResult = PCANBasic.Initialize(
                    Members.m_PcanHandle,
                    Members.m_Baudrate,
                    Members.m_HwType,
                    Convert.ToUInt32("0100"),
                    0x3);
                if (stsResult != TPCANStatus.PCAN_ERROR_OK) { //ERROR
                    PCANBasic.GetErrorText(stsResult, 0, strMsg);
                    TraceManager.AddLog("ERROR   #Connection_Error  $" + strMsg.ToString( ) + "@No Exception StackTrace");
                    MessageBox.Show(strMsg.ToString( ), "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                } else { //NO ERROR
                    TraceManager.AddLog("CONNECT #Connect  $PCAN Connected @No Exception StackTrace");
                    MessageBox.Show("PCAN Connected", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
                MessageBox.Show(ex.Message, "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}