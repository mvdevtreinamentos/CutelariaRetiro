//------------------------------------------------------------------------------
// <auto-generated>
//    O código foi gerado a partir de um modelo.
//
//    Alterações manuais neste arquivo podem provocar comportamento inesperado no aplicativo.
//    Alterações manuais neste arquivo serão substituídas se o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CutelariaRetiro.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Servico
    {
        public Servico()
        {
            this.ItemServico = new HashSet<ItemServico>();
            this.MaterialServico = new HashSet<MaterialServico>();
        }
    
        public int Id { get; set; }
        public string Cliente { get; set; }
        public string Telefone { get; set; }
        public System.DateTime Data { get; set; }
        public string Obs { get; set; }
        public bool Finalizado { get; set; }
    
        public virtual ICollection<ItemServico> ItemServico { get; set; }
        public virtual ICollection<MaterialServico> MaterialServico { get; set; }
    }
}