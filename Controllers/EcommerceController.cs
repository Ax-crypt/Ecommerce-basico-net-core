using ProyectoFinal.Models;
using ProyectoFinal.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ProyectoFinal.Controllers
{
    public class EcommerceController : Controller
    {
        //variable privada de solo lectura
        private readonly EcommerceDAO dao;
        List<Carrito> lista_carrito = new List<Carrito>();
        public EcommerceController(EcommerceDAO _dao)
        {
            dao = _dao;
        }

        // serializar los datos del objeto lista_carrito a
        // una cadena json guardando en una session
        void GrabarCarrito()
        {
            HttpContext.Session.SetString("Carrito",
                JsonConvert.SerializeObject(lista_carrito));
        }

        // deserializar la cadena json de una session a
        // un objeto lista_carrito
        List<Carrito> RecuperarCarrito()
        {
            return JsonConvert.DeserializeObject<List<Carrito>>(
                HttpContext.Session.GetString("Carrito")!)!;
        }

        // GET: EcommerceController
        public ActionResult IndexProductos() 
        {
            // si la session Carrito no existe
            // entonces la creamos con grabarCarrito
            if (HttpContext.Session.GetString("Carrito") == null)
            {
                GrabarCarrito();
                TempData.Clear();
            }
             var listado = dao.GetProductos();           

            //
            return View(listado);
        }

        // GET: EcommerceController/AgregarAlCarrito/5
        public ActionResult AgregarAlCarrito(string id) 
        {
            return View(dao.BuscarProducto(id));
        }

        // POST: EcommerceController/AgregarAlCarrito/5
        [HttpPost]
        public ActionResult AgregarAlCarrito(string id, int cantidad)
        {
            // recuperar los datos del producto
            var prod = dao.BuscarProducto(id);
            // creamos un nuevo objeto carrito de compra y le enviamos
            // los datos
            var car = new Carrito()
            {
                Codigo = prod.cod_art,
                NombreProducto = prod.nom_art,
                Precio = prod.pre_art,
                Cantidad = cantidad
            };
            // obtenemos el contenido del carrito de compra
            lista_carrito = RecuperarCarrito();
            // buscamos en el carrito el codigo "id" del producto
            var encontrado = lista_carrito.Find(x => x.Codigo.Equals(id));
            // si es nulo, el id no fue encontrado, entonces lo agregamos
            if (encontrado == null)
            {
                lista_carrito.Add(car);
                ViewBag.mensaje = "¡Excelente elección! El nuevo producto se ha agregado a tu cesto.";
            }
            else // si existe el producto, entonces aumentamos la cantidad
            {
                encontrado.Cantidad += cantidad;
            }
            // grabar los cambios 
            GrabarCarrito();
     
            return View(prod);
        }


        public ActionResult VerCarrito() 
        {
            // Si la variable de sesión existe
            if (HttpContext.Session.GetString("Carrito") != null)
            {
                lista_carrito = RecuperarCarrito();

                // Verificamos si el carrito está vacío
                if (lista_carrito.Count == 0)
                {
                    TempData["Mensaje"] = "Tu carrito está vacío. ¡Añade productos para continuar!";
                    return RedirectToAction("IndexProductos"); 
                }
            }
            else
            {
                TempData["Mensaje"] = "Tu carrito está vacío. ¡Añade productos para continuar!";
                return RedirectToAction("IndexProductos"); 
            }

            // Calcular el total del carrito
            ViewBag.total = lista_carrito.Sum(c => c.Importe);

            // Regresa la vista con la lista del carrito
            return View(lista_carrito);
        }


        // GET: EcommerceController/Delete/5
        public ActionResult EliminarProducto(string id)
        {
            lista_carrito = RecuperarCarrito();
            lista_carrito.RemoveAll(c => c.Codigo.Equals(id));

            GrabarCarrito();

            TempData["mensaje"] = "Producto Eliminado correctamente";
            return RedirectToAction("VerCarrito");
        }

        // Actualizar cantidad
        [HttpPost]
        public ActionResult ActualizarCantidad(string id, int cantidad)
        {
            // Recuperar el carrito desde la sesión
            lista_carrito = RecuperarCarrito();

            // Buscar el producto en el carrito
            var producto = lista_carrito.FirstOrDefault(c => c.Codigo == id);

            if (producto != null)
            {
                // Actualizar la cantidad si el producto existe
                producto.Cantidad = cantidad;

                // Guardar los cambios en el carrito
                GrabarCarrito();

                TempData["mensaje"] = "Cantidad actualizada correctamente.";
            }
            else
            {
                TempData["mensaje"] = "El producto no existe en el carrito.";
            }

            // Redirigir nuevamente a la vista del carrito
            return RedirectToAction("VerCarrito");
        }

        //
        public ActionResult FormularioCompra()
        {
            // Recuperar el total del carrito desde la sesión
            lista_carrito = RecuperarCarrito();
            ViewBag.total = lista_carrito.Sum(c => c.Importe);

            return View();
        }

        // 
        [HttpPost]
        public ActionResult ConfirmarCompra(string nombre, string direccion, string telefono, string tarjeta, int cod)
        {

            TempData["mensajeExito"] = "¡Compra exitosa! Gracias por su compra.";

            // Vaciar el carrito después de confirmar la compra
            HttpContext.Session.Remove("Carrito");

            return RedirectToAction("CompraExitosa");
        }

        // exito de compra
        public ActionResult CompraExitosa()
        {
            ViewBag.Mensaje = TempData["mensajeExito"];
            return View();
        }

        // busqueda y paginacion
        public async Task<IActionResult> ListarProductos(int nropagina = 0, string nombre= null!)
        {

           IEnumerable<Articulos> listado = string.IsNullOrEmpty(nombre) ? new List<Articulos>() : dao.listarArticulos(nombre);

            ViewBag.nombre = nombre;

            // Inicio de la paginacion
            int filas_pagina = 3;
            int contador = listado.Count();
            int paginas = 0;
            if (contador % filas_pagina == 0)
                paginas = contador / filas_pagina;
            else
                paginas = (contador / filas_pagina) + 1;

            ViewBag.paginas = paginas;
            return View(listado.Skip(nropagina * filas_pagina).Take(filas_pagina));


        }
    }
}
