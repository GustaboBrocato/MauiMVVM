using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMVVM.Models
{
    public class productosModel
    {
        public string? Key {  get; set; }
        public string? Nombre { get; set; }
        public double Precio { get; set; }
        public string? Foto { get; set; }
    }
}
