using DO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Yuce.MulakatApi.Hubs
{
    public class BackendService : IHostedService, IDisposable
    {
        public IConfiguration _configuration;

        public BackendService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Timer syncTimer { get; set; }
        private int PageIndex { get; set; } = 1;
        private bool isBusy { get; set; } = false;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            #region 20 dk da bir yeni sayfa çek ve kaydet. En fazla 5 sayfadan sonra 1. sayfaya dön kayıtlı olan varsa update et
            syncTimer = new Timer(o =>
                {
                    if (!isBusy)
                    {
                        try
                        {
                            isBusy = true;

                            string accessToken = @"eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI3OTUwMTcwMjEwYzJhMTRmZTJlMjVhZTg0OTdjYmVlOCIsInN1YiI6IjVmZjZjM2Y0NWE3ODg0MDAzZTJlYzA0OSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.__KJNE_Tm3RuXJGMbrxlFkf2xszNZ0WLjgQYmseNurs";

                            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.themoviedb.org/4/list/" + PageIndex.ToString());
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                            HttpClient client = new HttpClient();
                            HttpResponseMessage response = client.SendAsync(request).Result;

                            string json = response.Content.ReadAsStringAsync().Result;

                            dynamic result = new ExpandoObject();
                            result = JsonConvert.DeserializeObject<dynamic>(json);

                            foreach (dynamic item in result.results)
                            {
                                try
                                {
                                    string dateStr = item.release_date;
                                    string[] dateSplit = dateStr.Split('-');

                                    Movie movie = new Movie();
                                    movie.MoviePK = Guid.NewGuid();
                                    movie.MovieName = item.title;
                                    movie.syncId = item.id;
                                    movie.OverView = item.overview;
                                    movie.ReleaseDate = new DateTime(Convert.ToInt32(dateSplit[0]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[2]));
                                    movie.Country = item.original_language;

                                    var saveResult = new DA.MovieDA(_configuration).saveMovie(movie);
                                }
                                catch
                                {

                                }
                            }
                            isBusy = false;
                        }
                        catch (Exception err)
                        {
                            isBusy = false;
                            var errMsg = err.Message;
                        }

                        PageIndex++;

                        if (PageIndex > 5)
                            PageIndex = 1;
                    }
                },
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(20));
            #endregion

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            isBusy = false;
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            syncTimer?.Dispose();
        }
    }
}
