using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ListView.Models;
using System.Net.Http;
using ListView.Views.Settings;
using Microsoft.Extensions.Options;

namespace ListView.Controllers
{
    public class ListViewModel
    {
        public int albumId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
    }
    public class HomeController : Controller
    {
        private IHttpClientFactory _httpClient;
        private IOptions<TypicodeOptions> _typicode;
        public IList<ListViewModel> Pages { get; set; }
        public HomeController(IHttpClientFactory httpClient, IOptions<TypicodeOptions> typicode)
        {
            this._httpClient = httpClient;
            //API options to invoke with http client
            this._typicode = typicode;
        }
        public async Task<IActionResult> Index()
        {
            using (HttpClient client = this._httpClient.CreateClient())
            {
                var response = await client.GetAsync(this._typicode.Value.Api + this._typicode.Value.AllPhotos, HttpCompletionOption.ResponseContentRead);

                if (response.IsSuccessStatusCode)
                {
                    this.Pages = await response.Content.ReadAsAsync<IList<ListViewModel>>();
                }
                System.Diagnostics.Debug.WriteLine(this.Pages[0]);

                return View(this.Pages);
            }
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            using (HttpClient client = this._httpClient.CreateClient())
            {
                var response = await client.GetAsync(this._typicode.Value.Api + this._typicode.Value.AllPhotos + "/" + id.ToString(), HttpCompletionOption.ResponseContentRead);
                ListViewModel Page = new ListViewModel();
                if (response.IsSuccessStatusCode)
                {
                    Page = await response.Content.ReadAsAsync<ListViewModel>();
                }
                return View(Page);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
