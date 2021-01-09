using DO;
using Microsoft.Extensions.Configuration;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DA
{
    public class MovieDA
    {
        public IConfiguration _configuration;

        public MovieDA(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region LoginCheck
        public UserProfile LoginCheck(string userName, string password)
        {
            using (ISession db = DBSession.OpenUserSession(_configuration))
            {
                if (!db.Query<UserProfile>().Any())
                {
                    #region Create Default User
                    using (ITransaction tr = db.BeginTransaction())
                    {
                        UserProfile default_user = new UserProfile
                        {
                            UserName = "deneme",
                            Password = "abc",
                            Email = "deneme@deneme.com",
                            UserProfilePK = Guid.NewGuid()
                        };
                        db.SaveOrUpdate(default_user);
                        tr.Commit();
                    }
                    #endregion
                }

                UserProfile user = db.Query<UserProfile>().Where(p => p.UserName == userName && p.Password == password).FirstOrDefault();
                return user;
            }
        }
        #endregion

        #region getMovie
        public dynamic getMovie(Guid MoviePK)
        {
            try
            {
                vwMovie movie = null;

                using (ISession db = DBSession.OpenUserSession(_configuration))
                {
                    Movie movie_ = db.Query<Movie>().Where(p => p.MoviePK == MoviePK).FirstOrDefault();

                    movie = new vwMovie
                    {
                        Country = movie_.Country,
                        MovieName = movie_.MovieName,
                        MoviePK = movie_.MoviePK,
                        OverView = movie_.OverView,
                        ReleaseDate = movie_.ReleaseDate,
                        syncId = movie_.syncId
                    };

                    movie.MovieNotes = (from p in db.Query<UserMovieNote>()
                                        where p.MovieFK == movie.MoviePK
                                        select new vwUserMovieNote
                                        {
                                            MovieFK = p.MovieFK,
                                            MovieNotePK = p.MovieNotePK,
                                            UserProfileFK = p.UserProfileFK,
                                            Note = p.Note,
                                            MovieName = movie.MovieName
                                        }).ToList();

                    foreach (var item in movie.MovieNotes)
                    {
                        string userName = (from p in db.Query<UserProfile>()
                                           where p.UserProfilePK == item.UserProfileFK
                                           select p.UserName).FirstOrDefault();

                        if (userName != null)
                            item.UserName = userName;
                    }

                    List<UserMovieRating> ratings = db.Query<UserMovieRating>().Where(p => p.MovieFK == movie.MoviePK).ToList();

                    if (ratings.Count > 0)
                        movie.MovieRating = ratings.Select(item => item.Rating).Average();
                    else
                        movie.MovieRating = 0;
                }

                return new { ResultState = true, ResultMessage = "İşlem Başarılı", Data = movie };
            }
            catch (Exception err)
            {
                return new { ResultState = false, ResultMessage = err.Message };
            }
        }
        #endregion

        #region saveMovie
        public dynamic saveMovie(Movie movie)
        {
            try
            {
                using (ISession db = DBSession.OpenUserSession(_configuration))
                {
                    Movie isSaved = db.Query<Movie>().Where(p => p.syncId == movie.syncId).FirstOrDefault();
                    if (isSaved != null)
                        movie.MoviePK = isSaved.MoviePK;

                    using (ITransaction tr = db.BeginTransaction())
                    {
                        if (movie.MoviePK == null || movie.MoviePK.ToString().Contains("00000"))
                        {
                            movie.MoviePK = Guid.NewGuid();
                        }

                        db.SaveOrUpdate(movie);
                        tr.Commit();
                    }
                }

                return new { ResultState = true, ResultMessage = "Kayıt Başarılı", Data = movie };
            }
            catch (Exception err)
            {
                return new { ResultState = false, ResultMessage = err.Message };
            }
        }
        #endregion

        #region getMovies
        public dynamic getMovies(PagingObject pagingObject)
        {
            try
            {
                List<Movie> movies = new List<Movie>();

                using (ISession db = DBSession.OpenUserSession(_configuration))
                {
                    movies = (from p in db.Query<Movie>()
                              select p).Skip(pagingObject.startIndex).Take(pagingObject.countOfRecord).ToList();
                }

                return new { ResultState = true, ResultMessage = "İşlem Başarılı", Data = movies };
            }
            catch (Exception err)
            {
                return new { ResultState = false, ResultMessage = err.Message };
            }
        }
        #endregion

        #region saveMovieNote
        public dynamic saveMovieNote(UserMovieNote movieNote)
        {
            try
            {
                using (ISession db = DBSession.OpenUserSession(_configuration))
                {
                    using (ITransaction tr = db.BeginTransaction())
                    {
                        if (movieNote.MovieNotePK == null || movieNote.MovieNotePK.ToString().Contains("00000"))
                        {
                            movieNote.MovieNotePK = Guid.NewGuid();
                        }

                        db.SaveOrUpdate(movieNote);
                        tr.Commit();
                    }
                }

                return new { ResultState = true, ResultMessage = "Kayıt Başarılı", Data = movieNote };
            }
            catch (Exception err)
            {
                return new { ResultState = false, ResultMessage = err.Message };
            }
        }
        #endregion

        #region saveUserMovieRating
        public dynamic saveUserMovieRating(UserMovieRating userMovieRating)
        {
            try
            {
                using (ISession db = DBSession.OpenUserSession(_configuration))
                {
                    using (ITransaction tr = db.BeginTransaction())
                    {
                        if (userMovieRating.UserMovieRatingPK == null || userMovieRating.UserMovieRatingPK.ToString().Contains("00000"))
                        {
                            userMovieRating.UserMovieRatingPK = Guid.NewGuid();
                        }

                        db.SaveOrUpdate(userMovieRating);
                        tr.Commit();
                    }
                }

                return new { ResultState = true, ResultMessage = "Kayıt Başarılı", Data = userMovieRating };
            }
            catch (Exception err)
            {
                return new { ResultState = false, ResultMessage = err.Message };
            }
        }
        #endregion
    }
}
