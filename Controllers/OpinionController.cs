using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using practica4.Data;
using practica4.ML;
using practica4.Models;

namespace practica4.Controllers
{
   
    public class OpinionController : Controller
    {
        private readonly ILogger<OpinionController> _logger;
        private readonly ApplicationDbContext _context;

        public OpinionController(ILogger<OpinionController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Load sample data
                    var sampleData = new MLModel1.ModelInput()
                    {
                        Col1 = opinion.Mensaje,
                         // No es necesario, pero lo dejamos como placeholder
                    };

                    //Load model and predict output
                    var result = MLModel1.Predict(sampleData);
                    var predictedLabel = result.PredictedLabel;
                    var scorePositive = result.Score[1];
                    var scoreNegative = result.Score[0];

                    //Check if the result is positive or negative
                    if (predictedLabel == 1)
                    {
                        opinion.Etiqueta = "Positivo";
                        opinion.Puntuacion = scorePositive;
                    }
                    else
                    {
                        opinion.Etiqueta = "Negativo";
                        opinion.Puntuacion = scoreNegative;
                    }

                    _context.DbSetOpinion.Add(opinion);
                    _context.SaveChanges();
                    _logger.LogInformation("Se envio la opinion");
                    ViewData["Message"] = "Se envio la opinion";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al registrar la opinion");
                    ViewData["Message"] = "Error al registrar la opinion: " + ex.Message;
                }
            }
            else
            {
                ViewData["Message"] = "Datos de entrada no válidos";
            }
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}