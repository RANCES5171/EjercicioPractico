using EjercicioPractico.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EjercicioPractico.Controllers
{
    public class ProspectoController : Controller
    {
        // GET: Prospecto
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Listar()
        {
            EjemploProspectosContex db = new EjemploProspectosContex();
            List<Prospecto> lista = db.Prospecto.ToList();
            return View(lista);
        }

        public ActionResult Revisar()
        {
            using (EjemploProspectosContex db = new EjemploProspectosContex())
            {
                List<Prospecto> lista = db.Prospecto.ToList();
                return View(lista);
            }
        }
        public ActionResult Agregar()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Agregar(Prospecto A, HttpPostedFileBase[] Formulario, string[] NomArchivo)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                using (EjemploProspectosContex db = new EjemploProspectosContex())
                {
                    A.pros_rutas = "";
                    A.pros_notas = "";
                    A.pros_estatus = "Enviado";
                    db.Prospecto.Add(A);
                    db.SaveChanges();
                    string Rutas = "";
                    if (Formulario != null&& NomArchivo!=null)
                    {


                        for (int i = 0; i < Formulario.Length; i++)
                        {
                            if (Formulario[i] != null && Formulario[i].ContentLength > 0)
                            {
                                var fileName = A.pros_id + "_" + NomArchivo[i] + ".pdf";
                                Rutas += fileName + ";";
                                // extract only the fielname

                                // store the file inside ~/App_Data/uploads folder
                                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                Formulario[i].SaveAs(path);
                            }
                        }
                    }
                    A.pros_rutas = Rutas;
                    db.SaveChanges();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error al agregar", ex);
                return View();
            }

        }
        [HttpPost]
        public ActionResult ConsultaDetalles(int id)
        {
            try
            {


                using (EjemploProspectosContex db = new EjemploProspectosContex())
                {
                    var detalles = db.Prospecto.Find(id);
                    return PartialView("_FormularioView", detalles);
                }
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        public ActionResult ConsultaRevision(int id)
        {
            try
            {
                using (EjemploProspectosContex db = new EjemploProspectosContex())
                {
                    var detalles = db.Prospecto.Find(id);
                    return PartialView("_RevisionView", detalles);
                }
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }
        public JsonResult Aprovar(int id, string notas)
        {
            if (notas == null || notas == "")
            {
                notas = " ";
            }
            try
            {
                using (EjemploProspectosContex db = new EjemploProspectosContex())
                {
                    var Prospecto = db.Prospecto.Find(id);
                    Prospecto.pros_notas = notas;
                    Prospecto.pros_estatus = "Autorizado";
                    db.SaveChanges();
                    return Json("Exito", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Rechazar(int id, string notas)
        {
            if (notas == null || notas == "")
            {
                notas = " ";
            }
            try
            {
                using (EjemploProspectosContex db = new EjemploProspectosContex())
                {
                    var Prospecto = db.Prospecto.Find(id);
                    Prospecto.pros_notas = notas;
                    Prospecto.pros_estatus = "Rechazado";
                    db.SaveChanges();
                    return Json("Exito", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult GetReport(string path)
        {
            var FileURL = Path.Combine(Server.MapPath("~/App_Data/uploads"), path);
            string ReportURL = path;
            byte[] FileBytes = System.IO.File.ReadAllBytes(FileURL);
            return File(FileBytes, "application/pdf");
        }
    }

}