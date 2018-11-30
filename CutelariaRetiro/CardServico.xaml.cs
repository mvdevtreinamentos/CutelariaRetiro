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
    /// Interação lógica para CardServico.xam
    /// </summary>
    public partial class CardServico : UserControl
    {
        public delegate void Editar(Servico serv);
        public event Editar Edicao;

        private Servico servico;
        public CardServico(Servico serv)
        {
            InitializeComponent();

            servico = serv;
            lbNUmero.Text = $"N° {serv.Id.ToString()}";
            lbCliente.Text = serv.Cliente;
            lbValor.Text = $"R$ {serv.GetTotalGeral().ToString("N2")}";
            lbObs.Content = serv.Obs;
        }

        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            Edicao?.Invoke(servico);
        }
    }
}
