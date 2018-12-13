using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Stefanini.XPTO.WebApi.DAL;
using Stefanini.XPTO.WebApi.Models;

namespace Stefanini.XPTO.WebApi.Controllers
{
    public class ProductClientsController : ApiController
    {
        private XptoContext db = new XptoContext();

        // GET: api/ProductClients
        public IQueryable<ProductClient> GetProductsClient()
        {
            return db.ProductsClient;
        }

        // GET: api/ProductClients/5
        [ResponseType(typeof(ProductClient))]
        public async Task<IHttpActionResult> GetProductClient(int id)
        {
            ProductClient productClient = await db.ProductsClient.FindAsync(id);
            if (productClient == null)
            {
                return NotFound();
            }

            return Ok(productClient);
        }

        // PUT: api/ProductClients/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProductClient(int id, ProductClient productClient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productClient.ID)
            {
                return BadRequest();
            }

            db.Entry(productClient).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProductClients
        [ResponseType(typeof(ProductClient))]
        public async Task<IHttpActionResult> PostProductClient(ProductClient productClient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProductsClient.Add(productClient);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = productClient.ID }, productClient);
        }

        // DELETE: api/ProductClients/5
        [ResponseType(typeof(ProductClient))]
        public async Task<IHttpActionResult> DeleteProductClient(int id)
        {
            ProductClient productClient = await db.ProductsClient.FindAsync(id);
            if (productClient == null)
            {
                return NotFound();
            }

            db.ProductsClient.Remove(productClient);
            await db.SaveChangesAsync();

            return Ok(productClient);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductClientExists(int id)
        {
            return db.ProductsClient.Count(e => e.ID == id) > 0;
        }
    }
}