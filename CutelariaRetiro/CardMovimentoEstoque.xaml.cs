using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CutelariaRetiro
{
    /// <summary>
    /// Interação lógica para CardMovimentoEstoque.xam
    /// </summary>
    public partial class CardMovimentoEstoque : UserControl
    {
        public CardMovimentoEstoque(MovimentoEstoque mov)
        {
            InitializeComponent();

            comboBox.Visibility = Visibility.Hidden;
            lbTipo.Content = mov.Tipo;
            if (mov.Tipo.Equals("Entrada"))
                lbTipo.Foreground = Brushes.Blue;
            else
                lbTipo.Foreground = Brushes.LightSeaGreen;

            lbQuant.Content = $"{mov.Quant} UN";
            lbData.Content = mov.Data.ToString("dd/MM/yyyy");
        }
    }
}
