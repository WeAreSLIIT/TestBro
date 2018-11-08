using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;

namespace RestApi.Controllers
{
    //GetAll . select
    [Route("v1/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public List<Product> pros = new List<Product>();
        

        //*
        public List<Product> products = new List<Product>()
        {
            new Product{

                Id="1",
                Name="product1"
            },

             new Product{

                Id="2",
                Name="product2"
            }
        };

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(products);
        }

        //Get . select
        [Route("{Id}")]
        [HttpGet]
        public IActionResult GetProductById(string Id)
        {

           		
            string conString = "User Id=SDUORA;Password=123456;Data Source=172.21.112.44:1521/xe;";

            //Using TNSNAMES.ORA file in executable root, or where env TNS_ADMIN points
            //string conString = "User Id=hr;Password=hr;Data Source=orclpdb;";



            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;

                        //Use the command to display employee names from 
                        // the EMPLOYEES table
                        cmd.CommandText = "select * from EMP";


                        //Execute the command and use DataReader to display the data
                        OracleDataReader reader = cmd.ExecuteReader();
                        
                        while (reader.Read())
                        {


                            System.Diagnostics.Debug.WriteLine("Employee ID: " + reader.GetInt16(0));
                            System.Diagnostics.Debug.WriteLine("Employee Name: " + reader.GetString(1));

                            Product Newpro = new Product();
                            Newpro.Id= reader.GetInt16(0).ToString();
                            Newpro.Name = reader.GetString(1);

                            pros.Add(Newpro);

                        }

                        reader.Dispose();
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }
            }


            return Ok(pros);
        }



        //Post . insert
        // error handle karanna 
        [HttpPost]
        public IActionResult Addproduct([FromBody]Product product)
        {
            var newproduct = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = product.Name

            };

            products.Add(newproduct);
            return Ok(products);
        }


        //put  - update
        
        [HttpPut]

        public IActionResult Putproduct(String Id,[FromBody]Product product)  
        {
            var pro = products.FirstOrDefault(p => p.Id.Equals(product.Id));
            {
                if (pro == null || pro.Id !=product.Id)

                {                
                     return NotFound("Ehema akak ne");
                }
                else {
                    pro.Name = product.Name;
                }

            };
            return Ok(products);
        }

        //Delete 
        
        [HttpDelete]
        public IActionResult Deleteproduct(String Id, [FromBody]Product product)
        {
            var prod = products.FirstOrDefault(p => p.Id.Equals(product.Id));

            if (prod == null)
            {
                return NotFound("No product");
            }
            else
            {

                products.Remove(prod);               
                return Ok(products);
            }

        }
    }

}