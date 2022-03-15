using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

using LGHBAcsEngine;
using Peak.Can.Basic;
using Pack_Monitor.CAN;

namespace Pack_Monitor {
    public partial class main : Form {
        bool is_communicate = false, is_connected = false;
        public UInt16 check_enable = 0;
        string csv_savefile_path;

        public main( ) {
            InitializeComponent( );
        }

        public void start_setting( ) {
            try {
                TraceManager.AddLog("ENTER #login   $No Exception Message @No Exception StackTrace");
                combobox_port.DataSource = SerialPort.GetPortNames( );
                tab_control.TabPages.Remove(setting_tab);
                setting_tab.Enabled = false;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR #Error(1)  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void main_form_FormClosing(object sender, FormClosingEventArgs e) {
            TraceManager.AddLog("EXIT  #logout  $No Exception Message @No Exception StackTrace");
        }
        private void temperature_data_Click(object sender, EventArgs e) {
            this.main_temperature_data.SelectedIndex = -1;
        }

        private void voltage_data_Click(object sender, EventArgs e) {
            this.main_voltage_data.SelectedIndex = -1;
        }

        private void initialize_btn_Click(object sender, EventArgs e) {
            try {
                if (can_connect.Checked) {
                    if (Connection.connect( )) {
                        connect_state.Text = "Connected - CAN";
                        connect_state.ForeColor = Color.Black;
                        is_connected = true;
                        initialize_btn.BackColor = Color.Lime;
                        data_receive.Interval = 25;
                        data_receive.Enabled = true;
                    }
                } else if (rs232_connect.Checked) {
                    if (!rsport.IsOpen) {

                        is_connected = true;
                        rsport.PortName = combobox_port.Text;  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                        rsport.BaudRate = 19200;  //보레이트 변경이 필요하면 숫자 변경하기
                        rsport.DataBits = 8;
                        rsport.StopBits = StopBits.One;
                        rsport.Parity = Parity.None;

                        rsport.Open( );  //시리얼포트 열기

                        connect_state.Text = "Connected - RS232C";
                        combobox_port.Enabled = false;  //COM포트설정 콤보박스 비활성화
                        initialize_btn.BackColor = Color.Lime;
                        connect_state.ForeColor = Color.Black;
                    }
                } else {
                    MessageBox.Show("No Connection method has been selected", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
                is_connected = false;
            }
        }

        private void release_btn_Click(object sender, EventArgs e) {
            try {
                if (connect_state.Text == "Connected - CAN") {
                    Connection.disconnect( );
                } else if (connect_state.Text == "Connected - RS232C") {
                    rsport.Close( );
                }
                combobox_port.Enabled = true;
                connect_state.ForeColor = Color.Gray;
                initialize_btn.BackColor = Color.White;
                Com_start_btn.BackColor = Color.White;
                connect_state.Text = "UnConnected";
                data_receive.Enabled = timer.Enabled = timer_display.Enabled = false;
                is_connected = false;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void value_display( ) {
            try {
                int cnt = 0;
                main_pack_voltage_value.Text = Connection.buffer_ls[cnt++];
                main_pack_current_value.Text = Connection.buffer_ls[cnt++];
                main_soc_value.Text = Connection.buffer_ls[cnt++];
                main_soh_value.Text = Connection.buffer_ls[cnt++];
                main_cell_voltage_max_value.Text = Connection.buffer_ls[cnt++];
                main_cell_voltage_min_value.Text = Connection.buffer_ls[cnt++];
                main_cell_voltage_average_value.Text = Connection.buffer_ls[cnt++];
                main_cell_voltage_deviation_value.Text = Connection.buffer_ls[cnt++];
                main_cell_temperature_max_value.Text = Connection.buffer_ls[cnt++];
                main_cell_temperature_min_value.Text = Connection.buffer_ls[cnt++];
                main_cell_temperature_average_value.Text = Connection.buffer_ls[cnt++];
                main_cell_temperature_deviation_value.Text = Connection.buffer_ls[cnt++];
                main_cell_voltage_maxpossition_value.Text = Connection.buffer_ls[cnt++];
                main_cell_voltage_minpossition_value.Text = Connection.buffer_ls[cnt++];
                main_cell_temperature_maxpossition_value.Text = Connection.buffer_ls[cnt++];
                main_cell_temperature_minpossition_value.Text = Connection.buffer_ls[cnt++];
                main_capacity_cnt.Text = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[0] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[1] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[2] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[3] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[4] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[5] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[6] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[7] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[8] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[9] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[10] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[11] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[12] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[13] = Connection.buffer_ls[cnt++];
                main_voltage_data.Items[14] = Connection.buffer_ls[cnt++];
                main_temperature_data.Items[0] = Connection.buffer_ls[cnt++];
                main_temperature_data.Items[1] = Connection.buffer_ls[cnt++];
                main_temperature_data.Items[2] = Connection.buffer_ls[cnt++];
                main_temperature_data.Items[3] = Connection.buffer_ls[cnt++];
                main_life_cycle.Text = Connection.buffer_ls[cnt++];
                main_log_data_cnt.Text = Connection.buffer_ls[cnt++];

                if (Members.packready)
                    packready.ForeColor = Color.Blue;
                else
                    packready.ForeColor = Color.Silver;
                if (Members.packrelayon)
                    packrelayon.ForeColor = Color.Blue;
                else
                    packrelayon.ForeColor = Color.Silver;
                if (Members.packwarning)
                    packwarning.ForeColor = Color.DarkKhaki;
                else
                    packwarning.ForeColor = Color.Silver;
                if (Members.packfault)
                    packfault.ForeColor = Color.DarkRed;
                else
                    packfault.ForeColor = Color.Silver;

                if (Members.wcellovervoltage)
                    wcellovervoltagew.ForeColor = Color.DarkKhaki;
                else
                    wcellovervoltagew.ForeColor = Color.Silver;
                if (Members.wcellundervoltage)
                    wcellundervoltage.ForeColor = Color.DarkKhaki;
                else
                    wcellundervoltage.ForeColor = Color.Silver;
                if (Members.wdifferencecellvoltage)
                    wdifferencecellvoltage.ForeColor = Color.DarkKhaki;
                else
                    wdifferencecellvoltage.ForeColor = Color.Silver;
                if (Members.wcellovertemperature)
                    wcellovertemp.ForeColor = Color.DarkKhaki;
                else
                    wcellovertemp.ForeColor = Color.Silver;
                if (Members.wcellundertemperature)
                    wcellundertemp.ForeColor = Color.DarkKhaki;
                else
                    wcellundertemp.ForeColor = Color.Silver;
                if (Members.wdifferencecelltemperature)
                    wdiffcelltemp.ForeColor = Color.DarkKhaki;
                else
                    wdiffcelltemp.ForeColor = Color.Silver;

                if (Members.wpackovervotlage)
                    wpackovervoltage.ForeColor = Color.DarkKhaki;
                else
                    wpackovervoltage.ForeColor = Color.Silver;
                if (Members.wpackundervoltage)
                    wpackundervoltage.ForeColor = Color.DarkKhaki;
                else
                    wpackundervoltage.ForeColor = Color.Silver;
                if (Members.wchargeovercurrent)
                    wchargeovercurrent.ForeColor = Color.DarkKhaki;
                else
                    wchargeovercurrent.ForeColor = Color.Silver;
                if (Members.wdischargeovercurrent)
                    wdischargeovercurrent.ForeColor = Color.DarkKhaki;
                else
                    wdischargeovercurrent.ForeColor = Color.Silver;
                if (Members.wsocover)
                    wsocover.ForeColor = Color.DarkKhaki;
                else
                    wsocover.ForeColor = Color.Silver;
                if (Members.wsocunder)
                    wsocunder.ForeColor = Color.DarkKhaki;
                else
                    wsocunder.ForeColor = Color.Silver;

                if (Members.ccellovervoltage)
                    fcellovervoltage.ForeColor = Color.DarkRed;
                else
                    fcellovervoltage.ForeColor = Color.Silver;
                if (Members.ccellundervoltage)
                    fcellundevoltage.ForeColor = Color.DarkRed;
                else
                    fcellundevoltage.ForeColor = Color.Silver;
                if (Members.cdifferencecellvoltage)
                    fdifferencecellvoltage.ForeColor = Color.DarkRed;
                else
                    fdifferencecellvoltage.ForeColor = Color.Silver;
                if (Members.ccellovertemperature)
                    fcellovertemperature.ForeColor = Color.DarkRed;
                else
                    fcellovertemperature.ForeColor = Color.Silver;
                if (Members.ccellundertemperature)
                    fcellundertemp.ForeColor = Color.DarkRed;
                else
                    fcellundertemp.ForeColor = Color.Silver;
                if (Members.cdifferencecelltemperature)
                    fdiffcelltemp.ForeColor = Color.DarkRed;
                else
                    fdiffcelltemp.ForeColor = Color.Silver;

                if (Members.cpackovervotlage)
                    fpackovervoltage.ForeColor = Color.DarkRed;
                else
                    fpackovervoltage.ForeColor = Color.Silver;
                if (Members.cpackundervoltage)
                    fpackovervoltage.ForeColor = Color.DarkRed;
                else
                    fpackovervoltage.ForeColor = Color.Silver;
                if (Members.cchargeovercurrent)
                    fchargeovercurrent.ForeColor = Color.DarkRed;
                else
                    fchargeovercurrent.ForeColor = Color.Silver;
                if (Members.cdischargeovercurrent)
                    fdischargeovercurrent.ForeColor = Color.DarkRed;
                else
                    fdischargeovercurrent.ForeColor = Color.Silver;
                if (Members.csocover)
                    fsocover.ForeColor = Color.DarkRed;
                else
                    fsocover.ForeColor = Color.Silver;
                if (Members.csocunder)
                    fsocunder.ForeColor = Color.DarkRed;
                else
                    fsocunder.ForeColor = Color.Silver;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private int error_count = 0;
        private bool data_receive_test = false;

        private void timer_Tick(object sender, EventArgs e) {
            try {
                if (is_connected) {
                    if (connect_state.Text == "Connected - CAN") {
                        setting_data_set newMessage = new setting_data_set( );
                        newMessage.ID = protocol.DISPLAY_READ;
                        newMessage.value_number = 1;
                        newMessage.worf = 0x0A;
                        newMessage.message = "0x11";
                        Connection.reset( );
                        Connection.write_message(newMessage);
                    } else if (connect_state.Text == "Connected - RS232C") {
                        byte[ ] data = new byte[13];
                        data[0] = 0x02;
                        data[1] = 0x30;
                        data[2] = 0x01;
                        data[3] = 0x08;
                        data[4] = 0x07;
                        data[5] = 0x1A;
                        data[6] = 0xAA;
                        data[12] = 0x03;
                        rsport.Write(data, 0, 13);
                        TraceManager.AddLog("rs232c data receive command sent");
                    }
                    if (Connection.connection_check)
                        Com_start_btn.BackColor = Color.White;
                    else
                        Com_start_btn.BackColor = Color.Lime;

                    if (Connection.connection_check == data_receive_test)
                        ++error_count;

                    data_receive_test = Connection.connection_check;

                    if (error_count >= 5) {
                        error_count = 0;
                        timer_display.Enabled = timer.Enabled = false;
                        Com_start_btn.BackColor = Color.White;
                        is_communicate = false;
                        Connection.reset( );
                        MessageBox.Show("Interrupting the communication due to a communication problem", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        TraceManager.AddLog("ERROR   #communicate  $Communicate problem detected @interrupted the communication");
                    }
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void setting_socset_btn_Click(object sender, EventArgs e) {
            try {
                if (setting_soc_value.Text == string.Empty) {
                    return;
                }
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = protocol.VALUE_SETTING;
                newMessage.value_number = 2;
                newMessage.worf = 0x0B;
                newMessage.message = setting_soc_value.Text;
                Connection.write_message(newMessage);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void setting_sohset_btn_Click(object sender, EventArgs e) {
            try {
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = protocol.VALUE_SETTING;
                newMessage.value_number = 3;
                newMessage.worf = 0x0C;
                newMessage.message = setting_soh_value.Text;
                Connection.write_message(newMessage);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void setting_values_process(string file_path) {
            //한줄씩읽고 ','으로 나눠서 순서에 맞게 데이터 세팅
            try {
                string context = File.ReadAllText(file_path);
                string[ ] tmp = context.Split(',');
                int cnt = 0;
                setting_soc_value.Text = tmp[cnt++];
                setting_soh_value.Text = tmp[cnt++];
                pw_over_voltage_detection.Text = tmp[cnt++];
                pw_over_voltage_detection_time.Text = tmp[cnt++];
                pw_over_voltage_release.Text = tmp[cnt++];
                pw_over_voltage_release_time.Text = tmp[cnt++];
                pf_over_voltage_detection.Text = tmp[cnt++];
                pf_over_voltage_detection_time.Text = tmp[cnt++];
                pf_over_voltage_release.Text = tmp[cnt++];
                pf_over_voltage_release_time.Text = tmp[cnt++];
                pw_under_voltage_detection.Text = tmp[cnt++];
                pw_under_voltage_detection_time.Text = tmp[cnt++];
                pw_under_voltage_release.Text = tmp[cnt++];
                pw_under_voltage_release_time.Text = tmp[cnt++];
                pf_under_voltage_detection.Text = tmp[cnt++];
                pf_under_voltage_detection_time.Text = tmp[cnt++];
                pf_under_voltage_release.Text = tmp[cnt++];
                pf_under_voltage_release_time.Text = tmp[cnt++];
                pw_charge_over_current_detection.Text = tmp[cnt++];
                pw_charge_over_current_detection_time.Text = tmp[cnt++];
                pw_charge_over_current_release.Text = tmp[cnt++];
                pw_charge_over_current_release_time.Text = tmp[cnt++];
                pf_charge_over_current_detection.Text = tmp[cnt++];
                pf_charge_over_current_detection_time.Text = tmp[cnt++];
                pf_charge_over_current_release.Text = tmp[cnt++];
                pf_charge_over_current_release_time.Text = tmp[cnt++];
                pw_discharge_over_current_detection.Text = tmp[cnt++];
                pw_discharge_over_current_detection_time.Text = tmp[cnt++];
                pw_discharge_over_current_release.Text = tmp[cnt++];
                pw_discharge_over_current_release_time.Text = tmp[cnt++];
                pf_discharge_over_current_detection.Text = tmp[cnt++];
                pf_discharge_over_current_detection_time.Text = tmp[cnt++];
                pf_discharge_over_current_release.Text = tmp[cnt++];
                pf_discharge_over_current_release_time.Text = tmp[cnt++];
                pw_over_soc_detection.Text = tmp[cnt++];
                pw_over_soc_detection_time.Text = tmp[cnt++];
                pw_over_soc_release.Text = tmp[cnt++];
                pw_over_soc_release_time.Text = tmp[cnt++];
                pf_over_soc_detection.Text = tmp[cnt++];
                pf_over_soc_detection_time.Text = tmp[cnt++];
                pf_over_soc_release.Text = tmp[cnt++];
                pf_over_soc_release_time.Text = tmp[cnt++];
                pw_under_soc_detection.Text = tmp[cnt++];
                pw_under_soc_detection_time.Text = tmp[cnt++];
                pw_under_soc_release.Text = tmp[cnt++];
                pw_under_soc_release_time.Text = tmp[cnt++];
                pf_under_soc_detection.Text = tmp[cnt++];
                pf_under_soc_detection_time.Text = tmp[cnt++];
                pf_under_soc_release.Text = tmp[cnt++];
                pf_under_soc_release_time.Text = tmp[cnt++];
                pw_under_soh_detection.Text = tmp[cnt++];
                pw_under_soh_detection_time.Text = tmp[cnt++];
                cw_over_voltage_detection.Text = tmp[cnt++];
                cw_over_voltage_detection_time.Text = tmp[cnt++];
                cw_over_voltage_release.Text = tmp[cnt++];
                cw_over_voltage_release_time.Text = tmp[cnt++];
                cf_over_voltage_detection.Text = tmp[cnt++];
                cf_over_voltage_detection_time.Text = tmp[cnt++];
                cf_over_voltage_release.Text = tmp[cnt++];
                cf_over_voltage_release_time.Text = tmp[cnt++];
                cw_under_voltage_detection.Text = tmp[cnt++];
                cw_under_voltage_detection_time.Text = tmp[cnt++];
                cw_under_voltage_release.Text = tmp[cnt++];
                cw_under_voltage_release_time.Text = tmp[cnt++];
                cf_under_voltage_detection.Text = tmp[cnt++];
                cf_under_voltage_detection_time.Text = tmp[cnt++];
                cf_under_voltage_release.Text = tmp[cnt++];
                cf_under_voltage_release_time.Text = tmp[cnt++];
                cw_charge_over_current_detection.Text = tmp[cnt++];
                cw_charge_over_current_detection_time.Text = tmp[cnt++];
                cw_charge_over_current_release.Text = tmp[cnt++];
                cw_charge_over_current_release_time.Text = tmp[cnt++];
                cf_charge_over_current_detection.Text = tmp[cnt++];
                cf_charge_over_current_detection_time.Text = tmp[cnt++];
                cf_charge_over_current_release.Text = tmp[cnt++];
                cf_charge_over_current_release_time.Text = tmp[cnt++];
                cw_discharge_over_current_detection.Text = tmp[cnt++];
                cw_discharge_over_current_detection_time.Text = tmp[cnt++];
                cw_discharge_over_current_release.Text = tmp[cnt++];
                cw_discharge_over_current_release_time.Text = tmp[cnt++];
                cf_discharge_over_current_detection.Text = tmp[cnt++];
                cf_discharge_over_current_detection_time.Text = tmp[cnt++];
                cf_discharge_over_current_release.Text = tmp[cnt++];
                cf_discharge_over_current_release_time.Text = tmp[cnt++];
                cw_over_soc_detection.Text = tmp[cnt++];
                cw_over_soc_detection_time.Text = tmp[cnt++];
                cw_over_soc_release.Text = tmp[cnt++];
                cw_over_soc_release_time.Text = tmp[cnt++];
                cf_over_soc_detection.Text = tmp[cnt++];
                cf_over_soc_detection_time.Text = tmp[cnt++];
                cf_over_soc_release.Text = tmp[cnt++];
                cf_over_soc_release_time.Text = tmp[cnt++];
                cw_under_soc_detection.Text = tmp[cnt++];
                cw_under_soc_detection_time.Text = tmp[cnt++];
                cw_under_soc_release.Text = tmp[cnt++];
                cw_under_soc_release_time.Text = tmp[cnt++];
                cf_under_soc_detection.Text = tmp[cnt++];
                cf_under_soc_detection_time.Text = tmp[cnt++];
                cf_under_soc_release.Text = tmp[cnt++];
                cf_under_soc_release_time.Text = tmp[cnt++];
                setting_battery_capacity.Text = tmp[cnt++];
                setting_cell_life_cycle.Text = tmp[cnt++];
                setting_number_of_cell.Text = tmp[cnt++];
                setting_cell_balancing_start_v.Text = tmp[cnt++];
                p_overvoltage_ch.Checked = Convert.ToBoolean(tmp[cnt++]);
                p_undervoltage_ch.Checked = Convert.ToBoolean(tmp[cnt++]);
                p_chargeovercurrent_ch.Checked = Convert.ToBoolean(tmp[cnt++]);
                p_dischargeovercurrent_ch.Checked = Convert.ToBoolean(tmp[cnt++]);
                p_oversoc_ch.Checked = Convert.ToBoolean(tmp[cnt++]);
                p_undersoc_ch.Checked = Convert.ToBoolean(tmp[cnt++]);
                p_undersoh_ch.Checked = Convert.ToBoolean(tmp[cnt++]);
                checkBox12.Checked = Convert.ToBoolean(tmp[cnt++]);
                checkBox11.Checked = Convert.ToBoolean(tmp[cnt++]);
                checkBox10.Checked = Convert.ToBoolean(tmp[cnt++]);
                checkBox9.Checked = Convert.ToBoolean(tmp[cnt++]);
                checkBox8.Checked = Convert.ToBoolean(tmp[cnt++]);
                checkBox7.Checked = Convert.ToBoolean(tmp[cnt++]);
                setting_bettery_type_cmb.Text = tmp[cnt++];
                setting_current_direction_cmb.Text = tmp[cnt++];
                setting_current_sensor_cmb.Text = tmp[cnt];
                return;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void setting_read_from_file_btn_Click(object sender, EventArgs e) {
            try {
                string file_path;
                OpenFileDialog openFileDialog = new OpenFileDialog( );
                if (Properties.Settings.Default.save_file_path == string.Empty) {
                    Properties.Settings.Default.save_file_path = Directory.GetCurrentDirectory( );
                }
                openFileDialog.InitialDirectory = Properties.Settings.Default.save_file_path;
                openFileDialog.Title = "To Read file location";
                openFileDialog.DefaultExt = "ion";
                openFileDialog.Filter = "Setting Data Files (*.ion)|*.ion";
                openFileDialog.ShowDialog( );
                if (openFileDialog.FileName.Length <= 0) {
                    throw new Exception("Failed to retrieve file location or lenth is 0");
                } else {
                    file_path = openFileDialog.FileName;
                }
                setting_values_process(file_path);
                TraceManager.AddLog("FILEREAD #Setting file read @read from : " + file_path);
                MessageBox.Show("Successfully read the file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Properties.Settings.Default.save_file_path = file_path;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private string getdisplaylog( ) {
            int a = 0;
            string logs = DateTime.Now.ToString("yyyyMMdd HH-mm-ss") + ',' + main_pack_voltage_value.Text + ',' + main_pack_current_value.Text + ',' + main_soc_value.Text + ',' + main_soh_value.Text + ',' +
                main_cell_voltage_max_value.Text + ',' + main_cell_voltage_min_value.Text + ',' + main_cell_voltage_average_value.Text + ',' + main_cell_voltage_deviation_value.Text + ',' + main_cell_temperature_max_value.Text + ',' +
                main_cell_temperature_min_value.Text + ',' + main_cell_temperature_average_value.Text + ',' + main_cell_temperature_deviation_value.Text + ',' + main_cell_voltage_maxpossition_value.Text + ',' +
                main_cell_voltage_minpossition_value.Text + ',' + main_cell_temperature_maxpossition_value.Text + ',' + main_cell_temperature_minpossition_value.Text + ',' +
                main_capacity_cnt.Text + ',';


            /*
              + Members.packready + ',' + Members.packwarning + ',' + Members.packfault + ',' + Members.packrelayon + ','
             Members.wcellovervoltage + ',' + Members.wcellundervoltage + ',' + Members.wdifferencecelltemperature + ',' + Members.wcellovertemperature + ',' + Members.wcellundertemperature
             + ',' + Members.wdifferencecelltemperature + ',' + Members.wpackovervotlage + ',' + Members.wpackundervoltage + ',' + Members.wchargeovercurrent + ',' + Members.wdischargeovercurrent
             + ',' + Members.wsocover + ',' + Members.wsocunder + ',' +

             Members.ccellovervoltage + ',' + Members.ccellundervoltage + ',' + Members.cdifferencecelltemperature + ',' + Members.ccellovertemperature + ',' + Members.ccellundertemperature
             + ',' + Members.cdifferencecelltemperature + ',' + Members.cpackovervotlage + ',' + Members.cpackundervoltage + ',' + Members.cchargeovercurrent + ',' + Members.cdischargeovercurrent
             + ',' + Members.csocover + ',' + Members.csocunder + ',';*/

            //state
            a += Members.packready ? 0b00000001 : 0;
            a += Members.packwarning ? 0b00000010 : 0;
            a += Members.packfault ? 0b00000100 : 0;

            a += Members.packrelayon ? 0b00010000 : 0;
            logs += "0x" + Convert.ToString(a, 16) + ',';

            //warning - 1 
            a = Members.wcellovervoltage ? 0b00000001 : 0;
            a += Members.wcellundervoltage ? 0b00000010 : 0;
            a += Members.wdifferencecelltemperature ? 0b00000100 : 0;

            a += Members.wcellovertemperature ? 0b00010000 : 0;
            a += Members.wcellundertemperature ? 0b00100000 : 0;
            a += Members.wdifferencecelltemperature ? 0b01000000 : 0;
            logs += "0x" + Convert.ToString(a, 16) + ',';

            //warning - 2
            a = Members.wpackovervotlage ? 0b00000001 : 0;
            a += Members.wpackundervoltage ? 0b00000010 : 0;
            a += Members.wchargeovercurrent ? 0b00000100 : 0;

            a += Members.wdischargeovercurrent ? 0b00001000 : 0;
            a += Members.wsocover ? 0b00010000 : 0;
            a += Members.wsocunder ? 0b00100000 : 0;
            logs += "0x" + Convert.ToString(a, 16) + ',';

            //fault - 1
            a = Members.ccellovervoltage ? 0b00000001 : 0;
            a += Members.ccellundervoltage ? 0b00000010 : 0;
            a += Members.cdifferencecelltemperature ? 0b00000100 : 0;

            a += Members.ccellovertemperature ? 0b00010000 : 0;
            a += Members.ccellundertemperature ? 0b00100000 : 0;
            a += Members.cdifferencecelltemperature ? 0b01000000 : 0;
            logs += "0x" + Convert.ToString(a, 16) + ',';

            //fault - 2
            a = Members.cpackovervotlage ? 0b00000001 : 0;
            a += Members.cpackundervoltage ? 0b00000010 : 0;
            a += Members.cchargeovercurrent ? 0b00000100 : 0;

            a += Members.cdischargeovercurrent ? 0b00001000 : 0;
            a += Members.csocover ? 0b00010000 : 0;
            a += Members.csocunder ? 0b00100000 : 0;
            logs += "0x" + Convert.ToString(a, 16) + ',';



            logs += main_life_cycle.Text + ',' + main_log_data_cnt.Text + ',' +
            convert_tostring(main_voltage_data.Items[0].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[1].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[2].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[3].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[4].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[5].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[6].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[7].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[8].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[9].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[10].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[11].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[12].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[13].ToString( )) + ',' +
            convert_tostring(main_voltage_data.Items[14].ToString( )) + ',' +

            convert_tostring(main_temperature_data.Items[0].ToString( )) + ',' +
            convert_tostring(main_temperature_data.Items[1].ToString( )) + ',' +
            convert_tostring(main_temperature_data.Items[2].ToString( )) + ',' +
            convert_tostring(main_temperature_data.Items[3].ToString( ));
            return logs;
        }

        private string convert_tostring(string a) {
            a = a.Split(new string[ ] { " :    " }, StringSplitOptions.None)[1];
            a = a.Split(' ')[0];
            return a;
        }

        private void leave_log( ) {
            //data save 버튼 눌렀을때 1초마다 실행되는 로그 남기기
            try {
                string path = csv_savefile_path;
                if (!File.Exists(path)) {
                    using (File.Create(path)) {
                        TraceManager.AddLog("SUCCESS #create log file $logpath:" + path);
                    }
                    StreamWriter writera;
                    writera = File.AppendText(path);
                    string logsa = "Date,Pack Voltage,Pack Current,Pack SOC,Pack SOH,Max Cell Voltage,Min Cell Voltage,Average Voltage,Difference Cell Voltage,Max Cell Temperature,Min Cell Temperature," +
                        "Average Temperature,Difference Temperature,Max Cell V Position,Min Cell V Position,Max Temp Position,Min Temp Position,Ah Count," +
                        "state," +

                        "Cell W,Pack W," +

                        "Cell F,Pack F" +

                        ",Life Cycle,Log data count" +

                        ",C V #1,C V #2,C V #3,C V #4,C V #5,C V #6,C V #7,C V #8,C V #9" +
                        ",C V #10,C V #11,C V #12,C V #13,C V #14,C V #15" +

                        ",C T #1,C T #2,C T #3,C T #4";
                    writera.WriteLine(logsa);
                    TraceManager.AddLog("SUCCESS #leave_log $logpath:" + path + " @Data:" + logsa);
                    writera.Close( );
                }
                StreamWriter writer;
                writer = File.AppendText(path);
                string logs = getdisplaylog( );
                writer.WriteLine(logs);
                writer.Close( );
                TraceManager.AddLog("SUCCESS #leave_log $logpath:" + path + " @Data:" + logs);
                return;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void data_save_btn_Click(object sender, EventArgs e) {
            if (data_save_btn.Text == "Data Save") {
                SaveFileDialog saveFileDialog = new SaveFileDialog( );
                if (Properties.Settings.Default.save_file_path == string.Empty) {
                    Properties.Settings.Default.save_file_path = Directory.GetCurrentDirectory( );
                }
                saveFileDialog.InitialDirectory = Properties.Settings.Default.save_file_path;
                saveFileDialog.Title = "Please specify the path to save";
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";

                if (saveFileDialog.ShowDialog( ) == DialogResult.OK) {
                    csv_savefile_path = saveFileDialog.FileName;
                    data_save_timer.Interval = 1000;
                    data_save_timer.Enabled = true;
                    data_save_btn.Text = "Saving...";
                    data_save_btn.BackColor = Color.Lime;
                }
            } else {
                data_save_timer.Enabled = false;
                data_save_btn.Text = "Data Save";
                data_save_btn.BackColor = Color.White;
            }
        }

        private void data_save_timer_Tick(object sender, EventArgs e) {
            leave_log( );
        }

        private void wrtie_setting_value_to_file(string path) {
            //경로 파일 생성 이후 setting에있는 모든 데이터 순차적으로 입력
            try {
                string data = setting_soc_value.Text + ',' + setting_soh_value.Text + ',' + pw_over_voltage_detection.Text + ',' + pw_over_voltage_detection_time.Text + ',' + pw_over_voltage_release.Text + ',' +
                pw_over_voltage_release_time.Text + ',' + pf_over_voltage_detection.Text + ',' + pf_over_voltage_detection_time.Text + ',' + pf_over_voltage_release.Text + ',' + pf_over_voltage_release_time.Text + ',' +
                pw_under_voltage_detection.Text + ',' + pw_under_voltage_detection_time.Text + ',' + pw_under_voltage_release.Text + ',' + pw_under_voltage_release_time.Text + ',' +
                pf_under_voltage_detection.Text + ',' + pf_under_voltage_detection_time.Text + ',' + pf_under_voltage_release.Text + ',' + pw_under_voltage_release_time.Text + ',' +
                pw_charge_over_current_detection.Text + ',' + pw_charge_over_current_detection_time.Text + ',' + pw_charge_over_current_release.Text + ',' + pw_charge_over_current_release_time.Text + ',' +
                pf_charge_over_current_detection.Text + ',' + pf_charge_over_current_detection_time.Text + ',' + pf_charge_over_current_release.Text + ',' + pf_charge_over_current_release_time.Text + ',' +
                pw_discharge_over_current_detection.Text + ',' + pw_discharge_over_current_detection_time.Text + ',' + pw_discharge_over_current_release.Text + ',' + pw_discharge_over_current_release_time.Text + ',' +
                pf_discharge_over_current_detection.Text + ',' + pf_discharge_over_current_detection_time.Text + ',' + pf_discharge_over_current_release.Text + ',' + pf_discharge_over_current_release_time.Text + ',' +
                pw_over_soc_detection.Text + ',' + pw_over_soc_detection_time.Text + ',' + pw_over_soc_release.Text + ',' + pw_over_soc_release_time.Text + ',' +
                pf_over_soc_detection.Text + ',' + pf_over_soc_detection_time.Text + ',' + pf_over_soc_release.Text + ',' + pf_over_soc_release_time.Text + ',' +
                pw_under_soc_detection.Text + ',' + pw_under_soc_detection_time.Text + ',' + pw_under_soc_release.Text + ',' + pw_under_soc_release_time.Text + ',' +
                pf_under_soc_detection.Text + ',' + pf_under_soc_detection_time.Text + ',' + pf_under_soc_release.Text + ',' + pf_under_soc_release_time.Text + ',' +
                pw_under_soh_detection.Text + ',' + pw_under_soh_detection_time.Text + ',' +
                cw_over_voltage_detection.Text + ',' + cw_over_voltage_detection_time.Text + ',' + cw_over_voltage_release.Text + ',' + cw_over_voltage_release_time.Text + ',' +
                cf_over_voltage_detection.Text + ',' + cf_over_voltage_detection_time.Text + ',' + cf_over_voltage_release.Text + ',' + cf_over_voltage_release_time.Text + ',' +
                cw_under_voltage_detection.Text + ',' + cw_under_voltage_detection_time.Text + ',' + cw_under_voltage_release.Text + ',' + cw_under_voltage_release_time.Text + ',' +
                cf_under_voltage_detection.Text + ',' + cf_under_voltage_detection_time.Text + ',' + cf_under_voltage_release.Text + ',' + cf_under_voltage_release_time.Text + ',' +
                cw_charge_over_current_detection.Text + ',' + cw_charge_over_current_detection_time.Text + ',' + cw_charge_over_current_release.Text + ',' + cw_charge_over_current_release_time.Text + ',' +
                cf_charge_over_current_detection.Text + ',' + cf_charge_over_current_detection_time.Text + ',' + cf_charge_over_current_release.Text + ',' + cf_charge_over_current_release_time.Text + ',' +
                cw_discharge_over_current_detection.Text + ',' + cw_discharge_over_current_detection_time.Text + ',' + cw_discharge_over_current_release.Text + ',' + cw_discharge_over_current_release_time.Text + ',' +
                cf_discharge_over_current_detection.Text + ',' + cf_discharge_over_current_detection_time.Text + ',' + cf_discharge_over_current_release.Text + ',' + cf_discharge_over_current_release_time.Text + ',' +
                cw_over_soc_detection.Text + ',' + cw_over_soc_detection_time.Text + ',' + cw_over_soc_release.Text + ',' + cw_over_soc_release_time.Text + ',' +
                cf_over_soc_detection.Text + ',' + cf_over_soc_detection_time.Text + ',' + cf_over_soc_release.Text + ',' + cf_over_soc_release_time.Text + ',' +
                cw_under_soc_detection.Text + ',' + cw_under_soc_detection_time.Text + ',' + cw_under_soc_release.Text + ',' + cw_under_soc_release_time.Text + ',' +
                cf_under_soc_detection.Text + ',' + cf_under_soc_detection_time.Text + ',' + cf_under_soc_release.Text + ',' + cf_under_soc_release_time.Text + ',' +
                setting_battery_capacity.Text + ',' + setting_cell_life_cycle.Text + ',' + setting_number_of_cell.Text + ',' + setting_cell_balancing_start_v.Text + ',' +
                p_overvoltage_ch.Checked + ',' + p_undervoltage_ch.Checked + ',' + p_chargeovercurrent_ch.Checked + ',' + p_dischargeovercurrent_ch.Checked + ',' +
                p_oversoc_ch.Checked + ',' + p_undersoc_ch.Checked + ',' + p_undersoh_ch.Checked + ',' + checkBox12.Checked + ',' + checkBox11.Checked + ',' +
                checkBox10.Checked + ',' + checkBox9.Checked + ',' + checkBox8.Checked + ',' + checkBox7.Checked + ',' + setting_bettery_type_cmb.Text
                + ',' + setting_current_direction_cmb.Text + ',' + setting_current_sensor_cmb.Text + ',';
                StreamWriter fp;
                fp = File.AppendText(path);
                fp.Write(data);
                fp.Close( );
                return;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void save_to_file_btn_Click(object sender, EventArgs e) {
            try {
                string file_path;
                SaveFileDialog saveFile = new SaveFileDialog( );
                if (Properties.Settings.Default.save_file_path == string.Empty) {
                    Properties.Settings.Default.save_file_path = Directory.GetCurrentDirectory( );
                }
                saveFile.InitialDirectory = Properties.Settings.Default.save_file_path;
                saveFile.Title = "To save file location";
                saveFile.DefaultExt = "ion";
                saveFile.Filter = "Setting Data Files (*.ion)|*.ion";

                if (saveFile.ShowDialog( ) == DialogResult.OK) {
                    if (saveFile.FileName.Length <= 0) {
                        throw new Exception("Failed to retrieve file location or lenth is 0");
                    } else {
                        file_path = saveFile.FileName.ToString( );
                        wrtie_setting_value_to_file(file_path);
                        TraceManager.AddLog("SUCCESS #setting_log_writed $log file path : " + file_path);
                    }
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void dataclearbtn_Click(object sender, EventArgs e) {
            try {
                pw_over_voltage_detection.Text = string.Empty;
                pw_over_voltage_detection_time.Text = string.Empty;
                pw_over_voltage_release.Text = string.Empty;
                pw_over_voltage_release_time.Text = string.Empty;

                pw_under_voltage_detection.Text = string.Empty;
                pw_under_voltage_detection_time.Text = string.Empty;
                pw_under_voltage_release.Text = string.Empty;
                pw_under_voltage_release_time.Text = string.Empty;

                pw_charge_over_current_detection.Text = string.Empty;
                pw_charge_over_current_detection_time.Text = string.Empty;
                pw_charge_over_current_release.Text = string.Empty;
                pw_charge_over_current_release_time.Text = string.Empty;

                pw_discharge_over_current_detection.Text = string.Empty;
                pw_discharge_over_current_detection_time.Text = string.Empty;
                pw_discharge_over_current_release.Text = string.Empty;
                pw_discharge_over_current_release_time.Text = string.Empty;

                pw_over_soc_detection.Text = string.Empty;
                pw_over_soc_detection_time.Text = string.Empty;
                pw_over_soc_release.Text = string.Empty;
                pw_over_soc_release_time.Text = string.Empty;

                pw_under_soc_detection.Text = string.Empty;
                pw_under_soc_detection_time.Text = string.Empty;
                pw_under_soc_release.Text = string.Empty;
                pw_under_soc_release_time.Text = string.Empty;

                pw_under_soh_detection.Text = string.Empty;
                pw_under_soh_detection_time.Text = string.Empty;

                pf_over_voltage_detection.Text = string.Empty;
                pf_over_voltage_detection_time.Text = string.Empty;
                pf_over_voltage_release.Text = string.Empty;
                pf_over_voltage_release_time.Text = string.Empty;

                pf_under_voltage_detection.Text = string.Empty;
                pf_under_voltage_detection_time.Text = string.Empty;
                pf_under_voltage_release.Text = string.Empty;
                pf_under_voltage_release_time.Text = string.Empty;

                pf_charge_over_current_detection.Text = string.Empty;
                pf_charge_over_current_detection_time.Text = string.Empty;
                pf_charge_over_current_release.Text = string.Empty;
                pf_charge_over_current_release_time.Text = string.Empty;

                pf_discharge_over_current_detection.Text = string.Empty;
                pf_discharge_over_current_detection_time.Text = string.Empty;
                pf_discharge_over_current_release.Text = string.Empty;
                pf_discharge_over_current_release_time.Text = string.Empty;

                pf_over_soc_detection.Text = string.Empty;
                pf_over_soc_detection_time.Text = string.Empty;
                pf_over_soc_release.Text = string.Empty;
                pf_over_soc_release_time.Text = string.Empty;

                pf_under_soc_detection.Text = string.Empty;
                pf_under_soc_detection_time.Text = string.Empty;
                pf_under_soc_release.Text = string.Empty;
                pf_under_soc_release_time.Text = string.Empty;

                cw_over_voltage_detection.Text = string.Empty;
                cw_over_voltage_detection_time.Text = string.Empty;
                cw_over_voltage_release.Text = string.Empty;
                cw_over_voltage_release_time.Text = string.Empty;

                cw_under_voltage_detection.Text = string.Empty;
                cw_under_voltage_detection_time.Text = string.Empty;
                cw_under_voltage_release.Text = string.Empty;
                cw_under_voltage_release_time.Text = string.Empty;

                cw_charge_over_current_detection.Text = string.Empty;
                cw_charge_over_current_detection_time.Text = string.Empty;
                cw_charge_over_current_release.Text = string.Empty;
                cw_charge_over_current_release_time.Text = string.Empty;

                cw_discharge_over_current_detection.Text = string.Empty;
                cw_discharge_over_current_detection_time.Text = string.Empty;
                cw_discharge_over_current_release.Text = string.Empty;
                cw_discharge_over_current_release_time.Text = string.Empty;

                cw_over_soc_detection.Text = string.Empty;
                cw_over_soc_detection_time.Text = string.Empty;
                cw_over_soc_release.Text = string.Empty;
                cw_over_soc_release_time.Text = string.Empty;

                cw_under_soc_detection.Text = string.Empty;
                cw_under_soc_detection_time.Text = string.Empty;
                cw_under_soc_release.Text = string.Empty;
                cw_under_soc_release_time.Text = string.Empty;

                cf_over_voltage_detection.Text = string.Empty;
                cf_over_voltage_detection_time.Text = string.Empty;
                cf_over_voltage_release.Text = string.Empty;
                cf_over_voltage_release_time.Text = string.Empty;

                cf_under_voltage_detection.Text = string.Empty;
                cf_under_voltage_detection_time.Text = string.Empty;
                cf_under_voltage_release.Text = string.Empty;
                cf_under_voltage_release_time.Text = string.Empty;

                cf_charge_over_current_detection.Text = string.Empty;
                cf_charge_over_current_detection_time.Text = string.Empty;
                cf_charge_over_current_release.Text = string.Empty;
                cf_charge_over_current_release_time.Text = string.Empty;

                cf_discharge_over_current_detection.Text = string.Empty;
                cf_discharge_over_current_detection_time.Text = string.Empty;
                cf_discharge_over_current_release.Text = string.Empty;
                cf_discharge_over_current_release_time.Text = string.Empty;

                cf_over_soc_detection.Text = string.Empty;
                cf_over_soc_detection_time.Text = string.Empty;
                cf_over_soc_release.Text = string.Empty;
                cf_over_soc_release_time.Text = string.Empty;

                cf_under_soc_detection.Text = string.Empty;
                cf_under_soc_detection_time.Text = string.Empty;
                cf_under_soc_release.Text = string.Empty;
                cf_under_soc_release_time.Text = string.Empty;

                setting_battery_capacity.Text = string.Empty;
                setting_cell_life_cycle.Text = string.Empty;
                setting_cell_balancing_start_v.Text = string.Empty;
                setting_number_of_cell.Text = string.Empty;

                setting_soc_value.Text = string.Empty;
                setting_soh_value.Text = string.Empty;

                p_overvoltage_ch.Checked = false;
                p_undervoltage_ch.Checked = false;
                p_chargeovercurrent_ch.Checked = false;
                p_dischargeovercurrent_ch.Checked = false;
                p_oversoc_ch.Checked = false;
                p_undersoc_ch.Checked = false;
                p_undersoh_ch.Checked = false;

                checkBox12.Checked = false;
                checkBox11.Checked = false;
                checkBox10.Checked = false;
                checkBox9.Checked = false;
                checkBox8.Checked = false;
                checkBox7.Checked = false;
                TraceManager.AddLog("SUCCESS #Data Clear  $setting data set clear");
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private async Task download_bms( ) {
            try {
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = 0x130;
                newMessage.value_number = 4;
                newMessage.worf = 0x0D;
                newMessage.message = setting_battery_capacity.Text;
                TraceManager.AddLog("WRITE #write message $value number:4 @message:" + setting_battery_capacity.Text);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 5;
                newMessage.worf = 0x0E;
                newMessage.message = setting_cell_life_cycle.Text;
                TraceManager.AddLog("WRITE #write message $value number:5 @message:" + setting_cell_life_cycle.Text);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 6;
                newMessage.worf = 0x0F;
                newMessage.message = setting_cell_balancing_start_v.Text;
                TraceManager.AddLog("WRITE #write message $value number:6 @message:" + setting_cell_balancing_start_v.Text);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 11;
                newMessage.worf = 0x1E;
                newMessage.message = setting_number_of_cell.Text;
                TraceManager.AddLog("WRITE #write message $value number:11 @message:" + setting_number_of_cell.Text);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 12;
                newMessage.worf = 0x1F;
                newMessage.message = check_enable.ToString( );
                TraceManager.AddLog("WRITE #write message $value number:12 @message:" + check_enable.ToString( ));
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 21;
                newMessage.worf = 1;
                newMessage.message = pw_over_voltage_detection.Text + ',' + pw_over_voltage_detection_time.Text + ',' + pw_over_voltage_release.Text + ',' + pw_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:21 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 21;
                newMessage.worf = 2;
                newMessage.message = pf_over_voltage_detection.Text + ',' + pf_over_voltage_detection_time.Text + ',' + pf_over_voltage_release.Text + ',' + pf_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:21 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 22;
                newMessage.worf = 1;
                newMessage.message = pw_under_voltage_detection.Text + ',' + pw_under_voltage_detection_time.Text + ',' + pw_under_voltage_release.Text + ',' + pw_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:22 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 22;
                newMessage.worf = 2;
                newMessage.message = pf_under_voltage_detection.Text + ',' + pf_under_voltage_detection_time.Text + ',' + pf_under_voltage_release.Text + ',' + pf_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:22 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 23;
                newMessage.worf = 1;
                newMessage.message = pw_charge_over_current_detection.Text + ',' + pw_charge_over_current_detection_time.Text + ',' + pw_charge_over_current_release.Text + ',' + pw_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:23 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 23;
                newMessage.worf = 2;
                newMessage.message = pf_charge_over_current_detection.Text + ',' + pf_charge_over_current_detection_time.Text + ',' + pf_charge_over_current_release.Text + ',' + pf_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:23 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 24;
                newMessage.worf = 1;
                newMessage.message = pw_discharge_over_current_detection.Text + ',' + pw_discharge_over_current_detection_time.Text + ',' + pw_discharge_over_current_release.Text + ',' + pw_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:24 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 24;
                newMessage.worf = 2;
                newMessage.message = pf_discharge_over_current_detection.Text + ',' + pf_discharge_over_current_detection_time.Text + ',' + pf_discharge_over_current_release.Text + ',' + pf_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:24 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 25;
                newMessage.worf = 1;
                newMessage.message = pw_over_soc_detection.Text + ',' + pw_over_soc_detection_time.Text + ',' + pw_over_soc_release.Text + ',' + pw_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:25 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 25;
                newMessage.worf = 2;
                newMessage.message = pf_over_soc_detection.Text + ',' + pf_over_soc_detection_time.Text + ',' + pf_over_soc_release.Text + ',' + pf_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:25 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 26;
                newMessage.worf = 1;
                newMessage.message = pw_under_soc_detection.Text + ',' + pw_under_soc_detection_time.Text + ',' + pw_under_soc_release.Text + ',' + pw_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:26 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 26;
                newMessage.worf = 2;
                newMessage.message = pf_under_soc_detection.Text + ',' + pf_under_soc_detection_time.Text + ',' + pf_under_soc_release.Text + ',' + pf_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:26 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 27;
                newMessage.worf = 1;
                newMessage.message = pw_under_soh_detection.Text + ',' + pw_under_soh_detection_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:27 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                newMessage.value_number = 28;
                newMessage.worf = 1;
                newMessage.message = cw_over_voltage_detection.Text + ',' + cw_over_voltage_detection_time.Text + ',' + cw_over_voltage_release.Text + ',' + cw_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:28 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 28;
                newMessage.worf = 2;
                newMessage.message = cf_over_voltage_detection.Text + ',' + cf_over_voltage_detection_time.Text + ',' + cf_over_voltage_release.Text + ',' + cf_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:28 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 29;
                newMessage.worf = 1;
                newMessage.message = cw_under_voltage_detection.Text + ',' + cw_under_voltage_detection_time.Text + ',' + cw_under_voltage_release.Text + ',' + cw_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:29 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 29;
                newMessage.worf = 2;
                newMessage.message = cf_under_voltage_detection.Text + ',' + cf_under_voltage_detection_time.Text + ',' + cf_under_voltage_release.Text + ',' + cf_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:29 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 30;
                newMessage.worf = 1;
                newMessage.message = cw_charge_over_current_detection.Text + ',' + cw_charge_over_current_detection_time.Text + ',' + cw_charge_over_current_release.Text + ',' + cw_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:30 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 30;
                newMessage.worf = 2;
                newMessage.message = cf_charge_over_current_detection.Text + ',' + cf_charge_over_current_detection_time.Text + ',' + cf_charge_over_current_release.Text + ',' + cf_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:30 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 31;
                newMessage.worf = 1;
                newMessage.message = cw_discharge_over_current_detection.Text + ',' + cw_discharge_over_current_detection_time.Text + ',' + cw_discharge_over_current_release.Text + ',' + cw_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:31 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 31;
                newMessage.worf = 2;
                newMessage.message = cf_discharge_over_current_detection.Text + ',' + cf_discharge_over_current_detection_time.Text + ',' + cf_discharge_over_current_release.Text + ',' + cf_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:31 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 32;
                newMessage.worf = 1;
                newMessage.message = cw_over_soc_detection.Text + ',' + cw_over_soc_detection_time.Text + ',' + cw_over_soc_release.Text + ',' + cw_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:32 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 32;
                newMessage.worf = 2;
                newMessage.message = cf_over_soc_detection.Text + ',' + cf_over_soc_detection_time.Text + ',' + cf_over_soc_release.Text + ',' + cf_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:32 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 33;
                newMessage.worf = 1;
                newMessage.message = cw_under_soc_detection.Text + ',' + cw_under_soc_detection_time.Text + ',' + cw_under_soc_release.Text + ',' + cw_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:33 1 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 33;
                newMessage.worf = 2;
                newMessage.message = cf_under_soc_detection.Text + ',' + cf_under_soc_detection_time.Text + ',' + cf_under_soc_release.Text + ',' + cf_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:33 2 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 13;
                newMessage.worf = 16;
                newMessage.message = setting_bettery_type_cmb.Text;
                TraceManager.AddLog("WRITE #write message $value number:13 16 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 14;
                newMessage.worf = 17;
                newMessage.message = setting_current_direction_cmb.Text;
                TraceManager.AddLog("WRITE #write message $value number:14 17 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 15;
                newMessage.worf = 18;
                newMessage.message = setting_current_sensor_cmb.Text;
                TraceManager.AddLog("WRITE #write message $value number:15 18 @message:" + newMessage.message);
                Connection.write_message(newMessage);
                Thread.Sleep(10);

                if (data_save_command.Checked) {
                    newMessage.value_number = 0x22;
                    newMessage.worf = 0x2A;
                    newMessage.message = "0xFF";
                    TraceManager.AddLog("WRITE #write message $value number:34 0x2A @message:" + newMessage.message);
                    Connection.write_message(newMessage);
                    Thread.Sleep(10);

                    Connection.read_message( );
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
            return;
        }

        private void send(setting_data_set message) {
            byte[ ] data = new byte[13];
            data[0] = 0x02;
            data[1] = 0x30;
            data[2] = 0x01;
            data[3] = 0x08;
            data[12] = 0x03;
            TPCANMsg buffer = Connection.process_write_message(message);
            for (int i = 0; i < 8; ++i)
                Buffer.BlockCopy(buffer.DATA, i, data, 4 + i, 1);
            rsport.Write(data, 0, 13);
        }

        private async Task rsdownload_bms( ) {
            try {
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = 0x130;
                newMessage.value_number = 4;
                newMessage.worf = 0x0D;
                newMessage.message = setting_battery_capacity.Text;
                TraceManager.AddLog("WRITE #write message $value number:4 @message:" + setting_battery_capacity.Text);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 5;
                newMessage.worf = 0x0E;
                newMessage.message = setting_cell_life_cycle.Text;
                TraceManager.AddLog("WRITE #write message $value number:5 @message:" + setting_cell_life_cycle.Text);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 6;
                newMessage.worf = 0x0F;
                newMessage.message = setting_cell_balancing_start_v.Text;
                TraceManager.AddLog("WRITE #write message $value number:6 @message:" + setting_cell_balancing_start_v.Text);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 11;
                newMessage.worf = 0x1E;
                newMessage.message = setting_number_of_cell.Text;
                TraceManager.AddLog("WRITE #write message $value number:11 @message:" + setting_number_of_cell.Text);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 12;
                newMessage.worf = 0x1F;
                newMessage.message = check_enable.ToString( );
                TraceManager.AddLog("WRITE #write message $value number:12 @message:" + check_enable.ToString( ));
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 21;
                newMessage.worf = 1;
                newMessage.message = pw_over_voltage_detection.Text + ',' + pw_over_voltage_detection_time.Text + ',' + pw_over_voltage_release.Text + ',' + pw_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:21 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 21;
                newMessage.worf = 2;
                newMessage.message = pf_over_voltage_detection.Text + ',' + pf_over_voltage_detection_time.Text + ',' + pf_over_voltage_release.Text + ',' + pf_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:21 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 22;
                newMessage.worf = 1;
                newMessage.message = pw_under_voltage_detection.Text + ',' + pw_under_voltage_detection_time.Text + ',' + pw_under_voltage_release.Text + ',' + pw_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:22 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 22;
                newMessage.worf = 2;
                newMessage.message = pf_under_voltage_detection.Text + ',' + pf_under_voltage_detection_time.Text + ',' + pf_under_voltage_release.Text + ',' + pf_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:22 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 23;
                newMessage.worf = 1;
                newMessage.message = pw_charge_over_current_detection.Text + ',' + pw_charge_over_current_detection_time.Text + ',' + pw_charge_over_current_release.Text + ',' + pw_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:23 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 23;
                newMessage.worf = 2;
                newMessage.message = pf_charge_over_current_detection.Text + ',' + pf_charge_over_current_detection_time.Text + ',' + pf_charge_over_current_release.Text + ',' + pf_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:23 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 24;
                newMessage.worf = 1;
                newMessage.message = pw_discharge_over_current_detection.Text + ',' + pw_discharge_over_current_detection_time.Text + ',' + pw_discharge_over_current_release.Text + ',' + pw_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:24 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 24;
                newMessage.worf = 2;
                newMessage.message = pf_discharge_over_current_detection.Text + ',' + pf_discharge_over_current_detection_time.Text + ',' + pf_discharge_over_current_release.Text + ',' + pf_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:24 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 25;
                newMessage.worf = 1;
                newMessage.message = pw_over_soc_detection.Text + ',' + pw_over_soc_detection_time.Text + ',' + pw_over_soc_release.Text + ',' + pw_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:25 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 25;
                newMessage.worf = 2;
                newMessage.message = pf_over_soc_detection.Text + ',' + pf_over_soc_detection_time.Text + ',' + pf_over_soc_release.Text + ',' + pf_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:25 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 26;
                newMessage.worf = 1;
                newMessage.message = pw_under_soc_detection.Text + ',' + pw_under_soc_detection_time.Text + ',' + pw_under_soc_release.Text + ',' + pw_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:26 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 26;
                newMessage.worf = 2;
                newMessage.message = pf_under_soc_detection.Text + ',' + pf_under_soc_detection_time.Text + ',' + pf_under_soc_release.Text + ',' + pf_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:26 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 27;
                newMessage.worf = 1;
                newMessage.message = pw_under_soh_detection.Text + ',' + pw_under_soh_detection_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:27 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                newMessage.value_number = 28;
                newMessage.worf = 1;
                newMessage.message = cw_over_voltage_detection.Text + ',' + cw_over_voltage_detection_time.Text + ',' + cw_over_voltage_release.Text + ',' + cw_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:28 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 28;
                newMessage.worf = 2;
                newMessage.message = cf_over_voltage_detection.Text + ',' + cf_over_voltage_detection_time.Text + ',' + cf_over_voltage_release.Text + ',' + cf_over_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:28 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 29;
                newMessage.worf = 1;
                newMessage.message = cw_under_voltage_detection.Text + ',' + cw_under_voltage_detection_time.Text + ',' + cw_under_voltage_release.Text + ',' + cw_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:29 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 29;
                newMessage.worf = 2;
                newMessage.message = cf_under_voltage_detection.Text + ',' + cf_under_voltage_detection_time.Text + ',' + cf_under_voltage_release.Text + ',' + cf_under_voltage_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:29 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 30;
                newMessage.worf = 1;
                newMessage.message = cw_charge_over_current_detection.Text + ',' + cw_charge_over_current_detection_time.Text + ',' + cw_charge_over_current_release.Text + ',' + cw_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:30 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 30;
                newMessage.worf = 2;
                newMessage.message = cf_charge_over_current_detection.Text + ',' + cf_charge_over_current_detection_time.Text + ',' + cf_charge_over_current_release.Text + ',' + cf_charge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:30 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 31;
                newMessage.worf = 1;
                newMessage.message = cw_discharge_over_current_detection.Text + ',' + cw_discharge_over_current_detection_time.Text + ',' + cw_discharge_over_current_release.Text + ',' + cw_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:31 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 31;
                newMessage.worf = 2;
                newMessage.message = cf_discharge_over_current_detection.Text + ',' + cf_discharge_over_current_detection_time.Text + ',' + cf_discharge_over_current_release.Text + ',' + cf_discharge_over_current_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:31 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 32;
                newMessage.worf = 1;
                newMessage.message = cw_over_soc_detection.Text + ',' + cw_over_soc_detection_time.Text + ',' + cw_over_soc_release.Text + ',' + cw_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:32 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 32;
                newMessage.worf = 2;
                newMessage.message = cf_over_soc_detection.Text + ',' + cf_over_soc_detection_time.Text + ',' + cf_over_soc_release.Text + ',' + cf_over_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:32 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 33;
                newMessage.worf = 1;
                newMessage.message = cw_under_soc_detection.Text + ',' + cw_under_soc_detection_time.Text + ',' + cw_under_soc_release.Text + ',' + cw_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:33 1 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);
                newMessage.value_number = 33;
                newMessage.worf = 2;
                newMessage.message = cf_under_soc_detection.Text + ',' + cf_under_soc_detection_time.Text + ',' + cf_under_soc_release.Text + ',' + cf_under_soc_release_time.Text;
                TraceManager.AddLog("WRITE #write message $value number:33 2 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 13;
                newMessage.worf = 16;
                newMessage.message = setting_bettery_type_cmb.Text;
                TraceManager.AddLog("WRITE #write message $value number:13 16 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 14;
                newMessage.worf = 17;
                newMessage.message = setting_current_direction_cmb.Text;
                TraceManager.AddLog("WRITE #write message $value number:14 17 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                newMessage.value_number = 15;
                newMessage.worf = 18;
                newMessage.message = setting_current_sensor_cmb.Text;
                TraceManager.AddLog("WRITE #write message $value number:15 18 @message:" + newMessage.message);
                send(newMessage);
                Thread.Sleep(10);

                if (data_save_command.Checked) {
                    newMessage.value_number = 0x22;
                    newMessage.worf = 0x2A;
                    newMessage.message = "0xFF";
                    TraceManager.AddLog("WRITE #write message $value number:34 0x2A @message:" + newMessage.message);
                    send(newMessage);
                    Thread.Sleep(10);
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
            return;
        }

        private async void download_to_bms_btn_Click(object sender, EventArgs e) {
            try {
                if (connect_state.Text == "Connected - CAN") {
                    await download_bms( );
                } else if (connect_state.Text == "Connected - RS232C") {
                    await rsdownload_bms( );
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private async void display_setting_value( ) {
            try {
                setting_battery_capacity.Text = Members.batterycapacityvaluesetting;
                setting_cell_balancing_start_v.Text = Members.cellbalancingstartvsetting;
                setting_cell_life_cycle.Text = Members.celllifecyclesetting;
                setting_number_of_cell.Text = Members.numberofcell;

                string[ ] buffer = Members.povervoltage[0].Split(',');
                pw_over_voltage_detection.Text = buffer[0];
                pw_over_voltage_detection_time.Text = buffer[1];
                pw_over_voltage_release.Text = buffer[2];
                pw_over_voltage_release_time.Text = buffer[3];
                buffer = Members.povervoltage[1].Split(',');
                pf_over_voltage_detection.Text = buffer[0];
                pf_over_voltage_detection_time.Text = buffer[1];
                pf_over_voltage_release.Text = buffer[2];
                pf_over_voltage_release_time.Text = buffer[3];

                buffer = Members.pundervoltage[0].Split(',');
                pw_under_voltage_detection.Text = buffer[0];
                pw_under_voltage_detection_time.Text = buffer[1];
                pw_under_voltage_release.Text = buffer[2];
                pw_under_voltage_release_time.Text = buffer[3];
                buffer = Members.pundervoltage[1].Split(',');
                pf_under_voltage_detection.Text = buffer[0];
                pf_under_voltage_detection_time.Text = buffer[1];
                pf_under_voltage_release.Text = buffer[2];
                pf_under_voltage_release_time.Text = buffer[3];

                buffer = Members.pchargeovercurrent[0].Split(',');
                pw_charge_over_current_detection.Text = buffer[0];
                pw_charge_over_current_detection_time.Text = buffer[1];
                pw_charge_over_current_release.Text = buffer[2];
                pw_charge_over_current_release_time.Text = buffer[3];
                buffer = Members.pchargeovercurrent[1].Split(',');
                pf_charge_over_current_detection.Text = buffer[0];
                pf_charge_over_current_detection_time.Text = buffer[1];
                pf_charge_over_current_release.Text = buffer[2];
                pf_charge_over_current_release_time.Text = buffer[3];

                buffer = Members.pdischargeovercurrent[0].Split(',');
                pw_discharge_over_current_detection.Text = buffer[0];
                pw_discharge_over_current_detection_time.Text = buffer[1];
                pw_discharge_over_current_release.Text = buffer[2];
                pw_discharge_over_current_release_time.Text = buffer[3];
                buffer = Members.pdischargeovercurrent[1].Split(',');
                pf_discharge_over_current_detection.Text = buffer[0];
                pf_discharge_over_current_detection_time.Text = buffer[1];
                pf_discharge_over_current_release.Text = buffer[2];
                pf_discharge_over_current_release_time.Text = buffer[3];

                buffer = Members.poversoc[0].Split(',');
                pw_over_soc_detection.Text = buffer[0];
                pw_over_soc_detection_time.Text = buffer[1];
                pw_over_soc_release.Text = buffer[2];
                pw_over_soc_release_time.Text = buffer[3];
                buffer = Members.poversoc[1].Split(',');
                pf_over_soc_detection.Text = buffer[0];
                pf_over_soc_detection_time.Text = buffer[1];
                pf_over_soc_release.Text = buffer[2];
                pf_over_soc_release_time.Text = buffer[3];

                buffer = Members.pundersoc[0].Split(',');
                pw_under_soc_detection.Text = buffer[0];
                pw_under_soc_detection_time.Text = buffer[1];
                pw_under_soc_release.Text = buffer[2];
                pw_under_soc_release_time.Text = buffer[3];
                buffer = Members.pundersoc[1].Split(',');
                pf_under_soc_detection.Text = buffer[0];
                pf_under_soc_detection_time.Text = buffer[1];
                pf_under_soc_release.Text = buffer[2];
                pf_under_soc_release_time.Text = buffer[3];

                buffer = Members.pundersoh[0].Split(',');
                pw_under_soh_detection.Text = buffer[0];
                pw_under_soh_detection_time.Text = buffer[1];



                buffer = Members.covervoltage[0].Split(',');
                cw_over_voltage_detection.Text = buffer[0];
                cw_over_voltage_detection_time.Text = buffer[1];
                cw_over_voltage_release.Text = buffer[2];
                cw_over_voltage_release_time.Text = buffer[3];
                buffer = Members.covervoltage[1].Split(',');
                cf_over_voltage_detection.Text = buffer[0];
                cf_over_voltage_detection_time.Text = buffer[1];
                cf_over_voltage_release.Text = buffer[2];
                cf_over_voltage_release_time.Text = buffer[3];

                buffer = Members.cundervoltage[0].Split(',');
                cw_under_voltage_detection.Text = buffer[0];
                cw_under_voltage_detection_time.Text = buffer[1];
                cw_under_voltage_release.Text = buffer[2];
                cw_under_voltage_release_time.Text = buffer[3];
                buffer = Members.cundervoltage[1].Split(',');
                cf_under_voltage_detection.Text = buffer[0];
                cf_under_voltage_detection_time.Text = buffer[1];
                cf_under_voltage_release.Text = buffer[2];
                cf_under_voltage_release_time.Text = buffer[3];

                buffer = Members.cdeviationvoltage[0].Split(',');
                cw_charge_over_current_detection.Text = buffer[0];
                cw_charge_over_current_detection_time.Text = buffer[1];
                cw_charge_over_current_release.Text = buffer[2];
                cw_charge_over_current_release_time.Text = buffer[3];
                buffer = Members.cdeviationvoltage[1].Split(',');
                cf_charge_over_current_detection.Text = buffer[0];
                cf_charge_over_current_detection_time.Text = buffer[1];
                cf_charge_over_current_release.Text = buffer[2];
                cf_charge_over_current_release_time.Text = buffer[3];

                buffer = Members.covertemperature[0].Split(',');
                cw_discharge_over_current_detection.Text = buffer[0];
                cw_discharge_over_current_detection_time.Text = buffer[1];
                cw_discharge_over_current_release.Text = buffer[2];
                cw_discharge_over_current_release_time.Text = buffer[3];
                buffer = Members.covertemperature[1].Split(',');
                cf_discharge_over_current_detection.Text = buffer[0];
                cf_discharge_over_current_detection_time.Text = buffer[1];
                cf_discharge_over_current_release.Text = buffer[2];
                cf_discharge_over_current_release_time.Text = buffer[3];

                buffer = Members.cundertemperature[0].Split(',');
                cw_over_soc_detection.Text = buffer[0];
                cw_over_soc_detection_time.Text = buffer[1];
                cw_over_soc_release.Text = buffer[2];
                cw_over_soc_release_time.Text = buffer[3];
                buffer = Members.cundertemperature[1].Split(',');
                cf_over_soc_detection.Text = buffer[0];
                cf_over_soc_detection_time.Text = buffer[1];
                cf_over_soc_release.Text = buffer[2];
                cf_over_soc_release_time.Text = buffer[3];

                buffer = Members.cdeviationtemperature[0].Split(',');
                cw_under_soc_detection.Text = buffer[0];
                cw_under_soc_detection_time.Text = buffer[1];
                cw_under_soc_release.Text = buffer[2];
                cw_under_soc_release_time.Text = buffer[3];
                buffer = Members.cdeviationtemperature[1].Split(',');
                cf_under_soc_detection.Text = buffer[0];
                cf_under_soc_detection_time.Text = buffer[1];
                cf_under_soc_release.Text = buffer[2];
                cf_under_soc_release_time.Text = buffer[3];

                setting_current_sensor_cmb.Text = Members.current_sensor_type;
                setting_current_direction_cmb.Text = Members.current_direction;
                setting_bettery_type_cmb.Text = Members.battery_type;

                string bin = Convert.ToString(Members.check_enable_buffer, 2).PadLeft(16, '0');
                p_overvoltage_ch.Checked = bin[15] == '1';
                p_undervoltage_ch.Checked = bin[14] == '1';
                p_chargeovercurrent_ch.Checked = bin[13] == '1';
                p_dischargeovercurrent_ch.Checked = bin[12] == '1';
                p_oversoc_ch.Checked = bin[11] == '1';
                p_undersoc_ch.Checked = bin[10] == '1';
                p_undersoh_ch.Checked = bin[9] == '1';
                checkBox12.Checked = bin[7] == '1';
                checkBox11.Checked = bin[6] == '1';
                checkBox10.Checked = bin[5] == '1';
                checkBox9.Checked = bin[3] == '1';
                checkBox8.Checked = bin[2] == '1';
                checkBox7.Checked = bin[1] == '1';
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void setting_value_buffer( ) {
            while (!Members.display_setting) {
                Thread.Sleep(50);
            }
            Thread.Sleep(50);
            Members.display_setting = false;
            display_setting_value( );
            return;
        }

        private async void read_from_bms_btn_Click(object sender, EventArgs e) {
            try {
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = protocol.VALUE_SETTING;
                newMessage.value_number = 0x07;
                newMessage.worf = 0x1A;
                newMessage.message = "0xAA";
                Connection.reset( );
                Connection.write_message(newMessage);
                Thread buffer = new Thread(( ) => setting_value_buffer( ));
                buffer.IsBackground = true;
                buffer.Start( );
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void p_overvoltage_ch_CheckedChanged(object sender, EventArgs e) {
            if (p_overvoltage_ch.Checked) {
                check_enable += 0x0001;
            } else {
                check_enable -= 0x0001;
            }
            if (p_overvoltage_ch.Checked && p_undervoltage_ch.Checked &&
                p_chargeovercurrent_ch.Checked && p_dischargeovercurrent_ch.Checked &&
                p_oversoc_ch.Checked && p_undersoc_ch.Checked && p_undersoh_ch.Checked) {
                checkBox2.Checked = true;
            } else {
                checkBox2.Checked = false;
            }
        }

        private void p_undervoltage_ch_CheckedChanged(object sender, EventArgs e) {
            if (p_undervoltage_ch.Checked) {
                check_enable += 0x0002;
            } else {
                check_enable -= 0x0002;
            }
            if (p_overvoltage_ch.Checked && p_undervoltage_ch.Checked &&
                p_chargeovercurrent_ch.Checked && p_dischargeovercurrent_ch.Checked &&
                p_oversoc_ch.Checked && p_undersoc_ch.Checked && p_undersoh_ch.Checked) {
                checkBox2.Checked = true;
            } else {
                checkBox2.Checked = false;
            }
        }

        private void p_chargeovercurrent_ch_CheckedChanged(object sender, EventArgs e) {
            if (p_chargeovercurrent_ch.Checked) {
                check_enable += 0x0004;
            } else {
                check_enable -= 0x0004;
            }
            if (p_overvoltage_ch.Checked && p_undervoltage_ch.Checked &&
                p_chargeovercurrent_ch.Checked && p_dischargeovercurrent_ch.Checked &&
                p_oversoc_ch.Checked && p_undersoc_ch.Checked && p_undersoh_ch.Checked) {
                checkBox2.Checked = true;
            } else {
                checkBox2.Checked = false;
            }
        }

        private void p_dischargeovercurrent_ch_CheckedChanged(object sender, EventArgs e) {
            if (p_dischargeovercurrent_ch.Checked) {
                check_enable += 0x0008;
            } else {
                check_enable -= 0x0008;
            }
            if (p_overvoltage_ch.Checked && p_undervoltage_ch.Checked &&
                p_chargeovercurrent_ch.Checked && p_dischargeovercurrent_ch.Checked &&
                p_oversoc_ch.Checked && p_undersoc_ch.Checked && p_undersoh_ch.Checked) {
                checkBox2.Checked = true;
            } else {
                checkBox2.Checked = false;
            }
        }

        private void p_oversoc_ch_CheckedChanged(object sender, EventArgs e) {
            if (p_oversoc_ch.Checked) {
                check_enable += 0x0010;
            } else {
                check_enable -= 0x0010;
            }
            if (p_overvoltage_ch.Checked && p_undervoltage_ch.Checked &&
                p_chargeovercurrent_ch.Checked && p_dischargeovercurrent_ch.Checked &&
                p_oversoc_ch.Checked && p_undersoc_ch.Checked && p_undersoh_ch.Checked) {
                checkBox2.Checked = true;
            } else {
                checkBox2.Checked = false;
            }
        }

        private void p_undersoc_ch_CheckedChanged(object sender, EventArgs e) {
            if (p_undersoc_ch.Checked) {
                check_enable += 0x0020;
            } else {
                check_enable -= 0x0020;
            }
            if (p_overvoltage_ch.Checked && p_undervoltage_ch.Checked &&
                p_chargeovercurrent_ch.Checked && p_dischargeovercurrent_ch.Checked &&
                p_oversoc_ch.Checked && p_undersoc_ch.Checked && p_undersoh_ch.Checked) {
                checkBox2.Checked = true;
            } else {
                checkBox2.Checked = false;
            }
        }

        private void p_undersoh_ch_CheckedChanged(object sender, EventArgs e) {
            if (p_undersoh_ch.Checked) {
                check_enable += 0x0040;
            } else {
                check_enable -= 0x0040;
            }
            if (p_overvoltage_ch.Checked && p_undervoltage_ch.Checked &&
                p_chargeovercurrent_ch.Checked && p_dischargeovercurrent_ch.Checked &&
                p_oversoc_ch.Checked && p_undersoc_ch.Checked && p_undersoh_ch.Checked) {
                checkBox2.Checked = true;
            } else {
                checkBox2.Checked = false;
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e) {
            if (checkBox12.Checked) {
                check_enable += 0x0100;
            } else {
                check_enable -= 0x0100;
            }
            if (checkBox12.Checked && checkBox11.Checked &&
                checkBox10.Checked && checkBox9.Checked &&
                checkBox8.Checked && checkBox7.Checked) {
                checkBox1.Checked = true;
            } else {
                checkBox1.Checked = false;
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e) {
            if (checkBox11.Checked) {
                check_enable += 0x0200;
            } else {
                check_enable -= 0x0200;
            }
            if (checkBox12.Checked && checkBox11.Checked &&
                checkBox10.Checked && checkBox9.Checked &&
                checkBox8.Checked && checkBox7.Checked) {
                checkBox1.Checked = true;
            } else {
                checkBox1.Checked = false;
            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e) {
            if (checkBox10.Checked) {
                check_enable += 0x0400;
            } else {
                check_enable -= 0x0400;
            }
            if (checkBox12.Checked && checkBox11.Checked &&
                checkBox10.Checked && checkBox9.Checked &&
                checkBox8.Checked && checkBox7.Checked) {
                checkBox1.Checked = true;
            } else {
                checkBox1.Checked = false;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e) {
            if (checkBox9.Checked) {
                check_enable += 0x1000;
            } else {
                check_enable -= 0x1000;
            }
            if (checkBox12.Checked && checkBox11.Checked &&
                checkBox10.Checked && checkBox9.Checked &&
                checkBox8.Checked && checkBox7.Checked) {
                checkBox1.Checked = true;
            } else {
                checkBox1.Checked = false;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e) {
            if (checkBox8.Checked) {
                check_enable += 0x2000;
            } else {
                check_enable -= 0x2000;
            }
            if (checkBox12.Checked && checkBox11.Checked &&
                checkBox10.Checked && checkBox9.Checked &&
                checkBox8.Checked && checkBox7.Checked) {
                checkBox1.Checked = true;
            } else {
                checkBox1.Checked = false;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e) {
            if (checkBox7.Checked) {
                check_enable += 0x4000;
            } else {
                check_enable -= 0x4000;
            }
            if (checkBox12.Checked && checkBox11.Checked &&
                checkBox10.Checked && checkBox9.Checked &&
                checkBox8.Checked && checkBox7.Checked) {
                checkBox1.Checked = true;
            } else {
                checkBox1.Checked = false;
            }
        }

        private void setting_soc_value_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && !setting_soc_value.Text.Contains('.') && setting_soc_value.Text != string.Empty) {
                setting_soc_value.Text += ".0";
            }
        }

        private void setting_soh_value_TextChanged(object sender, EventArgs e) {

        }

        private void setting_soh_value_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && !setting_soh_value.Text.Contains('.') && setting_soh_value.Text != string.Empty) {
                setting_soc_value.Text += ".0";
            }
        }

        private void setting_soh_value_KeyDown(object sender, KeyPressEventArgs e) {

        }

        private void pw_discharge_over_current_detection_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && pw_discharge_over_current_detection.Text != string.Empty && !pw_discharge_over_current_detection.Text.Contains('-')) {
                pw_discharge_over_current_detection.Text = "-" + pw_discharge_over_current_detection.Text;
            }
            if (e.KeyCode == Keys.Enter) {
                SendKeys.Send("{TAB}");
            }
        }

        private void pw_discharge_over_current_release_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && pw_discharge_over_current_release.Text != string.Empty && !pw_discharge_over_current_release.Text.Contains('-')) {
                pw_discharge_over_current_release.Text = "-" + pw_discharge_over_current_release.Text;
            }
            if (e.KeyCode == Keys.Enter) {
                SendKeys.Send("{TAB}");
            }
        }

        private void pf_discharge_over_current_detection_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && pf_discharge_over_current_detection.Text != string.Empty && !pf_discharge_over_current_detection.Text.Contains('-')) {
                pf_discharge_over_current_detection.Text = "-" + pf_discharge_over_current_detection.Text;
            }
            if (e.KeyCode == Keys.Enter) {
                SendKeys.Send("{TAB}");
            }
        }

        private void pf_discharge_over_current_release_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && pf_discharge_over_current_release.Text != string.Empty && !pf_discharge_over_current_release.Text.Contains('-')) {
                pf_discharge_over_current_release.Text = "-" + pf_discharge_over_current_release.Text;
            }
            if (e.KeyCode == Keys.Enter) {
                SendKeys.Send("{TAB}");
            }
        }

        private void main_Load(object sender, EventArgs e) {
            start_setting( );
        }

        private void Com_start_btn_Click(object sender, EventArgs e) {
            try {
                if (is_connected) {
                    is_communicate = true;
                    timer.Interval = 1000;
                    timer_display.Interval = 500;
                    if (connect_state.Text == "Connected - CAN") {
                        data_receive.Interval = 25;
                        data_receive.Enabled = true;
                    }
                    timer.Enabled = timer_display.Enabled = true;
                } else {
                    MessageBox.Show("The Communication is not Initialized", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void Com_stop_btn_Click(object sender, EventArgs e) {
            try {
                is_communicate = false;
                timer.Enabled = timer_display.Enabled = false;
                Com_start_btn.BackColor = Color.White;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void setting_battery_capacity_TextChanged(object sender, EventArgs e) {

        }

        private void setting_number_of_cell_TextChanged(object sender, EventArgs e) {

        }

        private void setting_cell_life_cycle_TextChanged(object sender, EventArgs e) {

        }

        private void setting_cell_balancing_start_v_TextChanged(object sender, EventArgs e) {

        }

        private void cw_under_soc_detection_TextChanged(object sender, EventArgs e) {

        }

        private void timer_sec_Tick(object sender, EventArgs e) {
            value_display( );
        }

        private void panel10_Paint(object sender, PaintEventArgs e) {

        }

        private void label212_Click(object sender, EventArgs e) {

        }

        private async void timer1_Tick(object sender, EventArgs e) {
            try {
                Connection.read_message( );
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void bufferfunc( ) {
            try {
                while (Members.logdata.is_while) {
                    while (!Members.logdata.nextline)
                        ;
                    set_log_Data( );
                    Members.logdata.nextline = false;
                }
                return;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
                return;
            }
        }

        private void log_data_read_btn_Click(object sender, EventArgs e) {
            try {
                Members.logdata.is_while = true;

                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = protocol.VALUE_SETTING;
                newMessage.value_number = 0x0A;
                newMessage.worf = 0x1D;
                newMessage.message = "0xFF";
                Connection.write_message(newMessage);

                Thread buffer = new Thread(( ) => bufferfunc( ));
                buffer.IsBackground = true;
                buffer.Start( );
                return;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void log_data_clear_Click(object sender, EventArgs e) {
            try {
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = protocol.VALUE_SETTING;
                newMessage.value_number = 0x09;
                newMessage.worf = 0x1C;
                newMessage.message = "0xA0";
                Connection.write_message(newMessage);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        public void set_log_Data( ) {
            try {
                log_data.Rows.Add(Members.logdata.logdatanumber, Members.logdata.logdatacount, Members.logdata.packvoltage, Members.logdata.packcurrent, Members.logdata.packsoc
                , Members.logdata.maxcellvoltage, Members.logdata.mincellvoltage, Members.logdata.averagevoltage, Members.logdata.differencecellvoltage
                , Members.logdata.maxcelltemperature, Members.logdata.mincelltemperature, Members.logdata.averagetemperature, Members.logdata.differencetemperature
                , Members.logdata.chargedischargecount
                , "0x" + Members.logdata.status_c.ToString("X")
                , "0x" + Members.logdata.status_p.ToString("X")
                , Members.logdata.lifecycle);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private async void log_data_read_Tick(object sender, EventArgs e) {

        }

        private bool save_logdata(string path) {
            try {
                string delimiter = ",";
                FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                StreamWriter csvExport = new StreamWriter(fs, System.Text.Encoding.UTF8);

                if (log_data.Rows.Count == 0) {
                    MessageBox.Show("The logdata grid is empty !", "ERROR !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TraceManager.AddLog("ERROR   #logdata save to file $The logdata grid is empty");
                    return false;
                } else {
                    for (int i = 0; i < log_data.Columns.Count; ++i) {
                        csvExport.Write(log_data.Columns[i].HeaderText);
                        if (i != log_data.Columns.Count - 1)
                            csvExport.Write(delimiter);
                    }
                }
                csvExport.Write(csvExport.NewLine);

                foreach (DataGridViewRow row in log_data.Rows)
                    if (!row.IsNewRow) {
                        for (int i = 0; i < log_data.Columns.Count; ++i) {
                            csvExport.Write(row.Cells[i].Value);
                            if (i != log_data.Columns.Count - 1)
                                csvExport.Write(delimiter);
                        }
                        csvExport.Write(csvExport.NewLine);
                    }

                csvExport.Flush( );
                csvExport.Close( );
                fs.Close( );

                return true;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
                return false;
            }
        }

        private void logdata_savetofile_Click(object sender, EventArgs e) {
            try {
                string file_path;
                SaveFileDialog openFileDialog = new SaveFileDialog( );
                if (Properties.Settings.Default.save_file_path == string.Empty) {
                    Properties.Settings.Default.save_file_path = Directory.GetCurrentDirectory( );
                }
                openFileDialog.InitialDirectory = Properties.Settings.Default.save_file_path;
                openFileDialog.Title = "To Save file location";
                openFileDialog.DefaultExt = "csv";
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                openFileDialog.ShowDialog( );
                if (openFileDialog.FileName.Length <= 0) {
                    throw new Exception("Failed to retrieve file location or lenth is 0");
                } else {
                    file_path = openFileDialog.FileName;
                }
                if (save_logdata(file_path)) {
                    TraceManager.AddLog("LOGSAVE #logdata save file @saved to : " + file_path);
                    MessageBox.Show("Successfully save the file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Properties.Settings.Default.save_file_path = file_path;
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void capacity_reset_btn_Click(object sender, EventArgs e) {
            try {
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = protocol.VALUE_SETTING;
                newMessage.value_number = 0x08;
                newMessage.worf = 0x1B;
                newMessage.message = "0x0A";
                Connection.write_message(newMessage);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void log_Data_clear_btn_Click(object sender, EventArgs e) {
            try {
                setting_data_set newMessage = new setting_data_set( );
                newMessage.ID = protocol.VALUE_SETTING;
                newMessage.value_number = 0x09;
                newMessage.worf = 0x1C;
                newMessage.message = "0xA0";
                Connection.write_message(newMessage);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {

        }

        private void log_data_display_clear_Click(object sender, EventArgs e) {
            log_data.Rows.Clear( );
        }

        private void pw_over_voltage_detection_TextChanged(object sender, EventArgs e) {

        }

        private void pw_over_voltage_detection_KeyDown(object sender, KeyEventArgs e) {

        }

        private void pw_under_soc_detection_TextChanged(object sender, EventArgs e) {

        }

        private void pw_over_voltage_detection_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\n' || e.KeyChar == '\r') {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void pw_under_voltage_detection_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\n' || e.KeyChar == '\r') {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
        }

        private void checkBox2_CheckedChanged(object sender, MouseEventArgs e) {
            bool D;
            D = checkBox2.Checked;

            p_overvoltage_ch.Checked = D;
            p_undervoltage_ch.Checked = D;
            p_chargeovercurrent_ch.Checked = D;
            p_dischargeovercurrent_ch.Checked = D;
            p_oversoc_ch.Checked = D;
            p_undersoc_ch.Checked = D;
            p_undersoh_ch.Checked = D;
        }

        private void checkBox1_CheckedChanged(object sender, MouseEventArgs e) {
            bool D;
            D = checkBox1.Checked;

            checkBox12.Checked = D;
            checkBox11.Checked = D;
            checkBox10.Checked = D;
            checkBox9.Checked = D;
            checkBox8.Checked = D;
            checkBox7.Checked = D;
        }

        private void pw_discharge_over_current_detection_TextChanged(object sender, EventArgs e) {

        }

        private void pw_discharge_over_current_detection_KeyDown(object sender, KeyPressEventArgs e) {
            if ((e.KeyChar == '\n' || e.KeyChar == '\r')) {
                e.Handled = true;
                SendKeys.Send("{TAB}");

                if (pw_discharge_over_current_detection.Text != string.Empty && !pw_discharge_over_current_detection.Text.Contains('-'))
                    pw_discharge_over_current_detection.Text = "-" + pw_discharge_over_current_detection.Text;
            }
        }

        private void pw_discharge_over_current_release_KeyDown(object sender, KeyPressEventArgs e) {
            if ((e.KeyChar == '\n' || e.KeyChar == '\r')) {
                e.Handled = true;
                SendKeys.Send("{TAB}");

                if (pw_discharge_over_current_release.Text != string.Empty && !pw_discharge_over_current_release.Text.Contains('-'))
                    pw_discharge_over_current_release.Text = "-" + pw_discharge_over_current_release.Text;
            }
        }

        private void pf_discharge_over_current_detection_KeyDown(object sender, KeyPressEventArgs e) {
            if ((e.KeyChar == '\n' || e.KeyChar == '\r')) {
                e.Handled = true;
                SendKeys.Send("{TAB}");

                if (pf_discharge_over_current_detection.Text != string.Empty && !pf_discharge_over_current_detection.Text.Contains('-'))
                    pf_discharge_over_current_detection.Text = "-" + pf_discharge_over_current_detection.Text;
            }
        }

        private void pf_discharge_over_current_release_KeyDown(object sender, KeyPressEventArgs e) {
            if ((e.KeyChar == '\n' || e.KeyChar == '\r')) {
                e.Handled = true;
                SendKeys.Send("{TAB}");

                if (pf_discharge_over_current_release.Text != string.Empty && !pf_discharge_over_current_release.Text.Contains('-'))
                    pf_discharge_over_current_release.Text = "-" + pf_discharge_over_current_release.Text;
            }

        }

        private void label85_Click(object sender, EventArgs e) {

        }

        private void label87_Click(object sender, EventArgs e) {

        }

        private void label89_Click(object sender, EventArgs e) {

        }

        private void label91_Click(object sender, EventArgs e) {

        }

        private Object thislock = new Object( );

        const char _02 = (char)0x02;
        const char _0D = (char)0x0D;
        const char _0A = (char)0x0A;

        string inStream = string.Empty;

        private void rsport_data_receive( ) {
            try {
                byte[ ] datas = new byte[13];
                int size = rsport.BytesToRead;
                if (size > 1) {
                    rsport.Read(datas, 0, 13);
                }

                string str = string.Empty;
                for (int i = 0; i < datas.Length; ++i) {
                    str += datas[i].ToString("x") + " ";
                }
                TraceManager.AddLog("rs232c data receive : [" + str + "]");

                TPCANMsg buffer = new TPCANMsg( );
                int abuff = Convert.ToInt32((datas[2].ToString("x") + datas[1].ToString("x")).ToString( ), 16);
                buffer.ID = (uint)abuff;
                TraceManager.AddLog("rs232c ID : [" + buffer.ID.ToString( ) + "]");
                buffer.LEN = 8;
                for (int i = 4; i <= 11; ++i) {
                    buffer.DATA[i - 4] = datas[i];
                }

                string sstr = string.Empty;
                foreach (byte i in buffer.DATA) {
                    sstr += i + " ";
                }
                TraceManager.AddLog("rs232c Data Converted to [" + sstr + "]");

                Connection.process_message(buffer);
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
            return;
        }

        private void rsport_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            //when data receive on rs232c
            try {
                TraceManager.AddLog("rs232c receive log");
                Thread buffer = new Thread(( ) => rsport_data_receive( ));
                buffer.IsBackground = true;
                buffer.Start( );
            } catch (Exception ex) {
                TraceManager.AddLog("ERROR   #Exception  $" + ex.Message + "@" + ex.StackTrace);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox7.Text == "1118") {
                setting_tab.Enabled = true;
                tab_control.TabPages.Insert(2, setting_tab);
                tab_control.SelectedIndex = 2;
                tab_control.TabPages.Remove(login_tab);
                textBox7.Text = string.Empty;
            } else {
                MessageBox.Show("Incorrect password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e) {

        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                button1_Click(sender, e);
                return;
            }
        }

        private void tab_control_SelectedIndexChanged(object sender, EventArgs e) {
            if (tab_control.SelectedIndex != 2 && setting_tab.Enabled) {
                tab_control.TabPages.Insert(2, login_tab);
                tab_control.TabPages.Remove(setting_tab);
                setting_tab.Enabled = false;
            }
            if (tab_control.SelectedTab == login_tab)
                textBox7.Focus( );

            Connection.reset( );
            if (tab_control.SelectedIndex == 0 && is_communicate)
                timer.Enabled = true;
            else
                timer.Enabled = false;
        }
    }
}