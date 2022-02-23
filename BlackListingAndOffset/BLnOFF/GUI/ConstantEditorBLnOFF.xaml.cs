﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using MahApps.Metro.Controls;
using ZOT.resources;
using ZOT.resources.ZOTlib;

namespace ZOT.BLnOFF.GUI
{ 
    /// <summary>
    /// Contiene un monton de asignaciones entre constantes de la aplicacion y cuadros de texto
    /// </summary>
    public partial class ConstantEditorBLnOFF : MetroWindow
    {

        public ConstantEditorBLnOFF()
        {



            //PARA NUEVAS TECNOLOGIAS se debe añadir en la vista una casilla para cada umbral
            //Luego en el fichero de BLnOFFconstants.txt se pone el valor que se desea leer (en la posicion que toque)
            // PD: Buena suerte al que le toque cambiar esto... porque vas a tener que mirar una a una la posicion que debes añadir






            InitializeComponent();
            BL_MAX_DIST_700.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L700].ToString();
            BL_MAX_DIST_800.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L800].ToString();
            BL_MAX_DIST_1800.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L1800].ToString();
            BL_MAX_DIST_900.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L900].ToString();
            BL_MAX_DIST_2600.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L2600].ToString();
            BL_MAX_DIST_2100.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L2100].ToString();

            BL_MIN_SUCCESS_HANDOVER.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MIN_SUCCESS_HANDOVER.ToString();

            BL_MAX_PER_COLIN_700.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L700].ToString();
            BL_MAX_PER_COLIN_800.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L800].ToString();
            BL_MAX_PER_COLIN_1800.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L1800].ToString();
            BL_MAX_PER_COLIN_2600.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L2600].ToString();
            BL_MAX_PER_COLIN_900.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L900].ToString();
            BL_MAX_PER_COLIN_2100.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L2100].ToString();
            BL_MAX_ABS_COLIN_1800.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L1800].ToString();

            BL_MAX_ABS_COLIN_700.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L700].ToString();
            BL_MAX_ABS_COLIN_800.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L800].ToString();
            BL_MAX_ABS_COLIN_2600.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L2600].ToString();
            BL_MAX_ABS_COLIN_900.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L900].ToString();
            BL_MAX_ABS_COLIN_2100.Text = ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L2100].ToString();


            //_______________________________________________________________________________________________________

            OFF_PERCENTILE_CELL_RANGE.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.PERCENTILE_CELL_RANGE.ToString();
            OFF_MIN_SUCCESS_HANDOVER.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MIN_SUCCESS_HANDOVER.ToString();
            OFF_MAX_PER_COLIN_700.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L700].ToString();
            OFF_MAX_PER_COLIN_800.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L800].ToString();
            OFF_MAX_PER_COLIN_1800.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L1800].ToString();
            OFF_MAX_PER_COLIN_2600.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L2600].ToString();
            OFF_MAX_PER_COLIN_900.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L900].ToString();
            OFF_MAX_PER_COLIN_2100.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L2100].ToString();

            OFF_MAX_ABS_COLIN_700.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L700].ToString();
            OFF_MAX_ABS_COLIN_800.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L800].ToString();
            OFF_MAX_ABS_COLIN_1800.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L1800].ToString();
            OFF_MAX_ABS_COLIN_2600.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L2600].ToString();
            OFF_MAX_ABS_COLIN_900.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L900].ToString();
            OFF_MAX_ABS_COLIN_2100.Text = ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L2100].ToString();



            //_______________________________________________________________________________________________________

            U_INTER_PER_700.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L700].ToString();
            U_INTER_PER_800.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L800].ToString();
            U_INTER_PER_1800.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L1800].ToString();
            U_INTER_PER_2600.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L2600].ToString();
            U_INTER_PER_900.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L900].ToString();
            U_INTER_PER_2100.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L2100].ToString();

            U_INTRA_PER_700.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L700].ToString();
            U_INTRA_PER_800.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L800].ToString();
            U_INTRA_PER_1800.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L1800].ToString();
            U_INTRA_PER_2600.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L2600].ToString();
            U_INTRA_PER_900.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L900].ToString();
            U_INTRA_PER_2100.Text = ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L2100].ToString();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L700] = Convert.ToDouble(BL_MAX_DIST_700.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L800] = Convert.ToDouble(BL_MAX_DIST_800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L1800] = Convert.ToDouble(BL_MAX_DIST_1800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L2600] = Convert.ToDouble(BL_MAX_DIST_2600.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L900] = Convert.ToDouble(BL_MAX_DIST_900.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_DIST[TECH_NUM.L2100] = Convert.ToDouble(BL_MAX_DIST_2100.Text);

            ZOT.BLnOFF.Code.CONSTANTS.BL.MIN_SUCCESS_HANDOVER = Convert.ToDouble(BL_MIN_SUCCESS_HANDOVER.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L700] = Convert.ToDouble(BL_MAX_PER_COLIN_700.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L800] = Convert.ToDouble(BL_MAX_PER_COLIN_800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L1800] = Convert.ToDouble(BL_MAX_PER_COLIN_1800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L2600] = Convert.ToDouble(BL_MAX_PER_COLIN_2600.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L900] = Convert.ToDouble(BL_MAX_PER_COLIN_900.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN[TECH_NUM.L2100] = Convert.ToDouble(BL_MAX_PER_COLIN_2100.Text);

            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L700] = Convert.ToDouble(BL_MAX_ABS_COLIN_700.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L800] = Convert.ToDouble(BL_MAX_ABS_COLIN_800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L1800] = Convert.ToDouble(BL_MAX_ABS_COLIN_1800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L2600] = Convert.ToDouble(BL_MAX_ABS_COLIN_2600.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L900] = Convert.ToDouble(BL_MAX_ABS_COLIN_900.Text);
            ZOT.BLnOFF.Code.CONSTANTS.BL.MAX_COLIN[TECH_NUM.L2100] = Convert.ToDouble(BL_MAX_ABS_COLIN_2100.Text);

            ZOT.BLnOFF.Code.CONSTANTS.OFF.PERCENTILE_CELL_RANGE = Convert.ToDouble(OFF_PERCENTILE_CELL_RANGE.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MIN_SUCCESS_HANDOVER = Convert.ToDouble(OFF_MIN_SUCCESS_HANDOVER.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L700] = Convert.ToDouble(OFF_MAX_PER_COLIN_700.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L800] = Convert.ToDouble(OFF_MAX_PER_COLIN_800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L1800] = Convert.ToDouble(OFF_MAX_PER_COLIN_1800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L2600] = Convert.ToDouble(OFF_MAX_PER_COLIN_2600.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L900] = Convert.ToDouble(OFF_MAX_PER_COLIN_900.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN[TECH_NUM.L2100] = Convert.ToDouble(OFF_MAX_PER_COLIN_2100.Text);

            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L700] = Convert.ToDouble(OFF_MAX_ABS_COLIN_700.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L800] = Convert.ToDouble(OFF_MAX_ABS_COLIN_800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L1800] = Convert.ToDouble(OFF_MAX_ABS_COLIN_1800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L2600] = Convert.ToDouble(OFF_MAX_ABS_COLIN_2600.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L900] = Convert.ToDouble(OFF_MAX_ABS_COLIN_900.Text);
            ZOT.BLnOFF.Code.CONSTANTS.OFF.MAX_COLIN[TECH_NUM.L2100] = Convert.ToDouble(OFF_MAX_ABS_COLIN_2100.Text);

            ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L700] = Convert.ToDouble(U_INTER_PER_700.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L800] = Convert.ToDouble(U_INTER_PER_800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L1800] = Convert.ToDouble(U_INTER_PER_1800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L2600] = Convert.ToDouble(U_INTER_PER_2600.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L900] = Convert.ToDouble(U_INTER_PER_900.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTER.PER[TECH_NUM.L2100] = Convert.ToDouble(U_INTER_PER_2100.Text);

            ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L700] = Convert.ToDouble(U_INTRA_PER_700.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L800] = Convert.ToDouble(U_INTRA_PER_800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L1800] = Convert.ToDouble(U_INTRA_PER_1800.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L2600] = Convert.ToDouble(U_INTRA_PER_2600.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L900] = Convert.ToDouble(U_INTRA_PER_900.Text);
            ZOT.BLnOFF.Code.CONSTANTS.U_INTRA.PER[TECH_NUM.L2100] = Convert.ToDouble(U_INTRA_PER_2100.Text);

            ZOT.BLnOFF.Code.CONSTANTS.SaveConst();
            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
