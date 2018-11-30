using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.Model
{
    public class MovimentoEstoque
    {
        public int Id { get; set; }
        public MaterialServico MaterialServico { get; private set; }
        public EntradaMaterial EntradaMaterial { get; private set; }
        public string Tipo { get; private set; }
        public int Quant { get; private set; }
        public DateTime Data { get; private set; }

        public MovimentoEstoque(MaterialServico servico)
        {
            MaterialServico = servico;
            Data = servico.Servico.Data;
            Tipo = "Saída";
            Quant = servico.Quantidade;
        }

        public MovimentoEstoque(EntradaMaterial entrada)
        {
            EntradaMaterial = entrada;
            Data = entrada.Data;
            Tipo = "Entrada";
            Quant = entrada.Quantidade;
        }
    }
}
