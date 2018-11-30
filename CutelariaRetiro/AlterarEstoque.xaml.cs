using CutelariaRetiro.BLL;
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
using System.Windows.Shapes;

namespace CutelariaRetiro
{
    /// <summary>
    /// Lógica interna para AlterarEstoque.xaml
    /// </summary>
    public partial class AlterarEstoque : Window
    {
        private Material Material { get; set; }
        public AlterarEstoque(Material mat)
        {
            InitializeComponent();

            Material = mat;
            lbNomeMat.Content = mat.Descricao;
            var ultimaEntrada = mat.EntradaMaterial.LastOrDefault();
            if (ultimaEntrada != null)
                lbUltimaEntrada.Content = ultimaEntrada.Data.ToString("dd/MM/yyyy");

            lbEstoqueAtual.Content = $"{mat.Estoque} UN";
            txQuant.Text = "1";
            txQuant.ToNumeric();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if(int.Parse(txQuant.Text) <= 0)
            {
                MessageBox.Show("A quantidade deve ser superior a 0 (zero)",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var bll = new MaterialBLL();
            var mat = bll.Find(Material.Id);
            mat.Estoque = int.Parse(txQuant.Text);
            bll.Save(mat);

            Close();
        }
    }
}
