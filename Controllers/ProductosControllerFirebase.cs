using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using MauiMVVM.Models;

namespace MauiMVVM.Controllers
{
    public class ProductosControllerFirebase
    {
        //Cliente de Firebase
        private FirebaseClient client = new FirebaseClient("https://mauimvvm-default-rtdb.firebaseio.com/");

        //Constructor Vacio
        public ProductosControllerFirebase() { }

        //Metodo para crear/agregar/update un nuevo producto
        public async Task<bool> CrearProducto(productosModel producto)
        {
            if (string.IsNullOrEmpty(producto.Key))
            {
                try
                {
                    var productos = client.Child("Productos").OnceAsync<productosModel>();
                    if (productos.Result.Count == 0)
                    {
                        await client.Child("Productos").PostAsync(new productosModel
                        {
                            Nombre = producto.Nombre,
                            Precio = producto.Precio,
                            Foto = producto.Foto,
                        });

                        return true;
                    }
                }catch (Exception ex)
                {
                    return false;
                }      
            }
            else
            {
                try
                {
                    await client.Child("Productos").Child(producto.Key).PutAsync(producto);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                

            }

            return false;
        }

        //Metodo para Lectura de elementos
        public async Task<List<productosModel>> GetListProductos()
        {
            var productos = await client.Child("Productos").OnceSingleAsync<Dictionary<string, productosModel>>();

            return productos.Select(x => new productosModel
            {
                Key = x.Key,
                Nombre = x.Value.Nombre,
                Precio = x.Value.Precio,
                Foto = x.Value.Foto
            }).ToList();
        }

        //Delete
        public async Task<bool> deleteProducto(string key)
        {
            try
            {
                await client.Child("Productos").Child(key).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
