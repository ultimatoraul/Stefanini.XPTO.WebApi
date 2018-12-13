﻿using System;
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
  public class ProductsController : ApiController
  {
    private XptoContext db = new XptoContext();

    // GET: api/Products
    public IQueryable<Product> GetProducts()
    {
      return db.Products;
    }

    // GET: api/Products/5
    [ResponseType(typeof(Product))]
    public async Task<IHttpActionResult> GetProduct(int id)
    {
      Product product = await db.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      return Ok(product);
    }

    // PUT: api/Products/5
    [ResponseType(typeof(void))]
    public async Task<IHttpActionResult> PutProduct(int id, Product product)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (id != product.ID)
      {
        return BadRequest();
      }

      db.Entry(product).State = EntityState.Modified;

      try
      {
        await db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ProductExists(id))
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

    // POST: api/Products
    [ResponseType(typeof(Product))]
    public async Task<IHttpActionResult> PostProduct(Product product)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      db.Products.Add(product);

      try
      {
        await db.SaveChangesAsync();
      }
      catch (DbUpdateException)
      {
        if (ProductExists(product.ID))
        {
          return Conflict();
        }
        else
        {
          throw;
        }
      }

      return CreatedAtRoute("DefaultApi", new { id = product.ID }, product);
    }

    // DELETE: api/Products/5
    [ResponseType(typeof(Product))]
    public async Task<IHttpActionResult> DeleteProduct(int id)
    {
      Product product = await db.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }

      db.Products.Remove(product);
      await db.SaveChangesAsync();

      return Ok(product);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool ProductExists(int id)
    {
      return db.Products.Count(e => e.ID == id) > 0;
    }
  }
}