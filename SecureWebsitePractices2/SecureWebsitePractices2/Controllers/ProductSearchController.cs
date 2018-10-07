﻿using SecureWebsitePractices2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SecureWebsitePractices2.Controllers
{
    public class ProductSearchController : Controller
    {
        // GET: ProductSearch
        public ActionResult Index(string productid, string name)
        {
            var model = new ProductModel();

            if (productid == String.Empty && name == String.Empty || (productid == null && name == null))
            {
                model.ProductList = GetProductsId("1");
            }
            if (productid != null && (name == null || name == String.Empty))
            {
                model.ProductList = GetProductsId(productid);
                model.SearchedById = true;
                model.ProductKey = productid;
            }
            else if (name != null)
            {
                model.SearchedByName = true;
                model.ProductName = name;
                model.ProductList = GetProductsName(name);
            }
            else
            {
                return View("Index", model);

            }
            return View(model);
        }

        public ActionResult SearchValue(string id, string name)
        {
            return RedirectToAction("Index", new { productid = id, name = name });
        }

        public List<ProductModel> GetProductsId(string prodID)
        {
            var result = new List<ProductModel>();

            //TODO: SQL 1 --> Parameterisation
           // string productKey = prodID; //--> SQL 1 Parameter 
            ////var sqlString = "SELECT * FROM Product WHERE ProductKey = @ProductKey";   //--> SQL 1 Parameter 

            //var sqlString = "SELECT * FROM Product WHERE ProductKey = " + prodID; // original
            // -->
            var connString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var sqlString = "FetchProducts"; // --> SQL 2-Stored Procedures

            //--> SQL 3 Input validation-type conversion 
            int productKey;
            if (!int.TryParse(prodID, out productKey)){

                throw new ApplicationException("Not a valid input format");
            }
            //--> SQL 3 Input validation-type conversion  

            using (var conn = new SqlConnection(connString))
            {
                var command = new SqlCommand(sqlString, conn);
                // -->
                command.CommandType = CommandType.StoredProcedure; //--> SQL 2-Stored Procedures
                SqlParameter key = command.Parameters.AddWithValue("@ProductKey", productKey); //--> SQL 1 Parameter 
                // -->
                command.Connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        ProductModel Products = new ProductModel();

                        Products.ProductKey = reader.GetInt32(0).ToString();
                        Products.ProductAlternateKey = reader.GetString(1);
                        Products.ProductName = reader.GetString(5);
                        Products.StockLevel = reader.GetInt16(9);

                        result.Add(Products);
                    }
                }
            }
            return result;
        }

        public List<ProductModel> GetProductsName(string prodName)
        {
            var result = new List<ProductModel>();

            ProductModel Products = new ProductModel();

            Products.Searched = Request.QueryString["prodName"];


            var sqlString = "SELECT * FROM Product";
            var connString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var conn = new SqlConnection(connString))
            {
                var command = new SqlCommand(sqlString, conn);
                command.Connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products.ProductKey = reader.GetInt32(0).ToString();
                        Products.ProductAlternateKey = reader.GetString(1);
                        Products.ProductName = reader.GetString(5);
                        Products.StockLevel = reader.GetInt16(11);

                        result.Add(Products);
                    }
                }
            }
            //result = Products.Any(m => m.ProductName.Contains("Products.Searched, StringComparison.OrdinalIgnoreCase") >=0);//"SELECT * FROM Product WHERE ProductName = " + Products.Searched;
            return result;
        }
    }

}
