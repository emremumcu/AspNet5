using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Tools
{


    //ClaimsIdentity ci = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "uid") });

    //string pk = JWTFactory.GenerateRandomKey();

    //JWTFactory jwt = new JWTFactory(ci);

    //string t = jwt.CreateToken();

    //bool b = jwt.ValidateToken(t);

    //var tt = jwt.GetJwtSecurityToken(t);

    //string et = jwt.CreateEncryptedToken();

    //bool be = jwt.ValidateEncryptedToken(et);

    //var tte = jwt.GetJwtSecurityToken(et);

















    // Install-Package Microsoft.IdentityModel.Tokens
    // Install-Package System.IdentityModel.Tokens.Jwt

    [JsonObject("tokenParameters")]
    public class TokenParameters
    {
        [JsonProperty("securityKey")]
        public string SecurityKey { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("expires")]
        public int Expires { get; set; }
    }

    public class JWTFactory
    {
        public SecurityKey GetSigningKey()
        {
            string Base64TokenSecurityKey = Convert.ToBase64String(Encoding.Unicode.GetBytes("secret-1"));
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Base64TokenSecurityKey));
        }

        public SecurityKey GetEncryptionKey()
        {
            string Base64TokenSecurityKey = Convert.ToBase64String(Encoding.Unicode.GetBytes("secret-2"));
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Base64TokenSecurityKey));
        }

        public EncryptingCredentials GetEncryptingCredentials()
        {
            EncryptingCredentials encCredentials = new EncryptingCredentials(
                GetEncryptionKey(),
                SecurityAlgorithms.Aes128KW,
                SecurityAlgorithms.Aes128CbcHmacSha256
            );

            return encCredentials;
        }

        public SecurityTokenDescriptor GetSecurityTokenDescriptor()
        {
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Subject = _claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = "https://mumcu.net",
                Audience = "*",
                SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenDescriptor;
        }

        public SecurityTokenDescriptor GetEncryptedSecurityTokenDescriptor()
        {
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Subject = _claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = "https://mumcu.net",
                Audience = "*",
                EncryptingCredentials = GetEncryptingCredentials(),
                SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenDescriptor;
        }

        public TokenValidationParameters GetTokenValidationParameters()
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = "https://mumcu.net",
                ValidAudience = "*",
                IssuerSigningKey = GetSigningKey(),
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            return tokenValidationParameters;
        }

        public TokenValidationParameters GetEncryptedTokenValidationParameters()
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = "https://mumcu.net",
                ValidAudience = "*",
                IssuerSigningKey = GetSigningKey(),
                TokenDecryptionKey = GetEncryptionKey()
            };

            return tokenValidationParameters;
        }

        public readonly ClaimsIdentity _claimsIdentity;

        public static string  GenerateRandomKey(int keySize = 64)
        {
            // 1
            RijndaelManaged myAlg = new RijndaelManaged();
            Rfc2898DeriveBytes keyy = new Rfc2898DeriveBytes("password", Encoding.ASCII.GetBytes("saltsalt"));
            myAlg.Key = keyy.GetBytes(myAlg.KeySize / 8);
            myAlg.IV = keyy.GetBytes(myAlg.BlockSize / 8);

            // 2
            AesManaged aes = new AesManaged();
            aes.GenerateKey();
            aes.GenerateIV();

            using (var rng = new RNGCryptoServiceProvider())
            {
                var key = new byte[keySize];
                rng.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }

        public string CreateToken(TokenParameters tokenParameters, List<Claim> claims)
        {                 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParameters.SecurityKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: tokenParameters.Issuer,
                audience: tokenParameters.Audience,
                claims: claims,                
                notBefore: DateTime.UtcNow,                
                expires: DateTime.Now.AddMinutes(tokenParameters.Expires),
                signingCredentials: credentials
            );
            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }

        public JWTFactory() { }

        public JWTFactory(ClaimsIdentity claimsIdentity)
        {
            _claimsIdentity = claimsIdentity;
        }

        public string CreateToken()
        {            
            // Degug:
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(GetSecurityTokenDescriptor());
            // or
            JwtSecurityToken jwtToken = tokenHandler.CreateJwtSecurityToken(GetSecurityTokenDescriptor());

            return tokenHandler.WriteToken(token);
        }

        public string CreateEncryptedToken()
        {
            // Degug:
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(GetSecurityTokenDescriptor());
            // or
            JwtSecurityToken jwtToken = tokenHandler.CreateJwtSecurityToken(GetSecurityTokenDescriptor());

            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token, out string message)
        {
            try
            {
                SecurityToken validatedToken;
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal user = tokenHandler.ValidateToken(token, GetTokenValidationParameters(), out validatedToken);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }

            message = "Token is valid";
            return true;
        }

        public bool ValidateEncryptedToken(string token)
        {
            try
            {
                SecurityToken validatedToken;
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal user = tokenHandler.ValidateToken(token, GetTokenValidationParameters(), out validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public JwtSecurityToken GetJwtSecurityToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return securityToken;
        }


    }
}


//https://codeburst.io/jwt-auth-in-asp-net-core-148fb72bed03
//var refreshToken = new RefreshToken
//{
//    UserName = username,
//    TokenString = GenerateRefreshTokenString(),
//    ExpireAt = now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
//};
//_usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (s, t) => refreshToken);

//return new JwtAuthResult
//{
//    AccessToken = accessToken,
//    RefreshToken = refreshToken
//};

//private static string GenerateRefreshTokenString()
//{
//    var randomNumber = new byte[32];
//    using var randomNumberGenerator = RandomNumberGenerator.Create();
//    randomNumberGenerator.GetBytes(randomNumber);
//    return Convert.ToBase64String(randomNumber);
//}



//////////// Install-Package Microsoft.AspNetCore.Authentication.JwtBearer
//////////private Object CreateToken()
//////////{
//////////    string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
//////////    var issuer = "http://mysite.com";  //normally this will be your site URL    

//////////    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
//////////    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//////////    //Create a List of Claims, Keep claims name short    
//////////    var permClaims = new List<Claim>();
//////////    permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
//////////    permClaims.Add(new Claim("valid", "1"));
//////////    permClaims.Add(new Claim("userid", "1"));
//////////    permClaims.Add(new Claim("name", "bilal"));

//////////    //Create Security Token object by giving required parameters    
//////////    var token = new JwtSecurityToken(issuer, //Issure    
//////////                    issuer,  //Audience    
//////////                    permClaims,
//////////                    expires: DateTime.Now.AddDays(1),
//////////                    signingCredentials: credentials);
//////////    var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
//////////    return new { data = jwt_token };
//////////}

//////////// Register this in ConfigureServices
//////////// Request must have Headers =>  Authorization: Bearer<token> to pass Authorize attribute
//////////public void ValidateToken(IServiceCollection services)
//////////{
//////////    string key = "my_secret_key_12345"; //this should be same which is used while creating token      
//////////    var issuer = "http://mysite.com";  //this should be same which is used while creating token  

//////////    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//////////  .AddJwtBearer(options =>
//////////  {
//////////      options.TokenValidationParameters = new TokenValidationParameters
//////////      {
//////////          ValidateIssuer = true,
//////////          ValidateAudience = true,
//////////          ValidateIssuerSigningKey = true,
//////////          ValidIssuer = issuer,
//////////          ValidAudience = issuer,
//////////          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
//////////      };

//////////      options.Events = new JwtBearerEvents
//////////      {
//////////          OnAuthenticationFailed = context =>
//////////          {
//////////              if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
//////////              {
//////////                  context.Response.Headers.Add("Token-Expired", "true");
//////////              }
//////////              return Task.CompletedTask;
//////////          }
//////////      };
//////////  });
//////////}


//////////public string GenerateToken(int userId)
//////////{
//////////    var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
//////////    var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

//////////    var myIssuer = "http://mysite.com";
//////////    var myAudience = "http://myaudience.com";

//////////    var tokenHandler = new JwtSecurityTokenHandler();
//////////    var tokenDescriptor = new SecurityTokenDescriptor
//////////    {
//////////        Subject = new ClaimsIdentity(new Claim[]
//////////        {
//////////            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
//////////        }),
//////////        Expires = DateTime.UtcNow.AddDays(7),
//////////        Issuer = myIssuer,
//////////        Audience = myAudience,
//////////        SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
//////////    };

//////////    var token = tokenHandler.CreateToken(tokenDescriptor);
//////////    return tokenHandler.WriteToken(token);
//////////}

//////////public bool ValidateCurrentToken(string token)
//////////{
//////////    var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
//////////    var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

//////////    var myIssuer = "http://mysite.com";
//////////    var myAudience = "http://myaudience.com";

//////////    var tokenHandler = new JwtSecurityTokenHandler();
//////////    try
//////////    {
//////////        tokenHandler.ValidateToken(token, new TokenValidationParameters
//////////        {
//////////            ValidateIssuerSigningKey = true,
//////////            ValidateIssuer = true,
//////////            ValidateAudience = true,
//////////            ValidIssuer = myIssuer,
//////////            ValidAudience = myAudience,
//////////            IssuerSigningKey = mySecurityKey
//////////        }, out SecurityToken validatedToken);
//////////    }
//////////    catch
//////////    {
//////////        return false;
//////////    }
//////////    return true;
//////////}