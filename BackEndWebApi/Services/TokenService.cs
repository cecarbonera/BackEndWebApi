using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEndWebApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace BackEndWebApi.Services
{
    public static class TokenService
    {
        public static string GerarTokenAcesso(UsuariosDTO usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            //Gerar o token de acesso
            var token = tokenHandler.CreateToken(
                new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, usuario.Usuario.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Regra.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Settings.acessoJwt)),
                                                                    SecurityAlgorithms.HmacSha256Signature)

                });

            return tokenHandler.WriteToken(token);

        }

    }

}