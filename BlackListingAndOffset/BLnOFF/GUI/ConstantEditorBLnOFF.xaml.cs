using System;
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

namespace ZOT.BLnOFF.GUI
{ 
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class ConstantEditorBLnOFF : Window
    {

        public ConstantEditorBLnOFF()
        {
            InitializeComponent();
            BL_MAX_DIST.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_DIST.ToString();
            BL_MAX_DIST_2600.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_DIST_2600.ToString();
            BL_MAX_DIST_2100.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_DIST_2100.ToString();
            BL_MIN_SUCCESS_HANDOVER.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MIN_SUCCESS_HANDOVER.ToString();
            BL_MAX_PER_COLIN_800.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_800.ToString();
            BL_MAX_PER_COLIN_1800.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_1800.ToString();
            BL_MAX_PER_COLIN_2600.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_2600.ToString();
            BL_MAX_PER_COLIN_2100.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_2100.ToString();
            BL_MAX_ABS_COLIN_800.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_800.ToString();
            BL_MAX_ABS_COLIN_1800.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_1800.ToString();
            BL_MAX_ABS_COLIN_2600.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_2600.ToString();
            BL_MAX_ABS_COLIN_2100.Text = ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_2100.ToString();

            OFF_PERCENTILE_CELL_RANGE.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.PERCENTILE_CELL_RANGE.ToString();
            OFF_MIN_SUCCESS_HANDOVER.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MIN_SUCCESS_HANDOVER.ToString();
            OFF_MAX_PER_COLIN_800.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_800.ToString();
            OFF_MAX_PER_COLIN_1800.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_1800.ToString();
            OFF_MAX_PER_COLIN_2600.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_2600.ToString();
            OFF_MAX_PER_COLIN_2100.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_2100.ToString();
            OFF_MAX_ABS_COLIN_800.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_800.ToString();
            OFF_MAX_ABS_COLIN_1800.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_1800.ToString();
            OFF_MAX_ABS_COLIN_2600.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_2600.ToString();
            OFF_MAX_ABS_COLIN_2100.Text = ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_2100.ToString();

            U_INTER_PER_800.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_800.ToString();
            U_INTER_PER_1800.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_1800.ToString();
            U_INTER_PER_2600.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_2600.ToString();
            U_INTER_PER_2100.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_2100.ToString();

            U_INTRA_PER_800.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_800.ToString();
            U_INTRA_PER_1800.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_1800.ToString();
            U_INTRA_PER_2600.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_2600.ToString();
            U_INTRA_PER_2100.Text = ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_2100.ToString();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_DIST = Convert.ToDouble(BL_MAX_DIST.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_DIST_2600 = Convert.ToDouble(BL_MAX_DIST_2600.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_DIST_2100 = Convert.ToDouble(BL_MAX_DIST_2100.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MIN_SUCCESS_HANDOVER = Convert.ToDouble(BL_MIN_SUCCESS_HANDOVER.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_800 = Convert.ToDouble(BL_MAX_PER_COLIN_800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_1800 = Convert.ToDouble(BL_MAX_PER_COLIN_1800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_2600 = Convert.ToDouble(BL_MAX_PER_COLIN_2600.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_PER_COLIN_2100 = Convert.ToDouble(BL_MAX_PER_COLIN_2100.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_800 = Convert.ToDouble(BL_MAX_ABS_COLIN_800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_1800 = Convert.ToDouble(BL_MAX_ABS_COLIN_1800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_2600 = Convert.ToDouble(BL_MAX_ABS_COLIN_2600.Text);
            ZOT.BlnOFF.Code.CONSTANTS.BL.MAX_COLIN_2100 = Convert.ToDouble(BL_MAX_ABS_COLIN_2100.Text);

            ZOT.BlnOFF.Code.CONSTANTS.OFF.PERCENTILE_CELL_RANGE = Convert.ToDouble(OFF_PERCENTILE_CELL_RANGE.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MIN_SUCCESS_HANDOVER = Convert.ToDouble(OFF_MIN_SUCCESS_HANDOVER.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_800 = Convert.ToDouble(OFF_MAX_PER_COLIN_800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_1800 = Convert.ToDouble(OFF_MAX_PER_COLIN_1800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_2600 = Convert.ToDouble(OFF_MAX_PER_COLIN_2600.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_PER_COLIN_2100 = Convert.ToDouble(OFF_MAX_PER_COLIN_2100.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_800 = Convert.ToDouble(OFF_MAX_ABS_COLIN_800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_1800 = Convert.ToDouble(OFF_MAX_ABS_COLIN_1800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_2600 = Convert.ToDouble(OFF_MAX_ABS_COLIN_2600.Text);
            ZOT.BlnOFF.Code.CONSTANTS.OFF.MAX_COLIN_2100 = Convert.ToDouble(OFF_MAX_ABS_COLIN_2100.Text);

            ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_800 = Convert.ToDouble(U_INTER_PER_800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_1800 = Convert.ToDouble(U_INTER_PER_1800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_2600 = Convert.ToDouble(U_INTER_PER_2600.Text);
            ZOT.BlnOFF.Code.CONSTANTS.U_INTER.PER_2100 = Convert.ToDouble(U_INTER_PER_2100.Text);

            ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_800 = Convert.ToDouble(U_INTRA_PER_800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_1800 = Convert.ToDouble(U_INTRA_PER_1800.Text);
            ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_2600 = Convert.ToDouble(U_INTRA_PER_2600.Text);
            ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.PER_2100 = Convert.ToDouble(U_INTRA_PER_2100.Text);

            ZOT.BlnOFF.Code.CONSTANTS.SaveConst();
            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
