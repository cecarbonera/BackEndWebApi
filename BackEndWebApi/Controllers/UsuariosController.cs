using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dapper;
using BackEndWebApi.Models;
using BackEndWebApi.Services;
using System.Linq;

namespace BackEndWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsuariosController> _logger;
        private readonly SqlConnection _conexao;

        //Construtor
        public UsuariosController(IConfiguration configuration, ILogger<UsuariosController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _conexao = new SqlConnection(_configuration.GetConnectionString("Capacitacao"));

        }

        /// <summary>
        /// Inserir o Usuario no banco de Dados
        /// </summary>
        /// <param name="usuario">Objeto de Dados</param>
        /// <returns>Retorno padrão</returns>
        [HttpPost]
        [Route("Inserir")]
        public async Task<ActionResult> Inserir(UsuariosDTO usuario)
        {
            try
            {
                if (usuario == null)
                    return BadRequest();

                var resultado = await _conexao.ExecuteAsync(string.Concat(
                    $"INSERT INTO DBO.USUARIOS VALUES('{usuario.Usuario}', '{usuario.Senha}', '{usuario.Regra.ToUpper() ?? "F"}', '{usuario.Email.ToLower()}')"));

                if (resultado != 1)
                    return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Usuarios.Inserir() - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao executar Usuarios/Inserir ");

            }

            return Ok();

        }

        /// <summary>
        /// Geração do Token do Usuário
        /// </summary>
        /// <param name="usuario">Usuário</param>
        /// <param name="senha">Senha</param>
        /// <returns>
        ///     Dados e token de acesso do usuário
        /// </returns>
        [HttpGet]
        [Route("Login/{Usuario}/{Senha}")]
        public ActionResult<dynamic> Login(string usuario, string senha)
        {
            //Consultar por Usuario e senha
            var _usuario = _conexao.Query<UsuariosDTO>(
                    $"SELECT ID, USUARIO, SENHA, REGRA FROM DBO.USUARIOS WHERE UPPER(USUARIO) = '{usuario.ToUpper()}' AND SENHA = '{senha}'").ToList();

            if (_usuario.Count() == 0)
                return BadRequest();

            // Gerar o Token de acesso
            var _token = TokenService.GerarTokenAcesso(_usuario.AsList()[0]);

            //Retornas os dados
            return new
            {
                usuario = _usuario,
                acessToken = _token

            };

        }

    }

}