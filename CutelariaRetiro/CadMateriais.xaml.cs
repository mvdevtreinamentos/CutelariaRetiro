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
    /// Lógica interna para CadMateriais.xaml
    /// </summary>
    public partial class CadMateriais : Window
    {
        public int IdAtual { get; private set; }

        public CadMateriais()
        {
            InitializeComponent();

            btExcluir.Visibility = Visibility.Hidden;
            btSalvar.Visibility = Visibility.Hidden;
            txPreco.ToMoney();
            Buscar();
        }

        private void Buscar()
        {
            List<Material> materiais = new MaterialBLL().Search(txPesquisa.Text);
            listBox.DisplayMemberPath = "Descricao";
            listBox.SelectedValuePath = "Id";
            listBox.ItemsSource = materiais;
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            MaterialBLL bll = new MaterialBLL();

            Material mat = (IdAtual == 0
                ? new Material()
                : bll.Find(IdAtual));
            mat.Referencia = txReferencia.Text;
            mat.Descricao = txDescricao.Text;
            mat.Preco = decimal.Parse(txPreco.Text);
            mat.Inativo = ckInativo.IsChecked.Value;
            bll.Save(mat);

            LimparCampos();

            Buscar();
        }

        private void LimparCampos()
        {
            IdAtual = 0;
            txReferencia.Text = string.Empty;
            txDescricao.Text = string.Empty;
            txPreco.Text = string.Empty;
            ckInativo.IsChecked = false;
            btSalvar.Visibility = Visibility.Hidden;

            btExcluir.Visibility = Visibility.Hidden;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Material mat = (Material)listBox.SelectedItem;
            if (mat == null)
                return;

            FillForm(mat);
        }

        public void FillForm(Material mat)
        {
            IdAtual = mat.Id;
            txDescricao.Text = mat.Descricao;
            txReferencia.Text = mat.Referencia;
            txPreco.Text = mat.Preco.ToString("N2");
            ckInativo.IsChecked = mat.Inativo;
            btSalvar.Visibility = Visibility.Visible;

            if (new MaterialBLL().ExisteMovimentacaoMaterial(mat.Id))
                btExcluir.Visibility = Visibility.Hidden;
            else
                btExcluir.Visibility = Visibility.Visible;
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
            btSalvar.Visibility = Visibility.Visible;
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show($"Confirmar a exclusão do material '{txDescricao.Text}'? \nEsta ação não poderá ser revertida!",
                "Excluir material", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No)
                return;

            MaterialBLL bll = new MaterialBLL();
            bll.Remove(IdAtual);

            LimparCampos();
            Buscar();
        }

        private void txPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            Buscar();
        }
    }
}
