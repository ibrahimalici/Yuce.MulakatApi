using DA;
using DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Yuce.MulakatApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MovieController : JWTAuthenticationController
    {
        private IConfiguration _configuration;

        public MovieController(IConfiguration config) : base(config)
        {
            _configuration = config;
        }

        public dynamic isActive()
        {
            return new { ResultState = true, ResultMessage = "Başarılı" };
        }

        [Authorize]
        [HttpPost]
        public dynamic Rate([FromForm]UserMovieRating r)
        {
            if (r.Rating < 1 || r.Rating > 10)
                return new { ResultState = false, ResultMessage = "Lütfen 1-10 arası değer giriniz..." };

            dynamic result = new MovieDA(_configuration).saveUserMovieRating(r);

            return r;
        }

        [Authorize]
        [HttpPost]
        public dynamic getMovies([FromForm] PagingObject pagingObject)
        {
            if (pagingObject == null || pagingObject.countOfRecord == 0 || pagingObject.startIndex == 0)
                return new { ResultState = false, ResultMessage = "Lütfen startIndex ve countOfRecord alanlarını giriniz !" };

            dynamic result = new MovieDA(_configuration).getMovies(pagingObject);

            return result;
        }

        [Authorize]
        [HttpPost]
        public dynamic getMovie([FromForm] Guid MoviePK)
        {
            dynamic result = new MovieDA(_configuration).getMovie(MoviePK);

            return result;
        }

        [Authorize]
        [HttpPost]
        public dynamic saveMovieNote([FromForm] UserMovieNote movieNote)
        {
            dynamic result = new MovieDA(_configuration).saveMovieNote(movieNote);

            return result;
        }

        [Authorize]
        [HttpPost]
        public dynamic Advice([FromForm]AdviceObject adviceObject)
        {
            string mail_adres = _configuration["MailConfig:sender"];
            string mail_pass = _configuration["MailConfig:password"];

            string userId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            string userName = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            Movie movie = null;

            using(NHibernate.ISession db = DBSession.OpenUserSession(_configuration))
            {
                movie = db.Query<Movie>().Where(p => p.MoviePK == adviceObject.MoviePK).FirstOrDefault();
            }

            if (movie == null)
                return new { ResultState = false, ResultMessage = "Film bulunamadı !" };

            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = true;
            message.From = new MailAddress(mail_adres);

            message.Subject = userName + " Film Tavsiyesi";
            message.IsBodyHtml = true;
            message.To.Add(adviceObject.ReceiverEmail);

            string content_ = "<h1>" + movie.MovieName + "</h1>";
            content_ += "<p> Tarih : " + movie.ReleaseDate.ToShortDateString() +"</p>";
            content_ += "<p> Konusu : " + movie.OverView + "</p>";

            string body = string.Format(@"<head>
                        <meta name=""viewport"" content=""width = device - width"" />
                            <title>  </title>

                        <link href=""<link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"" integrity=""sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T"" crossorigin=""anonymous"">"" rel=""stylesheet"">
                    </head>
                        <body>

                            <section class=""center-block"">
                            <div class=""container"">
                                <div class=""row"">
                                    <div class=""col-lg-12"">
                                        <p></p>
                                        <div class=""panel panel-green"" style=""width:auto"">
                                            <div class=""panel-heading"" style=""background-color:orange;color:white;text-align:center"">
                                                <h3> Arkadaşınız " + userName + @" size bu filmi tavsiye ediyor.</h3>
                                            </div>
                                            <div class=""panel-body"" style=""text-align:center"">
                                                <div class=""row"">
                                                    {0}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </section>
                    </body>
                    </html>", content_);

            message.Body = body;
            smtp.Port = 587;
            smtp.Host = "smtp.yandex.com.tr";
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(mail_adres, mail_pass);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);

            return new { ResultState = true, ResultMessage = "İşlem Başarılı" };
        }
    }
}
