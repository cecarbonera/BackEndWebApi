using BackEndWebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using BackEndWebApi.Messageria;

namespace BackEndWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpregadosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmpregadosController> _logger;
        private readonly SqlConnection _conexao;

        public EmpregadosController(IConfiguration configuration,
                                    IWebHostEnvironment env,
                                    ILogger<EmpregadosController> logger)
        {
            _configuration = configuration;
            _env = env;
            _logger = logger;
            _conexao = new SqlConnection(_configuration.GetConnectionString("Capacitacao"));

        }

        /// <summary>
        /// D - Diretor
        /// G - Gerente
        /// F - Funcionário 
        /// E - Estagiário
        /// </summary>
        /// <returns>
        ///  Listagem de dados
        /// </returns>
        [HttpGet]
        [Route("Listar")]
        [Authorize(Roles = "D,G,F,E")]
        public async Task<ActionResult> Listar()
        {
            try
            {
                return Ok(_conexao.Query<EmpregadosDTO>(@"SELECT E.CODIGO,
                                                                 E.NOME,
                                                                 CONCAT(CAST(E.CODIGODEPTO AS VARCHAR(2)), ' - ', DESCRICAO) AS CODIGODEPTO,
                                                                 CONVERT(VARCHAR(10), E.DATAENTRADA, 120) AS DATAENTRADA,
                                                                 E.FOTO
                                                            FROM DBO.EMPREGADOS E
                                                                 INNER JOIN DBO.DEPARTAMENTO D ON (D.CODIGO = E.CODIGODEPTO) 
                                                           ORDER BY E.CODIGO"));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Empregados.Listar() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ao executar Listar()");

            }

        }

        [HttpPost]
        [Route("Inserir")]
        [Authorize(Roles = "D,G,F")]
        public async Task<ActionResult> Inserir(EmpregadosDTO empregados)
        {
            try
            {
                if (empregados == null)
                    return BadRequest();

                if (empregados.Foto != "")
                    empregados.Foto = empregados.Foto.Substring(empregados.Foto.LastIndexOf("/") + 1);

                var resultado = await _conexao.ExecuteAsync(string.Concat(
                    "INSERT INTO DBO.EMPREGADOS VALUES(", empregados.Codigo, ", '", empregados.Nome.Trim(), "', ", empregados.CodigoDepto, ", '",
                    Convert.ToDateTime(empregados.DataEntrada).ToString("yyyy-MM-dd"), "', '", empregados.Foto, "')"));

                if (resultado != 1)
                    return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Empregados.Inserir() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Empregados/Inserir ");

            }

            return Ok();

        }

        [HttpPut]
        [Route("Atualizar")]
        [Authorize(Roles = "D,G,F")]
        public async Task<IActionResult> Atualizar(EmpregadosDTO empregados)
        {
            try
            {
                if (empregados == null)
                    return BadRequest();

                if (empregados.Foto != "")
                    empregados.Foto = empregados.Foto.Substring(empregados.Foto.LastIndexOf("/") + 1);

                var resultado = await _conexao.ExecuteAsync(string.Concat(
                        "UPDATE DBO.EMPREGADOS ",
                        "   SET NOME        = '",empregados.Nome.Trim(), "'",
                        "     , CODIGODEPTO = ", empregados.CodigoDepto,
                        "     , DATAENTRADA = '", Convert.ToDateTime(empregados.DataEntrada).ToString("yyyy-MM-dd") , "'",
                        "     , FOTO        = '", empregados.Foto, "'",
                        " WHERE CODIGO      = ", empregados.Codigo));

                //Quantidade de linhas alteradas
                if (resultado != 1)
                    return BadRequest();

                //var msgEnviar = new ObjetoPersonalisado { Id = Guid.NewGuid(), Mensagem = "Ola" + new Random().Next(1, 8).ToString() };
                //new Publisher().PublisherMessage(msgEnviar);
                //new Consumer().ConsumerMessage();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Empregados.Atualizar() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Empregados/Atualizar ");

            }

            return Ok();
                                   
        }

        [HttpDelete("Excluir/{id}")]
        [Authorize(Roles = "D,G")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var resultado = await _conexao.ExecuteAsync($"DELETE FROM DBO.EMPREGADOS WHERE CODIGO = {id}");

                //Quantidade de linhas inseridas
                if (resultado != 1)
                    return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Empregados.Excluir() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Empregados/Excluir ");

            }

            return Ok();

        }

        [HttpPost]
        [Route("SalvarArquivo")]
        [Authorize(Roles = "D,G,F")]
        public IActionResult SalvarArquivo()
        {
            try
            {
                var httpReq = Request.Form;
                var arquivo = httpReq.Files[0];
                var caminhoFisico = _env.ContentRootPath + "/Fotos/" + arquivo.FileName;

                using (var stream = new FileStream(caminhoFisico, FileMode.Create))
                {
                    arquivo.CopyTo(stream);
                }

                return new JsonResult(arquivo.FileName);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Empregados.SalvarArquivo() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Empregados/SalvarArquivo ");

            }

        }

    }

}