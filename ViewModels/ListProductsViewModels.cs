using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiMVVM.Controllers;
using MauiMVVM.Models;

namespace MauiMVVM.ViewModels
{
    public class ListProductsViewModels : BaseViewModel
    {
        private ObservableCollection<Models.Productos> _products;
        private ProductosController productosController = new ProductosController();

        public ObservableCollection<Models.Productos> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged(); }
        }

        private Models.Productos _selectedProduct;

        public Models.Productos SelectedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public ICommand GoToDetailsCommand { private set; get; }
        public ICommand NuevoProductoCommand { private set; get; }
        public ICommand DeleteCommand { private set; get; }

        public INavigation Navigation { get; set; }

        public ListProductsViewModels(INavigation navigation)
        {
            Navigation = navigation;
            GoToDetailsCommand = new Command<Type>(async (pageType) => await GoToDetails(pageType, SelectedProduct));
            NuevoProductoCommand = new Command<Type>(async (pageType) => await NuevoProducto(pageType));
            DeleteCommand = new Command(async () => await DeleteProducto(SelectedProduct.Id));

            loadProductos();
        }

        async Task loadProductos()
        {
            List<Productos> listProductos;

            Products = new ObservableCollection<Productos>();

            try
            {
                listProductos = await productosController.getListProductos();
                foreach(var product in listProductos)
                {
                    Productos productos = new Productos
                    {
                        Id = product.Id,
                        Nombre = product.Nombre,
                        Precio = product.Precio,
                        Foto = product.Foto,
                    };

                    Products.Add(productos);
                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Se produjo un error al obtener los productos", "OK");
            }
        }

        async Task GoToDetails(Type pageType, Models.Productos selectedProduct)
        {
            if (selectedProduct != null)
            {
                var page = (Page)Activator.CreateInstance(pageType);

                var viewModel = new ProductosViewModels();
                viewModel.SelectedProduct = selectedProduct;
                viewModel.Nombre = selectedProduct.Nombre;
                viewModel.Foto = selectedProduct.Foto;
                viewModel.Precio = selectedProduct.Precio;
                viewModel.Id = selectedProduct.Id;
                page.BindingContext = viewModel;

                await Navigation.PushAsync(page);
            }
        }

        async Task NuevoProducto(Type pageType)
        {
            var page = (Page)Activator.CreateInstance(pageType);

            var viewModel = new ProductosViewModels();
            viewModel.SelectedProduct = null;
            page.BindingContext = viewModel;
            await Navigation.PushAsync(page);           
        }

        async Task DeleteProducto(int id)
        {
            if(SelectedProduct != null)
            {
                var tappedItem = Products.FirstOrDefault(item => item.Id == id);
                bool userConfirmed = await Application.Current.MainPage.DisplayAlert("Confirmación", "¿Está seguro de que desea eliminar este producto?", "Si", "No");
                if (userConfirmed)
                {
                    try
                    {

                        if (productosController != null)
                        {
                            int success = await productosController.deleteProducto(id);

                            if (success != 0)
                            {
                                Products.Remove(tappedItem);

                                await Application.Current.MainPage.DisplayAlert("Atención", "Producto Eliminado", "OK");
                            }
                        }

                    }
                    catch
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo eliminar el producto", "OK");
                    }
                }
                
            }
            
        }

    }
}
