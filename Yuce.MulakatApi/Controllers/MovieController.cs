using DA;
using DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Yuce.MulakatApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MovieController : JWTAuthenticationController
    {
        private IConfiguration _config;

        public MovieController(IConfiguration config) : base(config)
        {
            _config = config;
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

            dynamic result = new MovieDA(_config).saveUserMovieRating(r);

            return r;
        }

        [Authorize]
        [HttpPost]
        public dynamic getMovies([FromForm] PagingObject pagingObject)
        {
            if (pagingObject == null || pagingObject.countOfRecord == 0 || pagingObject.startIndex == 0)
                return new { ResultState = false, ResultMessage = "Lütfen startIndex ve countOfRecord alanlarını giriniz !" };

            dynamic result = new MovieDA(_config).getMovies(pagingObject);

            return result;
        }

        [Authorize]
        [HttpPost]
        public dynamic getMovie([FromForm] Guid MoviePK)
        {
            dynamic result = new MovieDA(_config).getMovie(MoviePK);

            return result;
        }

        [Authorize]
        [HttpPost]
        public dynamic saveMovieNote([FromForm] UserMovieNote movieNote)
        {
            dynamic result = new MovieDA(_config).saveMovieNote(movieNote);

            return result;
        }

    }
}
