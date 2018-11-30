using CutelariaRetiro.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutelariaRetiro.Model
{
    public static class MaterialExt
    {
        public static int MediaDiariaMaterial(this Material mat,
            DateTime dataInicio, DateTime dataFim)
        {
            ServicoBLL bll = new ServicoBLL();
            return bll.GetMediaDiariaMaterial(mat.Id, 
                dataInicio, dataFim);
        }
    }
}
