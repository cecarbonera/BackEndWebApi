using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dapper;
using BackEndWebApi.Models;

namespace BackEndWebApi.Controllers
{
    [ApiController]    
    [Route("api/[controller]")]
    [Authorize]
    public class DepartamentoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DepartamentoController> _logger;
        private readonly SqlConnection _conexao;

        //Construtor
        public DepartamentoController(IConfiguration configuration, ILogger<DepartamentoController> logger)
        {
            _configuration = configuration;
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
                return Ok(_conexao.Query<DepartamentoDTO>("SELECT CODIGO, DESCRICAO FROM DBO.DEPARTAMENTO ORDER BY CODIGO"));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Departamento.Listar() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ao executar Listar()");

            }

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
        [Route("ListarDeptoEmpr")]
        [Authorize(Roles = "D,G,F,E")]
        public async Task<ActionResult> ListarDeptoEmpr()
        {
            try
            {
                return Ok(_conexao.Query<DepartamentoDTO>(
                    "SELECT CODIGO, CONCAT(CAST(CODIGO AS VARCHAR(2)), ' - ', DESCRICAO) AS DESCRICAO FROM DBO.DEPARTAMENTO ORDER BY CODIGO"));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Departamento.Listar() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error ao executar Listar()");

            }

        }

        [HttpPost]
        [Route("Inserir")]
        [Authorize(Roles = "D,G,F")]
        public async Task<ActionResult> Inserir(DepartamentoDTO departamento)
        {
            try
            {
                if (departamento == null)
                    return BadRequest();

                var resultado = await _conexao.ExecuteAsync($"INSERT INTO DBO.DEPARTAMENTO VALUES({departamento.Codigo}, '{departamento.Descricao.Trim()}')");

                if (resultado != 1)
                    return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Departamento.Inserir() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Departamento/Inserir ");

            }

            return Ok();

        }

        [HttpPut]
        [Route("Atualizar")]
        [Authorize(Roles = "D,G,F")]
        public async Task<IActionResult> Atualizar(DepartamentoDTO departamento)
        {
            try
            {
                if (departamento == null)
                    return BadRequest();

                var resultado = await _conexao.ExecuteAsync(
                    $"UPDATE DBO.DEPARTAMENTO SET DESCRICAO = '{departamento.Descricao.Trim()}' WHERE CODIGO = {departamento.Codigo}");

                //Quantidade de linhas alteradas
                if (resultado != 1)
                    return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Departamento.Atualizar() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Departamento/Atualizar ");

            }

            return Ok();

        }

        [HttpDelete("Excluir/{id}")]
        [Authorize(Roles = "D,G")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var resultado = await _conexao.ExecuteAsync($"DELETE FROM DBO.DEPARTAMENTO WHERE CODIGO = {id}");

                //Quantidade de linhas inseridas
                if (resultado != 1)
                    return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Departamento.Excluir() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Departamento/Excluir ");

            }

            return Ok();

        }

    }

}