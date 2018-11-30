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
    /// Lógica interna para CadTipoServico.xaml
    /// </summary>
    public partial class CadTipoServico : Window
    {
        public CadTipoServico()
        {
            InitializeComponent();

            txPreco.ToMoney();
            Buscar();
        }

        private void Buscar()
        {
            TipoServicoBLL bll = new TipoServicoBLL();
            List<TipoServico> list = bll.Search(txPesquisa.Text);
            listBox.DisplayMemberPath = "Nome";
            listBox.ItemsSource = list;
        }

        public void FillForm(TipoServico tipo)
        {
            IdAtual = tipo.Id;
            txCodigo.Text = tipo.Id.ToString();
            txNome.Text = tipo.Nome;
            txPreco.Text = tipo.Preco.ToString("N2");
            txCodigo.IsEnabled = false;
        }

        private int IdAtual { get; set; }
        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txCodigo.Text))
            {
                MessageBox.Show("Informe um código para o tipo de serviço",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            TipoServicoBLL bll = new TipoServicoBLL();
            TipoServico tipo = (IdAtual == 0
                ? new TipoServico()
                : bll.Find(IdAtual));

            tipo.Id = int.Parse(txCodigo.Text);
            tipo.Nome = txNome.Text;
            tipo.Preco = decimal.Parse(txPreco.Text);

            if(tipo.Id == 0)
            {
                MessageBox.Show("Informe um código para o tipo de serviço",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            bll.Save(tipo);
            Buscar();
            LimparCampos();
        }

        private void LimparCampos()
        {
            IdAtual = 0;
            txCodigo.Text = "0";
            txNome.Text = string.Empty;
            txPreco.Text = "0,00";
            txCodigo.IsEnabled = true;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TipoServico tipo = (TipoServico)listBox.SelectedItem;
            if (tipo == null)
                return;

            FillForm(tipo);
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            TipoServico tipo = (TipoServico)listBox.SelectedItem;
            if (tipo == null)
                return;

            MessageBoxResult res = MessageBox.Show($"Confirmar a exclusão do '{tipo.Nome}'? \nEsta ação não pode ser revertida!",
                "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No)
                return;

            try
            {
                TipoServicoBLL bll = new TipoServicoBLL();
                bll.Remove(tipo.Id);

                LimparCampos();
                Buscar();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void txPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            Buscar();
        }
    }
}
