using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IDynamoDbService _dynamoDbService;

        public ItemsController(IDynamoDbService dynamoDbService)
        {
            _dynamoDbService = dynamoDbService;
        }

        // GET /Items
        public async Task<IActionResult> Index()
        {
            var items = await _dynamoDbService.GetAllItemsAsync();
            return View(items);
        }

        // GET /Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST /Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            await _dynamoDbService.PutItemAsync(item);
            return RedirectToAction(nameof(Index));
        }

        // GET /Items/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var item = await _dynamoDbService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST /Items/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _dynamoDbService.DeleteItemAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
