using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using SelfHost;

namespace ConsoleApp7
{
    public class ProductsRepository
    {
        static HttpClient client = new HttpClient();
        static void ListAllProducts()
        {
            HttpResponseMessage resp = client.GetAsync("api/products").Result;
            resp.EnsureSuccessStatusCode();

            var products = resp.Content.ReadAsStringAsync();
            Console.WriteLine(products);
        }

        static void ListProduct(int id)
        {
            var resp = client.GetAsync(string.Format("api/products/{0}", id)).Result;
            resp.EnsureSuccessStatusCode();

            var product = resp.Content.ReadAsStringAsync();
            Console.WriteLine(product);
        }

        static void ListProducts(string category)
        {
            Console.WriteLine("Products in '{0}':", category);

            string query = string.Format("api/products?category={0}", category);

            var resp = client.GetAsync(query).Result;
            resp.EnsureSuccessStatusCode();

            var products = resp.Content.ReadAsStringAsync();
            Console.WriteLine(products);
        }
    }
}
